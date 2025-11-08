using ShitChat.Api.Extensions;

namespace ShitChat.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;

        // Add JWT configuration
        builder.Services
            .AddJwtAuth(config)
            .AddRedis(config)
            .AddDatabase(config)
            .AddIdentity()
            .AddAppAuthorization()
            .AddAppServices()
            .AddValidators()
            .AddControllersAndFilter()
            .AddElasticSearch(config)
            .AddCorsPolicy()
            .AddOpenApiDocs();


        var app = builder.Build();
        app.ConfigureApp();
        app.Run();
    }
}
