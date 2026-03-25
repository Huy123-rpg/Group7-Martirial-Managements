namespace BLL.BusinessLogicLayer.Services.Email;

public interface IEmailService
{
    /// <summary>Gửi email thông báo tài khoản mới được tạo.</summary>
    Task SendWelcomeEmailAsync(string toEmail, string fullName, string tempPassword);

    /// <summary>Gửi email thông báo lịch công việc vừa được phân công.</summary>
    Task SendScheduleAssignmentEmailAsync(
        string toEmail, string fullName,
        string scheduleTitle, DateTimeOffset startTime,
        string warehouseName, string? description);
}
