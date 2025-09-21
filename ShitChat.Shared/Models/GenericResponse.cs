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
    public static GenericResponse<T> Error<T>(string message, Dictionary<string, List<string>>? errors = null, int status = 200)
    {
        return new GenericResponse<T>
        {
            Message = message,
            Errors = errors ?? new Dictionary<string, List<string>>(),
            Status = status
        };
    }
}