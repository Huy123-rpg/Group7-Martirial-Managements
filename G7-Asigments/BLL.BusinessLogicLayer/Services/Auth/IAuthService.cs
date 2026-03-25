using DAL.DataAccessLayer.Model;

namespace BLL.BusinessLogicLayer.Services.Auth;

public interface IAuthService
{
    /// <summary>Đăng nhập bằng email + password.</summary>
    User? Login(string email, string password);

    bool ChangePassword(Guid userId, string oldPassword, string newPassword);

    void UpdateLastLogin(Guid userId);

    /// <summary>Tạo tài khoản mới. Trả về User nếu thành công, null nếu lỗi.</summary>
    User? Register(string fullName, string email, string password, byte roleId, out string error);
}
