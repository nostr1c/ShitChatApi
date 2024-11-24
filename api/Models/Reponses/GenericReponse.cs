namespace api.Models.Reponses
{
    public class GenericResponse
    {
        public string Message { get; set; }

        public bool HasErrors => Errors.Count > 0;

        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
    }

    public class GenericResponse<T> : GenericResponse
    {
        public T Data { get; set; }
    }
}
