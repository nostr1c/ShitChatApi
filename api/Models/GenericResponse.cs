namespace api.Models
{
    public class GenericResponse<T>
    {
        public string? Message { get; set; }

        public bool HasErrors => Errors.Count > 0;

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        public T? Data { get; set; }
    }
}