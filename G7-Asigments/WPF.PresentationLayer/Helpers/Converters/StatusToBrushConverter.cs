using BLL.BusinessLogicLayer.Services.Scheduling;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.PresentationLayer.Helpers.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as string) switch
        {
            ScheduleService.StatusDraft      => new SolidColorBrush(Color.FromRgb( 33, 150, 243)), // #2196F3 blue
            ScheduleService.StatusInProgress => new SolidColorBrush(Color.FromRgb(255, 152,   0)), // #FF9800 orange
            ScheduleService.StatusCompleted  => new SolidColorBrush(Color.FromRgb( 56, 142,  60)), // #388E3C green
            ScheduleService.StatusCancelled  => new SolidColorBrush(Color.FromRgb(211,  47,  47)), // #D32F2F red
            ScheduleService.StatusMissed     => new SolidColorBrush(Color.FromRgb(255, 111,   0)), // #FF6F00 deep orange
            _                                => new SolidColorBrush(Color.FromRgb(189, 189, 189)), // fallback grey
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
