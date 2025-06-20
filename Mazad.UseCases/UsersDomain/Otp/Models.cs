namespace Mazad.UseCases.UsersDomain.Otp;

public class OtpRecord
{
    public string UserId { get; set; }
    public string OtpCode { get; set; }
    public DateTime ExpirationTime { get; set; }
    public bool IsUsed { get; set; }
}

// --- 3. GenerateOtpRequest.cs ---
public class GenerateOtpRequest
{
    public string UserId { get; set; }
}

// --- 4. GenerateOtpResponse.cs ---
public class GenerateOtpResponse
{
    public string OtpCode { get; set; }
    public DateTime ExpirationTime { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}

// --- 5. ValidateOtpRequest.cs ---
public class ValidateOtpRequest
{
    public string UserId { get; set; }
    public string Otp { get; set; }
}

// --- 6. ValidateOtpResponse.cs ---
public class ValidateOtpResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
}
