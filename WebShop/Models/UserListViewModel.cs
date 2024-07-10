using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebShop.Models
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public SelectList Roles { get; set; } = new SelectList(new List<Role>(), "Id", "RoleName");
        public string? Name { get; set; }
    }
}
