using SocialNetwork.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using SocialNetwork.Repository.Errors;

namespace SocialNetwork.Repository.Services
{
  public class AuthService : IAuthService
  {
    private readonly SocialNetworkDbContext _db;
     private readonly IConfiguration _config;

    public AuthService(SocialNetworkDbContext db, IConfiguration config)
    {
      _db = db;
       _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
      if (await _db.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email))
        return false;

      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

      var user = new User
      {
        Username = request.Username,
        Password = hashedPassword,
        Email = request.Email,
        Created = DateTime.UtcNow.ToString("yyyy-MM-dd")
      };
      _db.Users.Add(user);
      await _db.SaveChangesAsync();
      return true;
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
      var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return null;

      var claims = new[]
      {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id.ToString())
        };

      var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException(AuthErrors.InvalidJwtKey);
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _config["Jwt:Issuer"],
          audience: _config["Jwt:Audience"],
          claims: claims,
          expires: DateTime.UtcNow.AddHours(1),
          signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> DeleteAccountAsync(Guid userId)
    {
      var user = await _db.Users.FindAsync(userId);
      if (user == null)
        return false;

      _db.Users.Remove(user);
      await _db.SaveChangesAsync();
      return true;
    }

    public async Task<bool> EditProfileAsync(Guid userId, EditProfileRequest request)
    {
      var user = await _db.Users.FindAsync(userId);
      if (user == null)
        return false;

      if (!string.IsNullOrEmpty(request.Username))
        user.Username = request.Username;

      if (!string.IsNullOrEmpty(request.Email))
        user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.Password))
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

      _db.Users.Update(user);
      await _db.SaveChangesAsync();
      return true;
    }

        public async Task<string?> GetUsernameAsync(Guid userId)
        {
            return await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();
        }
    }
}