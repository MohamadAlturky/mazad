﻿using Mazad.Core.Shared.Results;

namespace Mazad.Core.Shared.CQRS;

public abstract class BaseQuery<T>
{
    public required string Language { get; set; }
    public required int UserId { get; set; }
}

public abstract class BaseQueryHandler<TBaseQuery, T> where TBaseQuery : BaseQuery<T>
{
    public abstract Task<Result<T>> Handle(TBaseQuery query);
}

public abstract class BaseQueryApiRequest<T> where T : BaseQuery<T>
{
    public abstract T ToQuery(int userId, string language);
}
