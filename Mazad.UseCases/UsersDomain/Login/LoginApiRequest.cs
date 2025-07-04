using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.Login;

public class LoginApiRequest : BaseApiRequest<LoginCommand>
{
    public string Phone { get; set; } = string.Empty;

    public override LoginCommand ToCommand(int userId, string language)
    {
        return new LoginCommand
        {
            Phone = Phone,
            Language = language,
            UserId = userId,
        };
    }
}
