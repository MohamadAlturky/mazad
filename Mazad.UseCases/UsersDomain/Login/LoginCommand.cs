using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.Login;

public class LoginCommand : BaseCommand<LoginResponse>
{
    public string Phone { get; set; } = string.Empty;
}

public class LoginResponse
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required int UserId { get; set; }
}
