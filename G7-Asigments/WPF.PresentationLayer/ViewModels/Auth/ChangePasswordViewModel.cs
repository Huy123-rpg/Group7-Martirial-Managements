using BLL.BusinessLogicLayer.Services.Auth;
using System;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Auth;

public class ChangePasswordViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    public Guid UserId { get; set; }

    private string _currentPassword = string.Empty;
    private string _newPassword     = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage    = string.Empty;

    public string CurrentPassword { get => _currentPassword; set => SetField(ref _currentPassword, value); }
    public string NewPassword     { get => _newPassword;     set => SetField(ref _newPassword, value); }
    public string ConfirmPassword { get => _confirmPassword; set => SetField(ref _confirmPassword, value); }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { SetField(ref _errorMessage, value); OnPropertyChanged(nameof(ErrorVisibility)); }
    }

    public Visibility ErrorVisibility
        => string.IsNullOrEmpty(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

    public ChangePasswordViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    // ─── Command ──────────────────────────────────────────────────────────────
    public RelayCommand ConfirmCommand => new(Confirm, CanConfirm);

    private bool CanConfirm()
        => !string.IsNullOrWhiteSpace(CurrentPassword)
        && !string.IsNullOrWhiteSpace(NewPassword)
        && !string.IsNullOrWhiteSpace(ConfirmPassword);

    private void Confirm()
    {
        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Mật khẩu xác nhận không khớp.";
            return;
        }

        if (NewPassword.Length < 8)
        {
            ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.";
            return;
        }

        if (NewPassword == CurrentPassword)
        {
            ErrorMessage = "Mật khẩu mới không được trùng mật khẩu cũ.";
            return;
        }

        try
        {
            bool ok = _authService.ChangePassword(UserId, CurrentPassword, NewPassword);
            if (!ok) { ErrorMessage = "Mật khẩu hiện tại không đúng."; return; }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi: {ex.Message}";
            return;
        }

        foreach (Window w in Application.Current.Windows)
        {
            if (w is Views.Auth.ChangePasswordWindow)
            {
                w.DialogResult = true;
                return;
            }
        }
    }
}
