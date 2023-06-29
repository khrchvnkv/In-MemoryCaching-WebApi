using InMemoryCaching_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCaching_WebAPI.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Driver> Drivers { get; set; }
        
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
    }
}