using Microsoft.UI.Xaml.Data;
namespace electrifier.Helpers;
/// <summary><see cref="string"/> to <see cref="Uri"/> converters.</summary>
public class StringToUriConverter : IValueConverter
{
    public StringToUriConverter() {  }
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string uriParameter)
        {
            //            UriParser.IsKnownScheme("https")
        }
        return new Uri("https://www.electrifier.org");
    }
    public string ConvertToString(object value, Type targetType, object parameter, string language) => Convert(value, targetType, parameter, language).ToString();
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        // todo: Implement
        return new Uri("https://www.electrifier.org");
    }
}
