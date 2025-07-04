using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Favorite : BaseEntity<int>
{
    public int UserId { get; set; }
    public int OfferId { get; set; }

    public User User { get; set; } = new();
    public Offer Offer { get; set; } = new();
}
