using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.Register;

public class RegisterUserCommand : BaseCommand<RegisterUserResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class RegisterUserResponse
{
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
}