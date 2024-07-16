namespace WebShop.Models
{
    public class Product : BaseInformation
    {
        public string? ProductName { get; set; }
        public decimal ProductCost { get; set; }
        public string? ProductUnit { get; set; }
        public string? ProductDescription { get; set; }
        public virtual Maufacturer ProductMaufacturer { get; set; }
    }
}
