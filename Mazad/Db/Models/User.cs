using System.ComponentModel.DataAnnotations;
using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class User : BaseEntity<int>
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? ProfilePhotoUrl { get; set; } = "placeholder.png";

    public UserType UserType { get; set; }

    [MaxLength(2000)]
    public string? Password { get; set; }

    [MaxLength(500)]
    public string? Email { get; set; }
}

public enum UserType
{
    Admin = 1,
    User = 2,
}
