using App.Models;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class ArchivosController : Controller
    {
        private readonly MinioService _minioService;
        private readonly ArchivosService _archivosService;
        public ArchivosController(MinioService minioService, ArchivosService archivosService)
        {
            _minioService = minioService;
            _archivosService = archivosService;
        }
        [HttpGet]
        public async Task<ActionResult> Archivos(string nombre, int? page)
        {
            try
            {
                ViewBag.SearchTermNombre = nombre;
                return View(await _archivosService.GetArchivos(nombre, (page ?? 0) == 0 ? 1 : page.Value, 30));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Hubo un error al visualizar esta página. El archivo " + ex.Message + " tiene errores.";
                return RedirectToAction("NoResultado", "Error");
            }
        }

        [HttpPost]
        public IActionResult BuscarArchivo(string nombre)
        {
            return RedirectToAction("Archivos", new { nombre });
        }

        [HttpGet]
        public IActionResult SubirArchivo()
        {
            return View(new SubirArchivoModel());
        }

        [HttpPost]
        public async Task<IActionResult> SubirArchivo(SubirArchivoModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ArchivoAdjunto.ContentType.Equals("application/pdf") && await _archivosService.SubirArchivo(model.ArchivoAdjunto))
                {
                    TempData["success"] = "El archivo se subió correctamente.";
                    return RedirectToAction("Archivos", "Archivos");
                }
                else
                {
                    TempData["error"] = "Hubo un error al subir el archivo.";
                    return View("SubirArchivo", model);
                }
                
            }
            else
            {
                return View("SubirArchivo", model);
            }
        }
    }
}
