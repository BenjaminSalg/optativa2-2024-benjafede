using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult NoResultado() => View(new InicioDto());
    }
}
