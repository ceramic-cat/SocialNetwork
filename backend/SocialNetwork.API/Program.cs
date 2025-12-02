using SocialNetwork.Repository.Interfaces;
using SocialNetwork.Repository.Services;
using SocialNetwork.Repository.Repositories;
using SocialNetwork.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddControllers();
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen();

      builder.Services.AddScoped<IPostService, PostService>();
      builder.Services.AddScoped<IPostRepository, InMemoryPostRepository>();
      builder.Services.AddScoped<IAuthService, AuthService>();

      builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

      builder.Services.AddCors(options =>
     {
       options.AddPolicy("AllowFrontend",
           policy =>
           {
             policy.WithOrigins("http://localhost:5173")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
           });
     });

      var app = builder.Build();

      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();
      app.UseCors("AllowFrontend");

      app.UseAuthorization();
      app.MapControllers();
      app.Run();
    }
  }
}