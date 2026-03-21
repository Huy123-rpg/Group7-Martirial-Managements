using DAL.DataAccessLayer.Models._Core;

namespace BLL.BusinessLogicLayer.Services.Auth;

public interface IAuthService
{
    User? Login(string username, string password);
    bool ChangePassword(Guid userId, string oldPassword, string newPassword);
    User? Register(string username, string password, string fullName, string email, byte roleId, out string error);
}
