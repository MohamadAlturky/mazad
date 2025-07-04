namespace Mazad.UseCases.UsersDomain.Otp;

public class OtpServiceSettings
{
    public int OtpLength { get; set; } = 6;
    public int OtpExpirationMinutes { get; set; } = 5;
}
