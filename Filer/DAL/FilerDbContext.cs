using Filer.DAL.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Filer.DAL
{
    public class FilerDbContext : DbContext
    {
        public FilerDbContext(DbContextOptions<FilerDbContext> options) : base(options)
        { 
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DbModels.File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }

    }
}
