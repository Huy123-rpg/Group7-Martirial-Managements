using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.BusinessLogicLayer.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    // ─── Hashing ─────────────────────────────────────────────────────────────
    private static string HashPassword(string plain)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static bool VerifyPassword(string plain, string hash)
        => HashPassword(plain) == hash;

    // ─── Login (bằng email) ───────────────────────────────────────────────────
    public User? Login(string email, string password)
    {
        var user = _uow.Users
            .Find(u => u.Email == email.Trim().ToLowerInvariant() && u.IsActive)
            .FirstOrDefault();

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    // ─── Update Last Login ────────────────────────────────────────────────────
    public void UpdateLastLogin(Guid userId)
    {
        var user = _uow.Users.GetById(userId);
        if (user == null) return;
        user.LastLoginAt = DateTimeOffset.UtcNow;
        user.UpdatedAt   = DateTimeOffset.UtcNow;
        _uow.Users.Update(user);
        _uow.Save();
    }

    // ─── Change Password ─────────────────────────────────────────────────────
    public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = _uow.Users.GetById(userId);
        if (user == null || !VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Users.Update(user);
        _uow.Save();
        return true;
    }

    // ─── Register ────────────────────────────────────────────────────────────
    public User? Register(string fullName, string email, string password, byte roleId, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            error = "Vui lòng điền đầy đủ thông tin.";
            return null;
        }

        if (password.Length < 8)
        {
            error = "Mật khẩu phải có ít nhất 8 ký tự.";
            return null;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            error = "Địa chỉ email không hợp lệ.";
            return null;
        }

        string normalizedEmail = email.Trim().ToLowerInvariant();

        bool emailExists = _uow.Users.Find(u => u.Email == normalizedEmail).Any();
        if (emailExists)
        {
            error = "Email đã được sử dụng.";
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        var newUser = new User
        {
            Id           = Guid.NewGuid(),
            FullName     = fullName.Trim(),
            Email        = normalizedEmail,
            PasswordHash = HashPassword(password),
            RoleId       = roleId,
            StaffCode    = $"USR-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            IsActive     = true,
            CreatedAt    = now,
            UpdatedAt    = now
        };

        _uow.Users.Add(newUser);
        _uow.Save();
        return newUser;
    }
}
