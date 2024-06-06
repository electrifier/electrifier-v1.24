using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;

namespace electrifier.Controls.Vanara;

public sealed partial class ExplorerBrowser : UserControl
{
    //private ShellItem CurrentFolder; 
    public static readonly DependencyProperty CurrentFolderProperty = DependencyProperty.Register(nameof(CurrentFolder),
        typeof(ShellItem), typeof(ExplorerBrowser), new PropertyMetadata(default(ShellItem)));

    public ExplorerBrowser()
    {
        InitializeComponent();
        DataContext = this;


        /* TODO: Remove and initialize by binding */
        CurrentFolder = ShellFolder.Desktop;
    }

    private ShellItem CurrentFolder
    {
        get => (ShellItem)GetValue(CurrentFolderProperty);
        set => SetValue(CurrentFolderProperty, value);
    }
}
