using ADroneSimulator.Bases;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace ADroneSimulator;

public partial class App : Application
{
    public App()
    {
        AppBase.Init(Assembly.GetExecutingAssembly().GetName().Name ?? "", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? "");
        Current.DispatcherUnhandledException += AppBase.CurrentOnDispatcherUnhandledException;
        Dispatcher.UnhandledException += AppBase.DispatcherOnUnhandledException;
    }
}
