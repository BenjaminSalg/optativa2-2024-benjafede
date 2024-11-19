using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class SubirArchivoModel
    {
        [Required(ErrorMessage = "Por favor, seleccione un archivo.")]
        [Display(Name = "Archivo Adjunto")]
        public IFormFile ArchivoAdjunto { get; set; }
    }
}
