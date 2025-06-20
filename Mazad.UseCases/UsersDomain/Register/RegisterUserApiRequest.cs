using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.Register;

public class RegisterUserApiRequest : BaseApiRequest<RegisterUserCommand>
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public override RegisterUserCommand ToCommand(int userId, string language)
    {
        return new RegisterUserCommand
        {
            Name = Name,
            Phone = Phone,
            UserId = userId,
            Language = language,
        };
    }
}
