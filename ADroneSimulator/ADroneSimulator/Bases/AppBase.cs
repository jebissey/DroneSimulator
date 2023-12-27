using System.Windows.Threading;
using System.Windows;
using Serilog;
using Serilog.Events;

namespace ADroneSimulator.Bases;


internal static class AppBase
{
    /// <summary>
    /// Catch unhandled exception and init logger
    /// </summary>
    public static void Init(string programName, string programVersion)
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnApplicationExit!);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        Log.Logger = new LoggerConfiguration()
         .MinimumLevel.Debug()
         .WriteTo.Console()
         .WriteTo.File($"logs/{programName}_.log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024 * 1024 * 64, retainedFileCountLimit: 100, shared: true)
         .CreateLogger();
        LogStart();

        #region Local methods
        static void OnApplicationExit(object sender, EventArgs e)
        {
            Log.Information("*******************************************");
            Log.Information("Stop");
            Log.Information("*******************************************\n\n");
            Log.CloseAndFlush();
        }

        void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e) => ManageException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

        void LogStart()
        {
            Log.Information("*******************************************");
            Log.Information("Start {program}", $"{programName} (V{programVersion}) by {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");
            Log.Information("*******************************************");
        }
        #endregion
    }

    #region public methods
    public static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ManageException(e.Exception, "AppBase.CurrentOnDispatcherUnhandledException");
        e.Handled = true;
    }

    public static void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ManageException(e.Exception, "AppBase.DispatcherOnUnhandledException");
        e.Handled = true;
    }
    #endregion

    #region Private methods
    static private void ManageException(Exception e, string from)
    {
        if (e is InvalidOperationException)
        {
            if (from == "AppBase.CurrentOnDispatcherUnhandledException") ExceptionManager.Process("", e, LogEventLevel.Information, MessageBoxImage.Information);
        }
        else if (e is InvalidProgramException)
        {
            if (from == "AppBase.CurrentOnDispatcherUnhandledException") ExceptionManager.Process("Program is going to stop", e, LogEventLevel.Fatal);
        }
        else
        {
            string firstLine = $"********** Crash application (from: {from}) **********";
            Log.Information(firstLine);
            ExceptionManager.Process("", e, LogEventLevel.Error);
            Log.Information(e.StackTrace ?? "");
            Log.Information($"{new string('*', firstLine.Length)}\n");
        }
    }
    #endregion
}

