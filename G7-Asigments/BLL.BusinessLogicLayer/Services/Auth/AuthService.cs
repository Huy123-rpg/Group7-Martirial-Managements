using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models._Core;

namespace BLL.BusinessLogicLayer.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    public User? Login(string username, string password)
    {
        // TODO: hash password khi implement thực tế
        return _uow.Users
            .Find(u => u.Username == username && u.IsActive)
            .FirstOrDefault();
    }

    public bool ChangePassword(Guid userId, string oldPassword, string newPassword)
    {
        var user = _uow.Users.GetById(userId);
        if (user == null) return false;
        user.PasswordHash = newPassword; // TODO: hash
        _uow.Users.Update(user);
        _uow.Save();
        return true;
    }
}
