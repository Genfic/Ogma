#nullable enable

namespace Ogma3.Infrastructure.MediatR
{
    public class Result<T>
    {
        public T? Data { get; init; }
        public ResultStatus Status { get; init; }
        public string? Error { get; init; }
    }

    public enum ResultStatus
    {
        Ok,
        Conflict,
        Unauthorized,
        NotFound,
        ServerError
    }

    public static class Result
    {
        public static Result<T?> Ok<T>(T data) => new()
        {
            Data = data,
            Status = ResultStatus.Ok,
        };

        public static Result<T> Conflict<T>(string error = "Conflict") => new()
        {
            Status = ResultStatus.Conflict,
            Error = error
        };
        
        public static Result<T> Unauthorized<T>(string error = "Unauthorized") => new()
        {
            Status = ResultStatus.Unauthorized,
            Error = error
        };

        public static Result<T> NotFound<T>(string error = "Not Found") => new()
        {
            Status = ResultStatus.NotFound,
            Error = error
        };

        public static Result<T> ServerError<T>(string error = "Internal Server Error") => new()
        {
            Status = ResultStatus.ServerError,
            Error = error
        };
    }
}