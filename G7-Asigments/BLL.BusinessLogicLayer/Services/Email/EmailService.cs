using System.Net;
using System.Net.Mail;

namespace BLL.BusinessLogicLayer.Services.Email;

public class EmailService : IEmailService
{
    // ── Cấu hình SMTP ── thay bằng thông tin thật ─────────────────────────────
    private const string SmtpHost     = "smtp.gmail.com";
    private const int    SmtpPort     = 587;
    private const string SenderEmail  = "littletree120805@gmail.com";   // ← đổi
    private const string SenderName   = "Warehouse Management";
    private const string AppPassword  = "oglp rkjn mvbi tkhu";    // ← Gmail App Password
    // ──────────────────────────────────────────────────────────────────────────

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, string tempPassword)
    {
        using var client = new SmtpClient(SmtpHost, SmtpPort)
        {
            Credentials = new NetworkCredential(SenderEmail, AppPassword),
            EnableSsl   = true
        };

        var mail = new MailMessage
        {
            From       = new MailAddress(SenderEmail, SenderName),
            Subject    = "[Warehouse Management] Thông tin tài khoản của bạn",
            IsBodyHtml = true,
            Body       = BuildBody(fullName, toEmail, tempPassword)
        };
        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }

    public async Task SendScheduleAssignmentEmailAsync(
        string toEmail, string fullName,
        string scheduleTitle, DateTimeOffset startTime,
        string warehouseName, string? description)
    {
        using var client = new SmtpClient(SmtpHost, SmtpPort)
        {
            Credentials = new NetworkCredential(SenderEmail, AppPassword),
            EnableSsl   = true
        };

        var mail = new MailMessage
        {
            From       = new MailAddress(SenderEmail, SenderName),
            Subject    = $"[Warehouse] Bạn có nhiệm vụ mới: {scheduleTitle}",
            IsBodyHtml = true,
            Body       = BuildAssignmentBody(fullName, scheduleTitle, startTime, warehouseName, description)
        };
        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }

    private static string BuildAssignmentBody(
        string fullName, string title, DateTimeOffset startTime,
        string warehouseName, string? description) => $"""
        <div style="font-family:Arial,sans-serif;max-width:520px;margin:auto;border:1px solid #e0e0e0;border-radius:8px;padding:32px">
            <h2 style="color:#1976D2;margin-top:0">Warehouse Management System</h2>
            <p>Xin chào <strong>{fullName}</strong>,</p>
            <p>Bạn vừa được phân công một nhiệm vụ mới:</p>
            <table style="border-collapse:collapse;width:100%;margin:16px 0">
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold;width:35%">Nhiệm vụ</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0"><strong>{title}</strong></td>
                </tr>
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold">Kho</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0">{warehouseName}</td>
                </tr>
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold">Thời gian bắt đầu</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0">{startTime.ToLocalTime():dd/MM/yyyy HH:mm}</td>
                </tr>
                {(string.IsNullOrWhiteSpace(description) ? "" : $"""
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold">Ghi chú</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0">{description}</td>
                </tr>
                """)}
            </table>
            <p>Vui lòng đăng nhập hệ thống để xem chi tiết và xác nhận thực hiện.</p>
            <hr style="border:none;border-top:1px solid #e0e0e0;margin:24px 0"/>
            <p style="color:#757575;font-size:12px;margin:0">Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
        """;

    private static string BuildBody(string fullName, string email, string tempPassword) => $"""
        <div style="font-family:Arial,sans-serif;max-width:520px;margin:auto;border:1px solid #e0e0e0;border-radius:8px;padding:32px">
            <h2 style="color:#1a73e8;margin-top:0">Warehouse Management System</h2>
            <p>Xin chào <strong>{fullName}</strong>,</p>
            <p>Tài khoản của bạn đã được tạo thành công. Thông tin đăng nhập:</p>
            <table style="border-collapse:collapse;width:100%;margin:16px 0">
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold;width:40%">Email</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0">{email}</td>
                </tr>
                <tr>
                    <td style="padding:8px 12px;background:#f5f5f5;font-weight:bold">Mật khẩu tạm thời</td>
                    <td style="padding:8px 12px;border-left:1px solid #e0e0e0;font-family:monospace">{tempPassword}</td>
                </tr>
            </table>
            <p style="color:#d32f2f;font-weight:bold">⚠ Vui lòng đổi mật khẩu ngay khi đăng nhập lần đầu.</p>
            <hr style="border:none;border-top:1px solid #e0e0e0;margin:24px 0"/>
            <p style="color:#757575;font-size:12px;margin:0">Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
        """;
}
