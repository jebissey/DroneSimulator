using ADroneSimulator.Bases;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using WPFLocalizeExtension.Engine;

namespace ADroneSimulator;

internal class MainWindowViewModel : ObservableObject
{

    public MainWindowViewModel() 
    {
        _ = new CountryFlags();
        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        LocalizeDictionary.Instance.Culture = new CultureInfo(string.IsNullOrEmpty(Settings.Default.Language) ? "fr-FR" : Settings.Default.Language);
        LocalizeDictionary.Instance.PropertyChanged += CultureChanged;
    }

    public string FlagPath => CountryFlags.GetFlagName(LocalizeDictionary.Instance.Culture.EnglishName.Split('(', ')')[1]);

    #region Private methods
    private static string GetSoftVersion()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fileVersionInfo.FileVersion ?? "";
    }

    private void CultureChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(FlagPath));
        Settings.Default.Language = LocalizeDictionary.Instance.Culture.ToString();
        Settings.Default.Save();
    }
    #endregion
}
