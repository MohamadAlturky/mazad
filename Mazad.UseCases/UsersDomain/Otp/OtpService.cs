using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mazad.UseCases.UsersDomain.Otp;

public interface IOtpService
{
    GenerateOtpResponse GenerateOtp(GenerateOtpRequest request);
    ValidateOtpResponse ValidateOtp(ValidateOtpRequest request);
    GetAllOtpsResponse GetAllOtps();
}

public class OtpService : IOtpService
{
    private static readonly ConcurrentDictionary<string, OtpRecord> _otpStore =
        new ConcurrentDictionary<string, OtpRecord>();

    private readonly OtpServiceSettings _settings;
    private readonly ILogger<OtpService> _logger;
    private readonly Random _random = new Random();

    public OtpService(IOptions<OtpServiceSettings> settings, ILogger<OtpService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public GenerateOtpResponse GenerateOtp(GenerateOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return new GenerateOtpResponse { Success = false, Message = "UserId cannot be empty." };
        }

        // Generate a random OTP code
        string otpCode = GenerateRandomOtp(_settings.OtpLength);
        DateTime expirationTime = DateTime.UtcNow.AddMinutes(_settings.OtpExpirationMinutes);

        var newOtpRecord = new OtpRecord
        {
            UserId = request.UserId,
            OtpCode = otpCode,
            ExpirationTime = expirationTime,
            IsUsed = false,
        };

        // Store or update the OTP for the user
        _otpStore[request.UserId] = newOtpRecord;
        _logger.LogInformation(
            "Generated OTP '{OtpCode}' for User '{UserId}'. Expires at {ExpirationTime} UTC.",
            otpCode,
            request.UserId,
            expirationTime
        );

        return new GenerateOtpResponse
        {
            Success = true,
            OtpCode = otpCode,
            ExpirationTime = expirationTime,
            Message = "OTP generated successfully.",
        };
    }

    public ValidateOtpResponse ValidateOtp(ValidateOtpRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Otp))
        {
            return new ValidateOtpResponse
            {
                IsValid = false,
                Message = "UserId and Otp cannot be empty.",
            };
        }

        if (!_otpStore.TryGetValue(request.UserId, out OtpRecord? storedOtp))
        {
            _logger.LogWarning(
                "Validation failed for User '{UserId}': No OTP found.",
                request.UserId
            );
            return new ValidateOtpResponse
            {
                IsValid = false,
                Message = "OTP not found for this user.",
            };
        }

        if (storedOtp.IsUsed)
        {
            _logger.LogWarning(
                "Validation failed for User '{UserId}': OTP '{OtpCode}' has already been used.",
                request.UserId,
                storedOtp.OtpCode
            );
            return new ValidateOtpResponse
            {
                IsValid = false,
                Message = "OTP has already been used.",
            };
        }

        if (storedOtp.ExpirationTime < DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Validation failed for User '{UserId}': OTP '{OtpCode}' has expired.",
                request.UserId,
                storedOtp.OtpCode
            );
            // Optionally remove expired OTPs from store
            _otpStore.TryRemove(request.UserId, out _);
            return new ValidateOtpResponse { IsValid = false, Message = "OTP has expired." };
        }

        if (storedOtp.OtpCode != request.Otp)
        {
            _logger.LogWarning(
                "Validation failed for User '{UserId}': Invalid OTP '{ProvidedOtp}'. Expected '{ExpectedOtp}'.",
                request.UserId,
                request.Otp,
                storedOtp.OtpCode
            );
            return new ValidateOtpResponse { IsValid = false, Message = "Invalid OTP." };
        }

        // Mark OTP as used to prevent replay attacks
        storedOtp.IsUsed = true;
        _logger.LogInformation(
            "OTP '{OtpCode}' successfully validated and marked as used for User '{UserId}'.",
            storedOtp.OtpCode,
            request.UserId
        );
        return new ValidateOtpResponse { IsValid = true, Message = "OTP validated successfully." };
    }

    public GetAllOtpsResponse GetAllOtps()
    {
        var otps = _otpStore.Values.ToList();
        _logger.LogInformation("Retrieved all {Count} OTP records.", otps.Count);
        return new GetAllOtpsResponse
        {
            Otps = otps,
            Success = true,
            Message = "All OTPs retrieved successfully.",
        };
    }

    private string GenerateRandomOtp(int length)
    {
        const string chars = "0123456789";
        char[] otpChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            otpChars[i] = chars[_random.Next(chars.Length)];
        }
        return new string(otpChars);
    }
}

// You will also need to define the response class for GetAllOtps
public class GetAllOtpsResponse
{
    public List<OtpRecord> Otps { get; set; } = new List<OtpRecord>();
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
