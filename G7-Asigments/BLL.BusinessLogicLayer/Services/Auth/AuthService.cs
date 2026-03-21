using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models._Core;
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

    // ─── Login ────────────────────────────────────────────────────────────────
    public User? Login(string username, string password)
    {
        var user = _uow.Users
            .Find(u => u.Username == username && u.IsActive)
            .FirstOrDefault();

        if (user == null || !VerifyPassword(password, user.PasswordHash))
            return null;

        return user;
    }

    // ─── Change Password ─────────────────────────────────────────────────────
    public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = _uow.Users.GetById(userId);
        if (user == null || !VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(newPassword);
        _uow.Users.Update(user);
        _uow.Save();
        return true;
    }

    // ─── Register ────────────────────────────────────────────────────────────
    public User? Register(string username, string password, string fullName, string email, byte roleId, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
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

        bool usernameExists = _uow.Users.Find(u => u.Username == username).Any();
        if (usernameExists)
        {
            error = "Tên đăng nhập đã tồn tại.";
            return null;
        }

        bool emailExists = _uow.Users.Find(u => u.Email == email).Any();
        if (emailExists)
        {
            error = "Email đã được sử dụng.";
            return null;
        }

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Username = username.Trim(),
            PasswordHash = HashPassword(password),
            FullName = fullName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            RoleId = roleId,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _uow.Users.Add(newUser);
        _uow.Save();
        return newUser;
    }
}
