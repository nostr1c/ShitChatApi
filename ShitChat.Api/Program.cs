using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using ShitChat.Api.Authorization;
using ShitChat.Api.Filters;
using ShitChat.Api.Hubs;
using ShitChat.Api.Middleware;
using ShitChat.Application.Interfaces;
using ShitChat.Application.Requests;
using ShitChat.Application.Services;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using StackExchange.Redis;
using System.Text;

namespace ShitChat.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add JWT configuration
        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];

        builder.Services.AddAuthentication(options =>
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
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

        // Redis
        var redisPassword = builder.Configuration["REDIS_PASSWORD"];
        var redisConnectionString = $"redis:6379,password={redisPassword},abortConnect=false";
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString)
        );

        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        // Entity framework
        var dbServer = builder.Configuration["DB_SERVER"];
        var dbDatabase = builder.Configuration["DB_DATABASE"];
        var dbUser = builder.Configuration["DB_USER"];
        var dbPassword = builder.Configuration["DB_PASSWORD"];

        var connectionString = $"Host=postgres;Port=5432;Database={dbDatabase};Username={dbUser};Password={dbPassword}";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var keysFolder = "/Keys";

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysFolder));

        // Identity
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        });

        // Authorization
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("GroupMember", policy =>
                policy.Requirements.Add(new GroupMembershipRequirement()));
        });

        builder.Services.AddScoped<IAuthorizationHandler, GroupMembershipHandler>();

        // Services
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IConnectionService, ConnectionService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IGroupService, GroupService>();
        builder.Services.AddScoped<IInviteService, InviteService>();
        builder.Services.AddSingleton<IPresenceService, PresenceService>();

        // Validators
        builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        builder.Services.AddScoped<IValidator<LoginUserRequest>, RequestLoginValidator>();
        builder.Services.AddScoped<IValidator<UpdateAvatarRequest>, UpdateAvatarRequestValidator>();
        builder.Services.AddScoped<IValidator<CreateGroupRequest>, CreateGroupRequestValidator>();
        builder.Services.AddScoped<IValidator<SendMessageRequest>, SendMessageRequestValidator>();
        builder.Services.AddScoped<IValidator<CreateInviteRequest>, CreateInviteRequestValidator>();
        builder.Services.AddScoped<IValidator<CreateGroupRoleRequest>, CreateGroupRoleRequestValidator>();
        builder.Services.AddScoped<IValidator<EditGroupRoleRequest>, EditGroupRoleRequestValidator>();

        builder.Services.AddScoped<ValidationFilter>();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddSignalR();

        builder.Services.AddControllers(options =>
        {
            options.Filters.AddService<ValidationFilter>();
        });

        builder.Services.AddControllers();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // ScalarApi
        builder.Services.AddOpenApi();

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins(
                        "http://filipsiri.se",
                        "https://filipsiri.se",
                        "http://localhost:3000",
                        "https://localui.test"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Auth-Status");
                });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        //app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionMiddleware>();

        app.MapHub<ChatHub>("/chatHub").RequireAuthorization(new AuthorizeAttribute
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        });

        app.MapControllers();

        app.Run();
    }
}
