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
    private static readonly BitmapImage DefaultFolderImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default Folder.ico"));
    private static readonly BitmapImage DefaultFileImage = new(new Uri("ms-appx:///Assets/Views/Workbench/Shell32 Default unknown File.ico"));

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
        get; private init;
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
            ? DefaultFolderImage
            : DefaultFileImage;
    }

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
