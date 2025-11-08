using Elastic.Clients.Elasticsearch;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShitChat.Api.Authorization;
using ShitChat.Api.Filters;
using ShitChat.Application.Auth.Requests;
using ShitChat.Application.Auth.Services;
using ShitChat.Application.Caching.Services;
using ShitChat.Application.Connections.Services;
using ShitChat.Application.Groups.Requests;
using ShitChat.Application.Groups.Services;
using ShitChat.Application.Invites.Requests;
using ShitChat.Application.Invites.Services;
using ShitChat.Application.Roles.Services;
using ShitChat.Application.Translations.Requests;
using ShitChat.Application.Translations.Services;
using ShitChat.Application.Uploads.Services;
using ShitChat.Application.Users.Requests;
using ShitChat.Application.Users.Services;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using StackExchange.Redis;
using System.Text;

namespace ShitChat.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwtKey = config["Jwt:Key"];
        var jwtIssuer = config["Jwt:Issuer"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                ClockSkew = TimeSpan.Zero,
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var path = ctx.HttpContext.Request.Path;

                    if (ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                    {
                        ctx.Token = accessToken;
                        Console.WriteLine($"[JWT] Token found in cookie for {path}");
                    }

                    return Task.CompletedTask;
                },
            };
        });

        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var redisPassword = config["REDIS_PASSWORD"];
        var redisConnectionString = $"redis:6379,password={redisPassword},abortConnect=false";
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString)
        );

        services.AddSingleton<ICacheService, RedisCacheService>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var dbServer = config["DB_SERVER"];
        var dbDatabase = config["DB_DATABASE"];
        var dbUser = config["DB_USER"];
        var dbPassword = config["DB_PASSWORD"];
        var connectionString = $"Host=postgres;Port=5432;Database={dbDatabase};Username={dbUser};Password={dbPassword}";

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/Keys"));

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, AppRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        });

        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("GroupMember", policy =>
                policy.Requirements.Add(new GroupMembershipRequirement()));

            options.AddPolicy("CanKick", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("kick_user")));

            options.AddPolicy("CanBan", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("ban_user")));

            options.AddPolicy("CanManageUserRoles", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("manage_user_roles")));

            options.AddPolicy("CanManageServerRoles", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("manage_server_roles")));

            options.AddPolicy("CanManageInvites", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("manage_invites")));

            options.AddPolicy("CanManageServer", policy =>
                policy.Requirements.Add(new GroupPermissionRequirement("manage_server")));
        });

        services.AddScoped<IAuthorizationHandler, GroupMembershipHandler>();
        services.AddScoped<IAuthorizationHandler, GroupPermissionHandler>();

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IInviteService, InviteService>();
        services.AddSingleton<IPresenceService, PresenceService>();
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<ITranslationService, TranslationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRolesService, UserRolesService>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        services.AddScoped<IValidator<LoginUserRequest>, RequestLoginValidator>();
        services.AddScoped<IValidator<UpdateAvatarRequest>, UpdateAvatarRequestValidator>();
        services.AddScoped<IValidator<CreateGroupRequest>, CreateGroupRequestValidator>();
        services.AddScoped<IValidator<SendMessageRequest>, SendMessageRequestValidator>();
        services.AddScoped<IValidator<CreateInviteRequest>, CreateInviteRequestValidator>();
        services.AddScoped<IValidator<CreateGroupRoleRequest>, CreateGroupRoleRequestValidator>();
        services.AddScoped<IValidator<EditGroupRoleRequest>, EditGroupRoleRequestValidator>();
        services.AddScoped<IValidator<MarkAsReadRequest>, MarkAsReadRequestValidator>();
        services.AddScoped<IValidator<BanUserRequest>, BanUserRequestValidator>();
        services.AddScoped<IValidator<EditGroupRequest>, EditGroupRequestValidator>();
        services.AddScoped<IValidator<CreateTranslationRequest>, CreateTranslationRequestValidator>();
        services.AddScoped<IValidator<UpdateTranslationRequest>, UpdateTranslationRequestValidator>();

        return services;
    }

    public static IServiceCollection AddControllersAndFilter(this IServiceCollection services)
    {
        services.AddScoped<ValidationFilter>();
        services.AddControllers(options => options.Filters.AddService<ValidationFilter>());
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddSignalR();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins(
                        "http://filipsiri.se",
                        "https://filipsiri.se",
                        "http://localhost:3000",
                        "http://localhost:3001",
                        "http://localhost:3100",
                        "http://admin.filipsiri.se",
                        "https://admin.filipsiri.se"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Auth-Status");
                });
        });

        return services;
    }

    public static IServiceCollection AddOpenApiDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();

        return services;
    }

    public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration config)
    {
        var elasticUrl = config["ELASTIC_URL"] ?? "http://localhost:9200";
        var settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
            .DefaultIndex("users");

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);

        return services;
    }
}
