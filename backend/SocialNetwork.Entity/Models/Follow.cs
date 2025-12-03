using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialNetwork.Entity.Models;

[Table("userFollows")]
public class Follow
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Column("followerId")]
    public required Guid FollowerId { get; set; }
    [Column("followeeId")]
    public required Guid FolloweeId { get; set; }
    [Column("created")]
    public  DateTime Created { get; set; } = DateTime.UtcNow;


}
