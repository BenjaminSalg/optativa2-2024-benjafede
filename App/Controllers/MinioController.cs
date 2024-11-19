using App.Services;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Minio;
using App.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    public class MinioController : Controller
    {
        protected readonly SqlContext _sqlContext;
        public MinioController(SqlContext sqlContext)
        {
            _sqlContext = sqlContext;
        }

        #region MÉTODOS PÚBLICOS
        [HttpGet]
        public async Task<ActionResult> VisualizarArchivo(string nombre)
        {
            var log = new Log
            {
                Fecha = DateTime.Now.ToLocalTime(),
                NombreArchivo = nombre,
                AccionId = 2,
            };
            _sqlContext.Logs.Add(log);

            await _sqlContext.SaveChangesAsync();

            return await ManejarArchivo(nombre, true);
        }

        [HttpGet]
        public async Task<ActionResult> DescargarArchivo(string nombre)
        {
            var log = new Log
            {
                Fecha = DateTime.Now.ToLocalTime(),
                NombreArchivo = nombre,
                AccionId = 1,
            };
            _sqlContext.Logs.Add(log);

            await _sqlContext.SaveChangesAsync();

            return await ManejarArchivo(nombre, false);
        }
        #endregion
        #region MÉTODOS PRIVADOS
        private async Task<ActionResult> ManejarArchivo(string nombre, bool visualizar)
        {
            if (nombre == null)
            {
                TempData["error"] = "Atributos del archivo nulo.";
                return RedirectToAction("NoResultado", "Error");
            }
            try
            {
                MinioService clienteMinio = new();
                var cliente = clienteMinio.GetCliente();
                cliente.Build();

                if (!await BuscarArchivoMinio(nombre, cliente))
                {
                    TempData["error"] = $"El archivo no existe en Minio.";
                    return RedirectToAction("NoResultado", "Error");
                }

                var archivo = await LeerArchivoMinio(nombre, cliente);
                if (!visualizar)
                {
                    Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{nombre}\"");
                }

                return File(archivo, "application/pdf");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error al {(visualizar ? "visualizar" : "descargar")} el archivo: {ex.Message}";
                return RedirectToAction("NoResultado", "Error");
            }
        }
        private static async Task<bool> BuscarArchivoMinio(string nombre, IMinioClient pCliente)
        {
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                    .WithBucket(Environment.GetEnvironmentVariable("MINIO_BUCKET"))
                    .WithObject(nombre);
                var metadata = await pCliente.StatObjectAsync(statObjectArgs);
                return metadata.Size != 0;
            }
            catch (MinioException) { return false; }
        }

        private static async Task<byte[]> LeerArchivoMinio(string nombre, IMinioClient pCliente)
        {
            try
            {
                MemoryStream memoryStream = new();
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                    .WithBucket(Environment.GetEnvironmentVariable("MINIO_BUCKET"))
                    .WithObject(nombre)
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    });
                await pCliente.GetObjectAsync(getObjectArgs);
                return memoryStream.ToArray();
            }
            catch { return []; }
        }
        #endregion
    }
}
