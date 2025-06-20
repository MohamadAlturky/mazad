using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;

namespace Mazad.UseCases.Users.Profile;

public class GetUserProfileQuery : BaseQuery<UserProfile> { }

public class UserProfile
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
}

public class GetUserProfileQueryHandler : BaseQueryHandler<GetUserProfileQuery, UserProfile>
{
    private readonly MazadDbContext _context;

    public GetUserProfileQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<UserProfile>> Handle(GetUserProfileQuery query)
    {
        var user = await _context.Users.FindAsync(query.UserId);
        if (user == null)
        {
            return Result<UserProfile>.Fail(
                new LocalizedMessage { Arabic = "المستخدم غير موجود.", English = "User not found." }
            );
        }
        return Result<UserProfile>.Ok(
            new UserProfile
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
            },
            new LocalizedMessage
            {
                Arabic = "تم الحصول على الملف الشخصي بنجاح.",
                English = "User profile retrieved successfully.",
            }
        );
    }
}
