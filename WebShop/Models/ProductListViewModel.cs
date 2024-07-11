using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebShop.Models
{
	public class ProductListViewModel
	{
		public List<Product> Products { get; set; } = new List<Product>();
		public SelectList Manufactures { get; set; } = new SelectList(new List<Maufacturer>(), "Id", "MaufacturerName");
		public string? Name { get; set; }
	}
}
