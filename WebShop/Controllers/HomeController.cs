using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using WebShop.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;

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

                _context.Maufacturers.AddRange(company1, company2, company3, company4);
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
                return Json(new { result = true });
            }
            return NotFound();
        }

        public async Task<IActionResult> AddUser(int? id)
        {
            ViewBag.RoleList = _context.Roles.ToList();
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
            /*if (isValuesNull(user))
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["AlertMessage"] = "Не все данные заполненны";
				ViewBag.RoleList = _context.Roles.ToList();
				return View(user);
            }*/
        }

        /*private bool isValuesNull(object obj)
        {
            foreach(PropertyInfo property in obj.GetType().GetProperties())
            {
                if(property.GetValue(obj) == null)
                    return false;
            }
            return true;
        }*/

        public JsonResult Test(int id)
        {
            return Json(_context.Users.Where(u => u.UserRole.Id == id));
        }

        public ActionResult Index(int? role, string? name, SortState sortOrder = SortState.IdAsc)
        {
            ViewBag.SortOrder = 1;
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);
            if (role != null && role != 0)
                users = users.Where(p => p.UserRole.Id == role);

            if (!string.IsNullOrEmpty(name))
                users = users.Where(p => p.UserSurname!.Contains(name) ||
                                    p.UserName!.Contains(name) ||
                                    p.UserPantonymic!.Contains(name));

            ViewData["IdSort"] = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
            ViewData["SurnameSort"] = sortOrder == SortState.SurnameAsc ? SortState.SurnameDesc : SortState.SurnameAsc;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["PantonymicSort"] = sortOrder == SortState.PantonymicAsc ? SortState.PantonymicDesc : SortState.PantonymicAsc;
            ViewData["LoginSort"] = sortOrder == SortState.LoginAsc ? SortState.LoginDesc : SortState.LoginAsc;
            ViewData["PasswordSort"] = sortOrder == SortState.PasswordAsc ? SortState.PasswordDesc : SortState.PasswordAsc;
            ViewData["PhoneNumberSort"] = sortOrder == SortState.PhoneNumberAsc ? SortState.PhoneNumberDesc : SortState.PhoneNumberAsc;
            ViewData["MoneySort"] = sortOrder == SortState.MoneyAsc ? SortState.MoneyDesc : SortState.MoneyAsc;
            ViewData["RoleSort"] = sortOrder == SortState.RoleAsc ? SortState.RoleDesc : SortState.RoleAsc;

            users = sortOrder switch
            {
                SortState.IdDesc => users.OrderByDescending(s => s.Id),
                SortState.SurnameAsc => users.OrderBy(s => s.UserSurname),
                SortState.SurnameDesc => users.OrderByDescending(s => s.UserSurname),
                SortState.NameAsc => users.OrderBy(s => s.UserName),
                SortState.NameDesc => users.OrderByDescending(s => s.UserName),
                SortState.PantonymicAsc => users.OrderBy(s => s.UserPantonymic),
                SortState.PantonymicDesc => users.OrderByDescending(s => s.UserPantonymic),
                SortState.LoginAsc => users.OrderBy(s => s.UserLogin),
                SortState.LoginDesc => users.OrderByDescending(s => s.UserLogin),
                SortState.PasswordAsc => users.OrderBy(s => s.UserPassword),
                SortState.PasswordDesc => users.OrderByDescending(s => s.UserPassword),
                SortState.PhoneNumberAsc => users.OrderBy(s => s.UserPhoneNumber),
                SortState.PhoneNumberDesc => users.OrderByDescending(s => s.UserPhoneNumber),
                SortState.MoneyAsc => users.OrderBy(s => (double)s.UserMoney),
                SortState.MoneyDesc => users.OrderByDescending(s => (double)s.UserMoney),
                SortState.RoleAsc => users.OrderBy(s => s.UserRole!.Id),
                SortState.RoleDesc => users.OrderByDescending(s => s.UserRole!.Id),
                _ => users.OrderBy(s => s.Id),
            };


            List<Role> roles = _context.Roles.ToList();
            roles.Insert(0, new Role { RoleName = "Все", Id = 0 });

            UserListViewModel userListViewModel = new UserListViewModel
            {
                Users = users.ToList(),
                Roles = new SelectList(roles, "Id", "RoleName", role),
                Name = name
            };

            return View(userListViewModel);
        }
    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
