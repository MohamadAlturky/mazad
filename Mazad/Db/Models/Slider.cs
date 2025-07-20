using System.ComponentModel.DataAnnotations;
using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Slider : BaseEntity<int>
{
    [MaxLength(100)]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(100)]
    public string NameAr { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}
