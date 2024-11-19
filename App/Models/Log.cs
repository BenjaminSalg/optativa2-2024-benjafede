namespace App.Models
{
    public class Log
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreArchivo { get; set; }
        public long AccionId { get; set; }
    }
}
