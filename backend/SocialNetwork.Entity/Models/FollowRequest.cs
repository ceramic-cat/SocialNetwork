using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Entity.Models;

public class FollowRequest
{
    public Guid FollowerId { get; set; }
    public Guid FolloweeId { get; set; }
}
