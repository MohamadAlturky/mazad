using Mazad.Core.Domain.Users;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Mazad.UseCases.UsersDomain.Otp;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Users.Register;

public class RegisterUserCommandHandler
    : BaseCommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly MazadDbContext _context;
    private readonly RegisterUserCommandValidator _validator;
    private readonly IOtpService _otpService;

    public RegisterUserCommandHandler(MazadDbContext context, IOtpService otpService)
    {
        _context = context;
        _validator = new RegisterUserCommandValidator();
        _otpService = otpService;
    }

    public override async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand command)
    {
        // 1. Validate the command
        var validationResult = _validator.Validate(command);
        if (!validationResult.Success)
        {
            return validationResult;
        }

        // 2. Check for duplicate phone numbers
        var userExists = await _context.Users.AnyAsync(u => u.Phone == command.Phone);

        if (userExists)
        {
            return Result<RegisterUserResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "يوجد مستخدم مسجل بنفس رقم الهاتف بالفعل.",
                    English = "A user with the same phone number already exists.",
                }
            );
        }

        // 4. Create a new User entity
        var newUser = new User
        {
            Name = command.Name,
            Phone = command.Phone,
            IsActive = true,
        };

        // 5. Add the new user to the database context
        _context.Users.Add(newUser);

        // 6. Save changes to the database
        try
        {
            await _context.SaveChangesAsync();
            _otpService.GenerateOtp(new GenerateOtpRequest { UserId = newUser.Id.ToString() });
        }
        catch (DbUpdateException ex)
        {
            // Log the exception
            Console.WriteLine($"Error registering user: {ex.Message}");
            return Result<RegisterUserResponse>.Fail(
                new LocalizedMessage
                {
                    Arabic = "حدث خطأ أثناء تسجيل المستخدم.",
                    English = "An error occurred while registering the user.",
                },
                ex
            );
        }

        // 7. Return a success result
        return Result<RegisterUserResponse>.Ok(
            new RegisterUserResponse
            {
                UserId = newUser.Id,
                Name = newUser.Name,
                Phone = newUser.Phone,
            },
            new LocalizedMessage
            {
                Arabic = "تم تسجيل المستخدم بنجاح.",
                English = "User registered successfully.",
            }
        );
    }
}
