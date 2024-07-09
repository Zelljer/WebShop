using Microsoft.EntityFrameworkCore;

namespace WebShop.Models
{
    public class ApplicationContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\sqlite\task.db");
        }

        public ApplicationContext() => Database.EnsureCreated();
        public ApplicationContext(DbContextOptions options) : base(options) { Database.EnsureCreated(); }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Maufacturer> Maufacturers { get; set; } 
        public DbSet<Product> Products { get; set; }

        public static ApplicationContext New => new ApplicationContext();

    }
}