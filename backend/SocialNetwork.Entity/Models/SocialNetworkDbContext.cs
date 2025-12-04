using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Entity.Models
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Follow> UserFollows { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}