namespace WebShop.Models
{
	public class ProductListViewModel
	{
        public List<Maufacturer> FiltrList { get; set; } = new List<Maufacturer>();

        public List<string> SortList { get; set; } = new List<string>()
        {
            "Без сортировки",
            "Id по возростанию", "Id по убыванию",
            "Название по возростанию", "Название по убыванию",
            "Цена по возростанию", "Цена по убыванию",
            "Производитель по возростанию", "Производитель по убыванию",
        };
    }
}
