using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationContext _context;
        public RoleController(ApplicationContext context) => _context = context;
       
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetData()
        {
            var data = await _context.Roles.ToListAsync();
            return PartialView("_RolePartial", data);
        }

        public async Task<IActionResult> AddRole(int? id)
        {
            if (id != null)
            {
                Role? role = await _context.Roles.FirstOrDefaultAsync(p => p.Id == id);
                if (role != null) return View(role);
            }
            return View(new Role());
        }

		[HttpPost]
		public async Task<IActionResult> AddRole(Role role)
		{
			_context.Roles.Update(role);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

        public async Task<IActionResult> Delete(int id, bool deleteBinds = false)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
                return NotFound();

            var usersWithRole = await _context.Users.Where(u => u.UserRole.Id == id).ToListAsync();
            _context.Roles.Remove(role);

            if (usersWithRole.Any())
            {
                if (deleteBinds)
                {
                    _context.Users.RemoveRange(usersWithRole);
                    await _context.SaveChangesAsync();
                    return Json(new { result = "deleted" });
                }
                else
                {
                    return Json(new { result = "haveBinds" });
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { result = "deleted" });
        }
    }
}
