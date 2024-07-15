using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Controllers
{
	public class MaufacturerController : Controller
	{
		private readonly ApplicationContext _context;

		public MaufacturerController(ApplicationContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> GetData()
		{
			var data = await _context.Maufacturers.ToListAsync();
			return PartialView("_MaufacturerPartial", data);
		}

		public async Task<IActionResult> AddMaufacturer(int? id)
		{
			if (id != null)
			{
				Maufacturer? maufacturer = await _context.Maufacturers.FirstOrDefaultAsync(p => p.Id == id);
				if (maufacturer != null) return View(maufacturer);
			}
			return View(new Maufacturer());
		}
		[HttpPost]
		public async Task<IActionResult> AddMaufacturer(Maufacturer maufacturer)
		{
			_context.Maufacturers.Update(maufacturer);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Delete(int id, bool deleteBinds = false)
        {
            var maufacturer = await _context.Maufacturers.FindAsync(id);

            if (maufacturer == null)
                return NotFound();

            var productsOfMaufacturer = await _context.Products.Where(u => u.ProductMaufacturer.Id == id).ToListAsync();
            _context.Maufacturers.Remove(maufacturer);

            if (productsOfMaufacturer.Any())
            {
                if (deleteBinds)
                {
                    _context.Products.RemoveRange(productsOfMaufacturer);
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

        private bool RoleExists(int id)
		{
			return _context.Roles.Any(e => e.Id == id);
		}
	}
}
