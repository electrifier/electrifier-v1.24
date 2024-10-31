using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace electrifier.Helpers;

public class StringToUriConverter : IValueConverter
{
    public StringToUriConverter()
    {
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (parameter is string uriParameter)
        {
            //            UriParser.IsKnownScheme("https")
        }

        return new Uri("https://www.electrifier.org");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return new Uri("https://www.electrifier.org");
    }
}
