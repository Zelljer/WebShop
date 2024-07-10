namespace WebShop.Models
{
    public class User : BaseInformation
    {
        public string? UserSurname { get; set; }
        public string? UserName { get; set; }
        public string? UserPantonymic { get; set; }
        public string? UserLogin{ get; set; }
        public string? UserPassword { get; set; } 
        public int UserPhoneNumber { get; set; }
        public decimal UserMoney { get; set; }
        public virtual Role? UserRole { get; set; }
    }
}
