using BLL.BusinessLogicLayer.Core;
using DAL.DataAccessLayer.Models._Core;
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
        set { SetField(ref _selected, value); OnPropertyChanged(nameof(ToggleActiveLabel)); }
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
    public RelayCommand ToggleActiveCommand => new(ToggleActive, () => Selected != null);

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
