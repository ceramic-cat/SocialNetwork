using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SocialNetwork.Entity.Models;
using SocialNetwork.Repository;
using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Repository.Repositories;
using SocialNetwork.Repository.Services;
using System.Text;

namespace SocialNetwork.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Social Network API",
                        Version = "v1",
                        Description = "API for the Social Network application",
                        Contact = new OpenApiContact
                        {
                            Name = "Your Name",
                            Email = "your@email.com"
                        }
                    };
                    return Task.CompletedTask;
                });

                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                options.AddOperationTransformer<AuthorizeOperationTransformer>();
            });
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFollowsService, FollowService>();
            builder.Services.AddScoped<IFollowRepository, FollowRepository>();
            builder.Services.AddScoped<ITimelineService, TimelineService>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<IUserRepository, SqliteUserRepository>();
            builder.Services.AddScoped<IDirectMessageRepository>(sp =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new SqliteDirectMessageRepository(connectionString!);
            });
            builder.Services.AddScoped<IDirectMessageService, DirectMessageService>();

            builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            var contentRoot = builder.Environment.ContentRootPath;

            var dbPath = Path.GetFullPath(Path.Combine(contentRoot, "../socialnetwork.db"));
            var seedPath = Path.Combine(contentRoot, "../socialnetwork_seed.db");

            if (!File.Exists(dbPath))
            {
                if (!File.Exists(seedPath))
                {
                    throw new FileNotFoundException("Seed database not found.", seedPath);
                }

                File.Copy(seedPath, dbPath);
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var jwtSettings = builder.Configuration.GetSection("Jwt");

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
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? string.Empty))
                };
            });

            builder.Services.AddAuthorization();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}