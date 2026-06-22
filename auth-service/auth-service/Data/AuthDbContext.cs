using auth_service.Models;
using Microsoft.EntityFrameworkCore;

namespace auth_service.Data
{
    public class AuthDbContext:DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
    }
}
