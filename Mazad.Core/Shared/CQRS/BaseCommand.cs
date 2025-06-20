using Mazad.Core.Shared.Results;

namespace Mazad.Core.Shared.CQRS;

public abstract class BaseCommand<T> : BaseCommand { }

public abstract class BaseCommandValidator<TBaseCommand, T>
    where TBaseCommand : BaseCommand<T>
{
    public abstract Result<T> Validate(TBaseCommand command);
}

public abstract class BaseCommandHandler<TBaseCommand, T>
    where TBaseCommand : BaseCommand<T>
{
    public abstract Task<Result<T>> Handle(TBaseCommand command);
}

public abstract class BaseCommand
{
    public required string Language { get; set; }
    public required int UserId { get; set; }
}

public abstract class BaseCommandHandler<TBaseCommand>
    where TBaseCommand : BaseCommand
{
    public abstract Task<Result> Handle(TBaseCommand command);
}

public abstract class BaseCommandValidator<TBaseCommand>
    where TBaseCommand : BaseCommand
{
    public abstract Result Validate(TBaseCommand command);
}

public abstract class BaseApiRequest<T>
    where T : BaseCommand
{
    public abstract T ToCommand(int userId, string language);
}
