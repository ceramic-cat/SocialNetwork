using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialNetwork.Entity.Models;

[Table("userFollows")]
public class UserFollows
{

    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("followerId")]
    public Guid FollowerId { get; set; }
    [Column("followeeId")]
    public Guid FolloweeID { get; set; }
    [Column("created")]
    public string Created { get; set; } = default!;


}
