using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Follower : BaseEntity<int>
{
    public int FollowerId { get; set; }
    public int FollowedId { get; set; }

    public User TheFollower { get; set; } = new();
    public User TheFollowed { get; set; } = new();
}
