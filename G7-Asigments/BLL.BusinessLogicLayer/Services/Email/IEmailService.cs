namespace BLL.BusinessLogicLayer.Services.Email;

public interface IEmailService
{
    /// <summary>Gửi email thông báo tài khoản mới được tạo.</summary>
    Task SendWelcomeEmailAsync(string toEmail, string fullName, string tempPassword);
}
