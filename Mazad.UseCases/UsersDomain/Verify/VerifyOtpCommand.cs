using Mazad.Core.Shared.CQRS;

namespace Mazad.UseCases.Users.VerifyOtp;

public class VerifyOtpCommand : BaseCommand<VerifyOtpResponse>
{
    public string UserIdToVerify { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public class VerifyOtpResponse
{
    public required string Token { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
}
