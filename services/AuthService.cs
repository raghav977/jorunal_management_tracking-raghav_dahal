using System;
using System.Linq;

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
        _dbContext.Database.EnsureCreated();
    }

    public bool VerifyPassword(string password)
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
        return _dbContext.UserSecurities.Any();
    }
}