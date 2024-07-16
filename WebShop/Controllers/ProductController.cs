using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShop.Models;

namespace WebShop.Controllers
{
    public class ProductController : Controller
    {
		private readonly ApplicationContext _context;
		public ProductController(ApplicationContext context) =>_context = context;

		[HttpPost]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id != null)
			{
				Product product = new Product { Id = id.Value };
				_context.Entry(product).State = EntityState.Deleted;
				await _context.SaveChangesAsync();
                return Json(new { result = true });
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
			product.ProductMaufacturer.MaufacturerName = (from a in _context.Maufacturers 
                                                          where a.Id == product.ProductMaufacturer.Id 
                                                          select a.MaufacturerName).FirstOrDefault();
			_context.Products.Update(product);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

        public async Task<IActionResult> DataSettings(int sortType, int filtrType, string searchText)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.ProductMaufacturer);

            if (filtrType != 0)
                products = products.Where(p => p.ProductMaufacturer.Id == filtrType);

            products = sortType switch
            {
                1 => products.OrderBy(x => x.Id),
                2 => products.OrderByDescending(x => x.Id),
                3 => products.OrderBy(x => x.ProductName),
                4 => products.OrderByDescending(x => x.ProductName),
                5 => products.OrderBy(x => (double)x.ProductCost),
                6 => products.OrderByDescending(x => (double)x.ProductCost),
                7 => products.OrderBy(x => (double)x.ProductMaufacturer.Id),
                8 => products.OrderByDescending(x => (double)x.ProductMaufacturer.Id),
                _ => products
            };

            if (!string.IsNullOrEmpty(searchText))
                products = products.Where(s => s.ProductName.Contains(searchText) ||
                                           s.ProductDescription.Contains(searchText));

            return PartialView("_ProductPartial", await products.ToListAsync());
        }

        public IActionResult Index()
		{
            List<Maufacturer> maufacturers = _context.Maufacturers.ToList();
			maufacturers.Insert(0, new Maufacturer { MaufacturerName = "Все", Id = 0 });

			ProductListViewModel productListViewModel = new ProductListViewModel
			{
                FiltrList= maufacturers
            };

			return View(productListViewModel);
		}

        public async Task<IActionResult> GetData()
        {
            IQueryable<Product> products = _context.Products.Include(p => p.ProductMaufacturer);
            return PartialView("_ProductPartial", await products.ToListAsync());
        }
	}
}