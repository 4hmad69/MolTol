
using Microsoft.AspNetCore.Mvc;

namespace MolTol.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
