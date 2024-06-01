using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Vanara.Windows.Shell;

using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using Vanara.PInvoke;
using Vanara.Extensions;
using Vanara.InteropServices;
using Vanara.Windows.Shell;
using Windows.Foundation.Collections;
using Windows.Foundation;
using System.Diagnostics;


namespace electrifier.Controls.Vanara;

public class /*record*/ Shell32GridViewItem
{
    public string DisplayName
    {
        get;
    }

    public string HintText
    {
        get; private set;
    }

    // TODO: Use: async bitmap for XAML binding: public readonly ImageSource ImageSource;

    public bool IsFolder
    {
        get; private set;
    }

    public ShellItem ShellItem
    {
        get;
    }

    public ImageSource ImageIconSource
    {
        get;
    }

    public Shell32GridViewItem(ShellItem shItem, string? overrideDisplayName = default)
    {
        ShellItem = shItem ?? throw new ArgumentNullException(nameof(shItem));
        DisplayName = overrideDisplayName ?? (ShellItem.Name ?? throw new Exception("shItem Display Name"));
        IsFolder = shItem.IsFolder;
        HintText = (IsFolder ? "Folder: " : "File: ") + shItem.ParsingName;

        ImageIconSource = shItem.IsFolder
            ? new BitmapImage(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"))
            : new BitmapImage(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));

        //var bitmap = new BitmapImage
        //{
        //    // TODO: make async: bitmap.SetSourceAsync("ms-appx:///...");
        //    UriSource = new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico")
        //};
    }

    //public Shell32GridViewItem(ShellItem shItem, string displayName, string hintText, bool isFolder, ShellItem shellItem)
    //    : this(shItem, displayName)
    //{
    //    HintText = hintText ?? throw new ArgumentNullException(nameof(hintText));
    //    IsFolder = isFolder;
    //    ShellItem = shellItem ?? throw new ArgumentNullException(nameof(shellItem));
    //}

    internal static Shell32GridViewItem? Parent(ShellItem shItem)
    {
        if (shItem is null)
        {
            throw new ArgumentNullException(nameof(shItem));
        }

        if (shItem.Parent is null)
        {
            return null;
        }

        return new Shell32GridViewItem(shItem.Parent, overrideDisplayName: "..")
        {
            IsFolder = true,
            HintText = shItem.Parent.ParsingName ?? throw new Exception("shItem Display Name")
        };
    }
}
