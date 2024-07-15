using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using WebShop.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebShop.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<HomeController> _logger;

        public UserController(ILogger<HomeController> logger, ApplicationContext context)
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

        public async Task<IActionResult> Sort(string SortType, bool isAscending)
        {
            ViewBag.SortType = SortType;
            ViewBag.isAscebding = isAscending;

            IQueryable<User> users = _context.Users.Include(p => p.UserRole);

            switch (SortType)
            {
                case "Id":
                    users = isAscending ? users.OrderBy(x => x.Id) : users.OrderByDescending(x => x.Id);
                    break;
                case "Name":
                    users = isAscending ? users.OrderBy(x => x.UserName) : users.OrderByDescending(x => x.UserName);
                    break;
                case "Surname":
                    users = isAscending ? users.OrderBy(x => x.UserSurname) : users.OrderByDescending(x => x.UserSurname);
                    break;
                case "Pantonymic":
                    users = isAscending ? users.OrderBy(x => x.UserPantonymic) : users.OrderByDescending(x => x.UserPantonymic);
                    break;
                case "Login":
                    users = isAscending ? users.OrderBy(x => x.UserLogin) : users.OrderByDescending(x => x.UserLogin);
                    break;
                case "Password":
                    users = isAscending ? users.OrderBy(x => x.UserPassword) : users.OrderByDescending(x => x.UserPassword);
                    break;
                case "PhoneNumber":
                    users = isAscending ? users.OrderBy(x => (double)x.UserPhoneNumber) : users.OrderByDescending(x => (double)x.UserPhoneNumber);
                    break;
                case "Money":
                    users = isAscending ? users.OrderBy(x => (double)x.UserMoney) : users.OrderByDescending(x => (double)x.UserMoney);
                    break;
                case "Role":
                    users = isAscending ? users.OrderBy(x => (double)x.UserRole.Id) : users.OrderByDescending(x => (double)x.UserRole.Id);
                    break;
            }

            return PartialView("_UserPartial",await users.ToListAsync());
        }

        public IActionResult Index(int? role, string? name)
        {
            List<Role> roles = _context.Roles.ToList();
            roles.Insert(0, new Role { RoleName = "Все", Id = 0 });

            UserListViewModel userListViewModel = new UserListViewModel
            {
                FiltrList = roles,
                SearchText = name
            };
            return View(userListViewModel);
        }

        public async Task<IActionResult> Filtr(int filtrId)
        {
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);
            if (filtrId != null && filtrId != 0)
                users = users.Where(p => p.UserRole.Id == filtrId);
            return PartialView("_UserPartial", await users.ToListAsync());
        }

        public async Task<IActionResult> Search(string searchText)
        {
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);

            if (searchText != null)
            {
                users = users.Where(s => s.UserSurname.Contains(searchText) || 
                                            s.UserSurname.Contains(searchText) || 
                                            s.UserPantonymic.Contains(searchText) || 
                                            s.UserLogin.Contains(searchText));
            }

            return PartialView("_UserPartial", await users.ToListAsync());
        }

        public async Task<IActionResult> GetData(string SortType, bool isAscending)
        {

            ViewBag.SortType = SortType;
            ViewBag.isAscebding = isAscending;

            IQueryable<User> users = _context.Users.Include(p => p.UserRole);

            switch (SortType)
            {
                case "Id":
                    users = isAscending ? users.OrderBy(x => x.Id) : users.OrderByDescending(x => x.Id);
                    break;
                case "Name":
                    users = isAscending ? users.OrderBy(x => x.UserName) : users.OrderByDescending(x => x.UserName);
                    break;
                case "Surname":
                    users = isAscending ? users.OrderBy(x => x.UserSurname) : users.OrderByDescending(x => x.UserSurname);
                    break;
                case "Pantonymic":
                    users = isAscending ? users.OrderBy(x => x.UserPantonymic) : users.OrderByDescending(x => x.UserPantonymic);
                    break;
                case "Login":
                    users = isAscending ? users.OrderBy(x => x.UserLogin) : users.OrderByDescending(x => x.UserLogin);
                    break;
                case "Password":
                    users = isAscending ? users.OrderBy(x => x.UserPassword) : users.OrderByDescending(x => x.UserPassword);
                    break;
                case "PhoneNumber":
                    users = isAscending ? users.OrderBy(x => (double)x.UserPhoneNumber) : users.OrderByDescending(x => (double)x.UserPhoneNumber);
                    break;
                case "Money":
                    users = isAscending ? users.OrderBy(x => (double)x.UserMoney) : users.OrderByDescending(x => (double)x.UserMoney);
                    break;
                case "Role":
                    users = isAscending ? users.OrderBy(x => (double)x.UserRole.Id) : users.OrderByDescending(x => (double)x.UserRole.Id);
                    break;
            }

            return PartialView("_UserPartial", await users.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

