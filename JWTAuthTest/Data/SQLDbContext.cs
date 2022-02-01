using JWTAuthTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthTest.Data
{
    public class SQLDbContext : DbContext
    {
        public SQLDbContext(DbContextOptions opt): base(opt) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
    }
}
