using Mazad.Core.Shared.Entities;

namespace Mazad.Core.Domain.Users;

public class User : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
