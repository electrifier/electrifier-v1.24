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

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;


    //[ObservableProperty]
    //private string _appearance;

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
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();
        //_appearance = "Monochrome";

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    /// <summary>
    /// documentation: https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggerdisplayattribute?view=net-6.0
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


    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
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
}
