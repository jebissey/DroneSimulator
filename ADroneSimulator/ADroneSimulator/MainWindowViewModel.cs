﻿using ADroneSimulator.Bases;
using ADroneSimulator.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gamepad;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WPFLocalizeExtension.Engine;

namespace ADroneSimulator;

internal partial class MainWindowViewModel : ObservableObject
{
    #region Fields
    private GamePad gamePad;
    #endregion

    public MainWindowViewModel()
    {
        _ = new CountryFlags();
        LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        LocalizeDictionary.Instance.Culture = new CultureInfo(string.IsNullOrEmpty(Settings.Default.Language) ? "fr-FR" : Settings.Default.Language);
        LocalizeDictionary.Instance.PropertyChanged += CultureChanged;

        _ = new Scene(Objects);

        Customize();
    }

    #region Properties
    public string FlagPath => CountryFlags.GetFlagName(LocalizeDictionary.Instance.Culture.EnglishName.Split('(', ')')[1]);
    public ObservableCollection<Visual3D> Objects { get; } = [];
    #endregion

    #region RelayCommand
    [RelayCommand]
    public void Quit()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    public void Customize()
    {
        if (gamePad != null) GamePad.StopListening();
        gamePad = new();
        if (!gamePad.StartListening()) MessageBox.Show(Resources.Resources.GamepadNotFound, Resources.Resources.MissingGamepad, MessageBoxButton.OK, MessageBoxImage.Warning);
        else MessageBox.Show($"{Resources.Resources.GamepadFound}{GamePad.GetJoystickName()}", "", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public ICommand WindowClosing => new RelayCommand<CancelEventArgs>((args) =>
    {
        GamePad.StopListening();
    });
    #endregion

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
