using PagedList;

namespace App.Models
{
    public class InicioArchivosDto(StaticPagedList<Archivo> pLista) : InicioDto
    {
        public StaticPagedList<Archivo> ListaArchivos { get; set; } = pLista;
    }
}
