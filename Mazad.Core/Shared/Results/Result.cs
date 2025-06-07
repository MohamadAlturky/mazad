namespace Mazad.Core.Shared.Results;

public class Result
{
    public bool Success { get; set; }
    public LocalizedMessage Message { get; set; }
    public Exception? Exception { get; set; }
    public static Result Ok(LocalizedMessage message)
    {
        return new Result
        {
            Success = true,
            Message = message,
        };
    }
    public static Result Fail(LocalizedMessage message, Exception? exception = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Exception = exception
        };
    }
}


public class LocalizedMessage
{
    public string Arabic { get; set; }
    public string English { get; set; }
}

public class Result<T> : Result
{
    public T Data { get; set; }
    public static Result<T> Ok(T data, LocalizedMessage message)
    {
        return new Result<T>
        {
            Success = true,
            Message = message,
            Data = data,
        };
    }
    public static Result<T> Fail(LocalizedMessage message, Exception? exception = null)
    {
       return new Result<T>
       {
           Success = false,
           Message = message,
           Exception = exception,
       };
    }
}



