using Minio.DataModel.Args;
using Minio;
using App.Models;
using PagedList;
using Minio.ApiEndpoints;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Mvc;

namespace App.Services
{
    public class ArchivosService
    {
        private readonly MinioService _minioService;
        public ArchivosService(MinioService minioService)
        {
            _minioService = minioService;
        }
        #region MÉTODOS PÚBLICOS
        public async Task<InicioArchivosDto> GetArchivos(string nombre, int pageNumber, int pageSize)
        {
            var listaArchivos = await GetDocumentosMinio();

            if (nombre != null)
            {
                listaArchivos = FiltrarPorNombre(listaArchivos, nombre);
            }

            return GetInicioArchivos(listaArchivos, pageNumber, pageSize);
        }
        #endregion

        #region MÉTODOS PRIVADOS
        private async Task<List<Archivo>> GetDocumentosMinio()
        {
            IMinioClient cliente = _minioService.GetCliente();
            List<Archivo> listaArchivos = [];

            var listArgs = new ListObjectsArgs()
                        .WithBucket(Environment.GetEnvironmentVariable("MINIO_BUCKET"))
                        .WithRecursive(true);

            var items = await cliente.ListObjectsAsync(listArgs).ToList();

            foreach (var item in items)
            {
                Archivo doc = new()
                {
                    Nombre = item.Key
                };
                listaArchivos.Add(doc);
            }

            return listaArchivos;
        }

        private InicioArchivosDto GetInicioArchivos(List<Archivo> listaArchivos, int pageNumber, int pageSize)
        {
            try
            {
                StaticPagedList<Archivo> pagedList = new StaticPagedList<Archivo>(
                        listaArchivos
                            .OrderBy(i => i.Nombre)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize),
                        pageNumber,
                        pageSize,
                        listaArchivos.Count);

                InicioArchivosDto inicio = new(pagedList);

                return inicio;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private List<Archivo> FiltrarPorNombre(List<Archivo> archivos, string nombre)
        {
            List<Archivo> listaFiltrada = [];

            foreach(Archivo archivo in archivos)
            {
                if (archivo.Nombre.Contains(nombre))
                {
                    listaFiltrada.Add(archivo);
                }
            }

            return listaFiltrada;
        }

        public async Task<bool> SubirArchivo(IFormFile file)
        {
            try
            {
                byte[] contenidoArchivo;
                using (MemoryStream memoryStream = new())
                {
                    await file.CopyToAsync(memoryStream);
                    contenidoArchivo = memoryStream.ToArray();
                }
                var putObjectArgs = new PutObjectArgs()
                    .WithStreamData(new MemoryStream(contenidoArchivo))
                    .WithObjectSize(contenidoArchivo.Length)
                    .WithBucket(Environment.GetEnvironmentVariable("MINIO_BUCKET"))
                    .WithObject(file.FileName);
                var putObjectResponse = await _minioService.GetCliente().PutObjectAsync(putObjectArgs);
                return putObjectResponse != null;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
