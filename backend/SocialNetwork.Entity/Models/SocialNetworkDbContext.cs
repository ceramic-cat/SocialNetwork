using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Entity.Models
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}