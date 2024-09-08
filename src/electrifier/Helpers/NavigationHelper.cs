using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Helpers;

/// <summary>
/// Helper class to set the navigation target for a NavigationViewItem.
///
/// Usage in XAML:
/// 
/// Usage in C#:
/// <code>NavigationHelper.SetNavigateTo(navigationViewItem, typeof(MainViewModel).FullName);</code>
/// </summary>
public class NavigationHelper
{
    public static string GetNavigateTo(NavigationViewItem item) => (string)item.GetValue(NavigateToProperty);

    public static void SetNavigateTo(NavigationViewItem item, string value) => item.SetValue(NavigateToProperty, value);

    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(NavigationHelper), new PropertyMetadata(null));
}
