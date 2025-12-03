using Microsoft.EntityFrameworkCore;
using SocialNetwork.Entity.Models.Follow;

namespace SocialNetwork.Entity.Models
{
    public class SocialNetworkDbContext : DbContext
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<UserFollows> UserFollows { get; set; }
    }
}