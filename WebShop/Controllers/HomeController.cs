using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebShop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebShop.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;


            if (!_context.Maufacturers.Any())
            {
                Maufacturer company1 = new Maufacturer { MaufacturerName = "Компания1" };
                Maufacturer company2 = new Maufacturer { MaufacturerName = "Компания2" };
                Maufacturer company3 = new Maufacturer { MaufacturerName = "Компания3" };
                Maufacturer company4 = new Maufacturer { MaufacturerName = "Компания4" };

                _context.Maufacturers.AddRange(company1, company3, company2, company4);
                _context.SaveChanges();
            }
        }

        public IActionResult Index()
        {
            return View(_context.Users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = new User { Id = id.Value };
                _context.Entry(user).State = EntityState.Deleted;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> AddUser(int? id)
        {
            if (id != null)
            {
                User? user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null) return View(user); 
            }
            return View(new User());
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        } 

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
