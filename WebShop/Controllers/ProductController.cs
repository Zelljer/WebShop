using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class ProductController : Controller
    {
		private readonly ApplicationContext _context;
		private readonly ILogger<HomeController> _logger;

		public ProductController(ILogger<HomeController> logger, ApplicationContext context)
		{
			_logger = logger;
			_context = context;

			/*if (!_context.Maufacturers.Any())
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
			}*/
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id != null)
			{
				Product product = new Product { Id = id.Value };
				_context.Entry(product).State = EntityState.Deleted;
				await _context.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return NotFound();
		}

		public async Task<IActionResult> AddProduct(int? id)
		{
			ViewBag.MaufacturerList = _context.Maufacturers.ToList();
			if (id != null)
			{
				Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
				if (product != null) return View(product);
			}
			return View(new Product());
		}
		[HttpPost]
		public async Task<IActionResult> AddProduct(Product product)
		{
			product.ProductMaufacturer.MaufacturerName = (from a in _context.Maufacturers where a.Id == product.ProductMaufacturer.Id select a.MaufacturerName).FirstOrDefault();
			_context.Products.Update(product);
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

		public ActionResult Index(int? maufacturer, string? name, SortState sortOrder = SortState.IdAsc)
		{
			IQueryable<Product> products = _context.Products.Include(p => p.ProductMaufacturer);
			if (maufacturer != null && maufacturer != 0)
				products = products.Where(p => p.ProductMaufacturer.Id == maufacturer);

			if (!string.IsNullOrEmpty(name))
				products = products.Where(p => p.ProductName!.Contains(name) ||
									p.ProductDescription!.Contains(name));

            ViewData["IdSort"] = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CostSort"] = sortOrder == SortState.CostAsc ? SortState.CostDesc : SortState.CostAsc;
            ViewData["UnitSort"] = sortOrder == SortState.UnitAsc ? SortState.UnitDesc : SortState.UnitAsc;
            ViewData["DescriptionSort"] = sortOrder == SortState.DescriptionAsc ? SortState.DescriptionDesc : SortState.DescriptionAsc;
            ViewData["MaufacturerSort"] = sortOrder == SortState.MaufacturerAsc ? SortState.MaufacturerDesc : SortState.MaufacturerAsc;

            products = sortOrder switch
            {
                SortState.IdDesc => products.OrderByDescending(s => s.Id),
                SortState.NameAsc => products.OrderBy(s => s.ProductName),
                SortState.NameDesc => products.OrderByDescending(s => s.ProductName),
                SortState.CostAsc => products.OrderBy(s => (double)s.ProductCost),
                SortState.CostDesc => products.OrderByDescending(s => (double)s.ProductCost),
                SortState.UnitAsc => products.OrderBy(s => s.ProductUnit),
                SortState.UnitDesc => products.OrderByDescending(s => s.ProductUnit),
                SortState.DescriptionAsc => products.OrderBy(s => s.ProductDescription),
                SortState.DescriptionDesc => products.OrderByDescending(s => s.ProductDescription),
                SortState.MaufacturerAsc => products.OrderBy(s => s.ProductMaufacturer!.Id),
                SortState.MaufacturerDesc => products.OrderByDescending(s => s.ProductMaufacturer!.Id),
                _ => products.OrderBy(s => s.Id),
            };

            List<Maufacturer> maufacturers = _context.Maufacturers.ToList();
			maufacturers.Insert(0, new Maufacturer { MaufacturerName = "Все", Id = 0 });

			ProductListViewModel productListViewModel = new ProductListViewModel
			{
				Products = products.ToList(),
				Manufactures = new SelectList(maufacturers, "Id", "MaufacturerName", maufacturer),
				Name = name
			};

			return View(productListViewModel);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}