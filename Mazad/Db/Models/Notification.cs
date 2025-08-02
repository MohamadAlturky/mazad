using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Notification : BaseEntity<long>
{
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string BodyAr { get; set; } = string.Empty;
    public string BodyEn { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? ActionUrl { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string NotificationType { get; set; }
}
