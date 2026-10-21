using System.Globalization;
using Avalonia.Data.Converters;

namespace QRLab.View.Converters;

public sealed class IconNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        "01" => "gen", "02" => "brush", "03" => "calc", "04" => "diff",
        "Кисть" => "brush", "Ластик" => "erase", "Рука" => "hand",
        "QR-код" => "qrcode", "Штрихкод" => "barcode",
        "Рядом" => "diff", "Наложение" => "layers", "Разница" => "selection",
        _ => "selection"
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
