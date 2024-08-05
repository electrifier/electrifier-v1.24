using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Controls;
using Vanara.PInvoke;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace electrifier.Controls.Vanara;
public sealed partial class Shell32BreadcrumbBar : UserControl
{
    public Shell32BreadcrumbBar()
    {
        this.InitializeComponent();

        NativeBreadcrumbBar.ItemsSource = new ObservableCollection<Shell32.Folder> { };
        NativeBreadcrumbBar.ItemClicked += NativeBreadcrumbBar_ItemClicked;
    }

    private void NativeBreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        var items = NativeBreadcrumbBar.ItemsSource as ObservableCollection<Shell32.Folder>;
        for (int i = items.Count - 1; i >= args.Index + 1; i--)
        {
            items.RemoveAt(i);
        }
    }
}
