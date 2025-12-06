using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.Entity.Models
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<DirectMessage> DirectMessages { get; set; }
    }
}