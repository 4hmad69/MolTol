
using Microsoft.AspNetCore.Mvc;

namespace MolTol.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
