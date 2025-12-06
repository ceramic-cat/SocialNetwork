using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Entity.Models;

namespace SocialNetwork.Repository.Services
{
     public interface ITimelineService
    {
        Task<List<Post>> GetPostsByUserIdAsync(Guid userId);
    }
}
