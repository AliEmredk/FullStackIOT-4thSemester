using dataaccess;
using dataaccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;
    private readonly PasswordHasher<AppUser> hasher = new();

    public AuthService(AppDbContext db, JwtService jwt)
    {
        this._db = db;
        this._jwt = jwt;
    }

    public async Task<string?> Login(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if(user == null)
            return null;
        
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        
        if(result == PasswordVerificationResult.Failed)
            return null;
        
        return _jwt.GenerateToken(user.Id.ToString(), user.Username);
    }

    public async Task Register(string username, string password)
    {
        var user = new AppUser
        {
            Username = username,
            PasswordHash = hasher.HashPassword(null!, password)
        };
        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
}