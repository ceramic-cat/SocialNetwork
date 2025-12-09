using System;
using System.Collections.Generic;
using System.Text;

namespace SocialNetwork.Entity.Models;

public class FollowerUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
}