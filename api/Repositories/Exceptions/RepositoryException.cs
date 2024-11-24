namespace api.Repositories.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception exception) : base(message, exception) { }
    }
}
