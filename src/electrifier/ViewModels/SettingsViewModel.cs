using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using electrifier.Contracts.Services;
using electrifier.Helpers;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace electrifier.ViewModels;

/// <summary>
/// <a href="https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute?view=net-6.0">Docs for DebuggerDisplay</a>
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class SettingsViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _appearance;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    public ICommand? SwitchAppearanceCommand
    {
        get;
    }
    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        var themeSelectorService1 = themeSelectorService;
        _elementTheme = themeSelectorService1.Theme;
        _versionDescription = GetVersionDescription();
        _appearance = "Monochrome";

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await themeSelectorService1.SetThemeAsync(param);
                }
            });
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMsix)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"electrifier - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    /// <summary>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute?view=net-6.0">Documentation for DebuggerDisplay</a>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    private string GetDebuggerDisplay()
    {
        return new StringBuilder().Append(nameof(SettingsViewModel))
        //.Append($" Appearance={Appearance}")
        .Append($" ElementTheme={ElementTheme}")
        .Append($" VersionDescription={VersionDescription}")
        .ToString();
    }
}
