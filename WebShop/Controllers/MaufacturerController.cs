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

		public async Task<IActionResult> Index()
		{
			return View(await _context.Maufacturers.ToListAsync());
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create([Bind("MaufacturerName,Id")] Maufacturer maufacturer)
		{
			if (ModelState.IsValid)
			{
				_context.Add(maufacturer);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(maufacturer);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var maufacturer = await _context.Maufacturers.FindAsync(id);
			if (maufacturer == null)
			{
				return NotFound();
			}
			return View(maufacturer);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(int id, [Bind("MaufacturerName,Id")] Maufacturer maufacturer)
		{
			if (id != maufacturer.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(maufacturer);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!RoleExists(maufacturer.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(maufacturer);
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
