using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Vanara.PInvoke;
namespace electrifier.Controls.Vanara;
public sealed partial class Shell32BreadcrumbBar : UserControl
{
    /// <summary><seealso href="https://learn.microsoft.com/en-us/windows/apps/design/controls/breadcrumbbar">
    /// BreadcrumbBar control</seealso> for navigating through the Windows Shell32 namespace.</summary>
    public Shell32BreadcrumbBar()
    {
        InitializeComponent();

        NativeBreadcrumbBar.ItemsSource = new ObservableCollection<ExplorerBrowserItem> { };
        NativeBreadcrumbBar.ItemClicked += NativeBreadcrumbBar_ItemClicked;
    }
    /// <summary><see cref="ItemClickEventHandler"/> for <see cref="Shell32BreadcrumbBar"/></summary>
    /// <param name="sender">the owner of this event</param>
    /// <param name="args">event args</param>
    private void NativeBreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (NativeBreadcrumbBar.ItemsSource is ObservableCollection<ExplorerBrowserItem> items)
        {
            for (var i = items.Count - 1; i >= args.Index + 1; i--)
            {
                items.RemoveAt(i);
            }
        }
    }
}
