namespace OfficinaGestionale.Api.Services;

public class ServiceResult
{
    public bool Success { get; }
    public string? ErrorMessage { get; }
    public int StatusCode { get; }

    protected ServiceResult(bool success, string? errorMessage, int statusCode)
    {
        Success = success;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    // risultati standard usati dai service
    public static ServiceResult Ok() => new(true, null, 200);
    public static ServiceResult Fail(string message) => new(false, message, 400);
    public static ServiceResult NotFound(string message) => new(false, message, 404);
    public static ServiceResult Conflict(string message) => new(false, message, 409);
    public static ServiceResult ServerError(string message) => new(false, message, 500);
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; }

    private ServiceResult(bool success, T? data, string? errorMessage, int statusCode)
        : base(success, errorMessage, statusCode)
    {
        Data = data;
    }

    // versione con dato di ritorno
    public static ServiceResult<T> Ok(T data) => new(true, data, null, 200);
    public static new ServiceResult<T> Fail(string message) => new(false, default, message, 400);
    public static new ServiceResult<T> NotFound(string message) => new(false, default, message, 404);
    public static new ServiceResult<T> Conflict(string message) => new(false, default, message, 409);
    public static new ServiceResult<T> ServerError(string message) => new(false, default, message, 500);
}