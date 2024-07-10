using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class ManufacturerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
