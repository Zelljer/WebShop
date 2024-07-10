using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Products()
        {
            return View();
        }
    }
}
