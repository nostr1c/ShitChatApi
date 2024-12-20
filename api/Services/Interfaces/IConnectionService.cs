namespace api.Services.Interfaces
{
    public interface IConnectionService
    {
        Task<(bool Success, string Message)> CreateConnectionAsync(string userName, string friendId);
    }
}
