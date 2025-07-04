using Mazad.Core.Domain.Users;
using Mazad.Core.Shared.Contexts;
using Mazad.Core.Shared.CQRS;
using Mazad.Core.Shared.Results;
using Microsoft.EntityFrameworkCore;

namespace Mazad.UseCases.Users.Read;

public class GetUsersListQuery : BaseQuery<GetUsersListQueryResponse>
{
    public bool? FilterByIsActiveEquals { get; set; }

    public int PageSize { get; set; } = 10;

    public int PageNumber { get; set; } = 1;

    public string? SearchTerm { get; set; } = string.Empty;
}

public class GetUsersListQueryResponse
{
    public required List<UserListDto> Users { get; set; }

    public required int TotalCount { get; set; }
}

public class GetUsersListQueryHandler
    : BaseQueryHandler<GetUsersListQuery, GetUsersListQueryResponse>
{
    private readonly MazadDbContext _context;

    public GetUsersListQueryHandler(MazadDbContext context)
    {
        _context = context;
    }

    public override async Task<Result<GetUsersListQueryResponse>> Handle(GetUsersListQuery query)
    {
        IQueryable<User> queryable = _context.Users.AsNoTracking();

        if (query.FilterByIsActiveEquals.HasValue)
        {
            queryable = queryable.Where(u => u.IsActive == query.FilterByIsActiveEquals.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            string searchTermLower = query.SearchTerm.ToLower();
            queryable = queryable.Where(u =>
                u.Name.ToLower().Contains(searchTermLower)
                || u.Phone.ToLower().Contains(searchTermLower)
            );
        }

        var totalCount = await queryable.CountAsync();

        var pagedUsers = await queryable
            .OrderByDescending(u => u.Id)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var userDtos = new List<UserListDto>();
        foreach (var user in pagedUsers)
        {
            userDtos.Add(MapUserToDto(user, query.Language));
        }

        return Result<GetUsersListQueryResponse>.Ok(
            new GetUsersListQueryResponse { Users = userDtos, TotalCount = totalCount },
            new LocalizedMessage
            {
                Arabic = "تم الحصول على قائمة المستخدمين بنجاح.",
                English = "User list retrieved successfully.",
            }
        );
    }

    private UserListDto MapUserToDto(User user, string language)
    {
        return new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            IsActive = user.IsActive,
        };
    }
}

public class UserListDto
{
    public required int Id { get; set; }

    public required string Name { get; set; } = string.Empty;

    public required string Phone { get; set; } = string.Empty;

    public required bool IsActive { get; set; }
}
