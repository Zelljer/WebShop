using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebShop.Models
{
    public class UserListViewModel
    {
        public List<Role> FiltrList { get; set; } = new List<Role>();
        public string? SearchText{ get; set; }

    }
}
