using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tasks4U.Model;
using Tasks4U.Models;

namespace Tasks4U.Data
{
    public class Tasks4UDbContext : IdentityDbContext<ApplicationUser>
    {
        public Tasks4UDbContext(DbContextOptions<Tasks4UDbContext> options):
            base(options)
        {

        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
