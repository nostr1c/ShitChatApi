using ShitChat.Api.Hubs;
using ShitChat.Api.Authorization;
using ShitChat.Application.Services;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Application.Requests;
using ShitChat.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                    if (!string.IsNullOrEmpty(accessToken))
                        ctx.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });

        // Entity framework
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Identity
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

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

        // Validators
        builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
        builder.Services.AddScoped<IValidator<LoginUserRequest>, RequestLoginValidator>();
        builder.Services.AddScoped<IValidator<UpdateAvatarRequest>, UpdateAvatarRequestValidator>();
        builder.Services.AddScoped<IValidator<CreateGroupRequest>, CreateGroupRequestValidator>();
        builder.Services.AddScoped<IValidator<SendMessageRequest>, SendMessageRequestValidator>();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddSignalR();
        builder.Services.AddControllers();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ShitChatApi",
                Version = "v1"
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter jwt token."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:4173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chatHub");

        app.Run();
    }
}
