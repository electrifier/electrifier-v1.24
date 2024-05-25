using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Vanara.PInvoke;
using Vanara.Windows.Shell;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace electrifier.Controls.Vanara;

public sealed partial class Shell32GridView : UserControl
{
    public readonly ObservableCollection<Shell32GridViewItem> GridShellItems;

    public Shell32GridView()
    {
        InitializeComponent();

        GridShellItems = new ObservableCollection<Shell32GridViewItem>();

        GridShellItems.Add(new Shell32GridViewItem(ShellFolder.Desktop));
    }
}

public class Shell32GridViewItem
{
    public string Name { get; }
    public string Path { get; }

    public Shell32GridViewItem(ShellItem shellItem)
    {
        Name = shellItem.Name ?? "NoName";
        Path = shellItem.ParsingName ?? "NoParsingName";
    }
}
