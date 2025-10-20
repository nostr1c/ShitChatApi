using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.DTOs;

public class GenericResponse<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
    public bool HasErrors => Errors.Count > 0 || Status >= 400;
    public Dictionary<string, List<string>> Errors { get; set; } = new();
    public int Status { get; set; } = 200;
}

public static class ResponseHelper
{
    private static string FormatMessage(object message)
    {
        return message switch
        {
            null => "UnknownError",
            Enum e => e.ToString(),
            string s => s,
            _ => message.ToString() ?? "UnknownError"
        };
    }
    public static GenericResponse<T> Error<T>(object message, Dictionary<string, List<string>>? errors = null, int status = StatusCodes.Status400BadRequest)
    {
        return new GenericResponse<T>
        {
            Message = FormatMessage(message),
            Errors = errors ?? new Dictionary<string, List<string>>(),
            Status = status
        };
    }

    public static GenericResponse<T> Success<T>(object message, T data, int status = 200)
    {
        return new GenericResponse<T>
        {
            Message = FormatMessage(message),
            Data = data,
            Status = status
        };
    }

    public static GenericResponse<object> Success(object message, int status = StatusCodes.Status200OK)
    {
        return new GenericResponse<object>
        {
            Message = FormatMessage(message),
            Data = null,
            Status = status
        };
    }
}