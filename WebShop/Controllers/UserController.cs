using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationContext _context;
        public UserController(ApplicationContext context) => _context = context;

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
            user.UserRole.RoleName = (from a in _context.Roles 
                                      where a.Id == user.UserRole.Id 
                                      select a.RoleName).FirstOrDefault();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
 
        public async Task<IActionResult> DataSettings(int sortType, int filtrType, string searchText)
        {
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);

            if (filtrType != 0)
                users = users.Where(p => p.UserRole.Id == filtrType);

            users = sortType switch
            {
                1 => users.OrderBy(x => x.Id),
                2 => users.OrderByDescending(x => x.Id),
                3 => users.OrderBy(x => x.UserName),
                4 => users.OrderByDescending(x => x.UserName),
                5 => users.OrderBy(x => x.UserSurname),
                6 => users.OrderByDescending(x => x.UserSurname),
                7 => users.OrderBy(x => x.UserPantonymic),
                8 => users.OrderByDescending(x => x.UserPantonymic),
                9 => users.OrderBy(x => x.UserLogin),
                10 => users.OrderByDescending(x => x.UserLogin),
                11 => users.OrderBy(x => (double)x.UserPhoneNumber),
                12 => users.OrderByDescending(x => (double)x.UserPhoneNumber),
                13 => users.OrderBy(x => (double)x.UserMoney),
                14 => users.OrderByDescending(x => (double)x.UserMoney),
                15 => users.OrderBy(x => (double)x.UserRole.Id),
                16 => users.OrderByDescending(x => (double)x.UserRole.Id),
                _ => users
            };

            if (searchText != null)
                users = users.Where(s => s.UserSurname.Contains(searchText) ||
                                           s.UserSurname.Contains(searchText) ||
                                           s.UserPantonymic.Contains(searchText) ||
                                           s.UserLogin.Contains(searchText));


            return PartialView("_UserPartial",await users.ToListAsync());
        }

        public IActionResult Index()
        {
            List<Role> roles = _context.Roles.ToList();
            roles.Insert(0, new Role { RoleName = "Все", Id = 0 });

            UserListViewModel userListViewModel = new UserListViewModel
            {
                FiltrList = roles
            };
            return View(userListViewModel);
        }

        public async Task<IActionResult> GetData()
        {
            IQueryable<User> users = _context.Users.Include(p => p.UserRole);
            return PartialView("_UserPartial", await users.ToListAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

