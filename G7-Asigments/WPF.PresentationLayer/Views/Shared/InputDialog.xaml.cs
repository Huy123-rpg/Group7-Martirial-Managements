using System.Windows;
using System.Windows.Input;

namespace WPF.PresentationLayer.Views.Shared;

public partial class InputDialog : Window
{
    public string Result { get; private set; } = string.Empty;

    public InputDialog(string prompt, string title = "Nhập thông tin", string defaultValue = "")
    {
        InitializeComponent();
        Title           = title;
        PromptText.Text = prompt;
        InputBox.Text   = defaultValue;
        Loaded += (_, _) => { InputBox.Focus(); InputBox.SelectAll(); };
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        Result          = InputBox.Text;
        DialogResult    = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void InputBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)  OK_Click(sender, e);
        if (e.Key == Key.Escape) Cancel_Click(sender, e);
    }
}
