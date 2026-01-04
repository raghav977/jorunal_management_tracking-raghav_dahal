using System.Security.Cryptography;
using System.Text;
using MauiApp3.Data;
using MauiApp3.Models;

namespace MauiApp3.Services;

public class AuthService
{
    private readonly AppDbContext _dbContext;

    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.InitializeDatabase();
    }

    public bool VerifyPassword(string password)
    {
        try
        {
            var userSecurity = _dbContext.UserSecurities.FirstOrDefault();
            if (userSecurity == null)
            {
                return false;
            }
            var hashedInputBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var hashedInputString = Convert.ToHexStringLower(hashedInputBytes);

            return hashedInputString == userSecurity.PasswordHash;
        }
        catch
        {
            return false;
        }
    }

    public void SetPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var hashedString = Convert.ToHexStringLower(hashedBytes);

        var userSecurity = _dbContext.UserSecurities.FirstOrDefault();
        if (userSecurity == null)
        {
            userSecurity = new UserSecurity
            {
                PasswordHash = hashedString,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _dbContext.UserSecurities.Add(userSecurity);
        }
        else
        {
            userSecurity.PasswordHash = hashedString;
            userSecurity.UpdatedAt = DateTime.UtcNow;
            _dbContext.UserSecurities.Update(userSecurity);
        }
        _dbContext.SaveChanges();
    }

    public bool IsPasswordSet()
    {
        try
        {
            return _dbContext.UserSecurities.Any();
        }
        catch
        {
            return false;
        }
    }
}