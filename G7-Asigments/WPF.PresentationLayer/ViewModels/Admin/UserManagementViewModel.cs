using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.PresentationLayer.Helpers;

namespace WPF.PresentationLayer.ViewModels.Admin;

public class UserManagementViewModel : BaseViewModel
{
    private readonly UnitOfWork _uow = UnitOfWork.Instance;

    private ObservableCollection<User> _users = [];
    private User? _selected;
    private string _searchText = string.Empty;

    public ObservableCollection<User> Users
    {
        get => _users;
        set => SetField(ref _users, value);
    }

    public User? Selected
    {
        get => _selected;
        set
        {
            SetField(ref _selected, value);
            OnPropertyChanged(nameof(ToggleActiveLabel));
            OnPropertyChanged(nameof(ActionButtonsVisible));
        }
    }

    // Ẩn nút khi chọn Admin
    public Visibility ActionButtonsVisible
    {
        get
        {
            if (Selected == null) return Visibility.Visible;
            var adminRole = _uow.UserRoles.Find(r => r.RoleCode == "ADMIN").FirstOrDefault();
            return (adminRole != null && Selected.RoleId == adminRole.RoleId)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    public string SearchText
    {
        get => _searchText;
        set { SetField(ref _searchText, value); Search(); }
    }

    public string ToggleActiveLabel
        => Selected?.IsActive == true ? "Vô hiệu hóa" : "Kích hoạt";

    // ─── Commands ────────────────────────────────────────────────────────────
    public RelayCommand CreateCommand       => new(OpenCreateWindow);
    public RelayCommand ToggleActiveCommand => new(ToggleActive,  () => Selected != null);
    public RelayCommand DeleteCommand       => new(DeleteUser,    () => Selected != null);

    public UserManagementViewModel() => Load();

    // ─── Load ─────────────────────────────────────────────────────────────────
    private void Load()
    {
        var items = _uow.Users.GetAll()
            .OrderByDescending(u => u.CreatedAt)
            .ToList();
        Users = new ObservableCollection<User>(items);
    }

    // ─── Search ───────────────────────────────────────────────────────────────
    private void Search()
    {
        var all = _uow.Users.GetAll();
        if (!string.IsNullOrWhiteSpace(SearchText))
            all = all.Where(u =>
                u.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (u.StaffCode ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Users = new ObservableCollection<User>(all.OrderByDescending(u => u.CreatedAt));
    }

    // ─── Toggle Active ────────────────────────────────────────────────────────
    private void ToggleActive()
    {
        if (Selected == null) return;

        string action = Selected.IsActive ? "vô hiệu hóa" : "kích hoạt";
        var result = MessageBox.Show(
            $"Bạn chắc chắn muốn {action} tài khoản \"{Selected.Email}\"?",
            "Xác nhận",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        Selected.IsActive  = !Selected.IsActive;
        Selected.UpdatedAt = DateTimeOffset.UtcNow;
        _uow.Users.Update(Selected);
        _uow.Save();
        Load();
    }

    // ─── Delete ───────────────────────────────────────────────────────────────
    private void DeleteUser()
    {
        if (Selected == null) return;

        var result = MessageBox.Show(
            $"Bạn chắc chắn muốn xóa tài khoản \"{Selected.Email}\"?\n" +
            "Hành động này không thể hoàn tác.",
            "Xác nhận xóa",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            _uow.Users.Delete(Selected);
            _uow.Save();
            Load();
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            MessageBox.Show($"Xóa thất bại: {detail}", "Lỗi",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── Create ───────────────────────────────────────────────────────────────
    private void OpenCreateWindow()
    {
        var win = new Views.Auth.RegisterWindow();

        if (win.DataContext is ViewModels.Auth.RegisterViewModel vm)
            vm.IsStandalone = false;

        win.ShowDialog();
        Load();
    }
}
