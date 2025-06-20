using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.VerifyOtp;

public class VerifyOtpApiRequest : BaseApiRequest<VerifyOtpCommand>
{
    public string UserIdToVerify { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;

    public override VerifyOtpCommand ToCommand(int userId, string language)
    {
        return new VerifyOtpCommand
        {
            UserId = userId,
            UserIdToVerify = UserIdToVerify,
            Otp = Otp,
            Language = language, // Inherit language from base request if needed
        };
    }
}
