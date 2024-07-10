using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
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

			if (!_context.Roles.Any())
			{
				Role role1 = new Role { RoleName = "Администратор" };
				Role role2 = new Role { RoleName = "Менеджер" }; 
                Role role3 = new Role { RoleName = "Клиент" };

				_context.Roles.AddRange(role1, role2, role3);
				_context.SaveChanges();
			}

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
            user.UserRole.RoleName = (from a in _context.Roles where a.Id == user.UserRole.Id select a.RoleName).FirstOrDefault();
			_context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult Index(int? role, string? name)
        {
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);
            if (role != null && role != 0)
                users = users.Where(p => p.UserRole.Id == role);

                if (!string.IsNullOrEmpty(name))
                users = users.Where(p => p.UserSurname!.Contains(name) || 
                                    p.UserName!.Contains(name) || 
                                    p.UserPantonymic!.Contains(name));

            List<Role> roles = _context.Roles.ToList();
            roles.Insert(0, new Role { RoleName = "Все", Id = 0 });

            UserListViewModel viewModel = new UserListViewModel
            {
                Users = users.ToList(),
                Roles = new SelectList(roles, "Id", "RoleName",role),
                Name = name
            };

			return View(viewModel);
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
