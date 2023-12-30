using Serilog;
using Serilog.Events;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ADroneSimulator.Bases;

internal class ExceptionManager
{

    public static void Process(string caption, Exception? exception, LogEventLevel logEventLevel, MessageBoxImage messageBoxImage = MessageBoxImage.Error, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string filePath = "")
    {
        string message = caption;
        if (exception != null)
        {
            string innerExceptionMessage = "";
            if (exception.InnerException != null) innerExceptionMessage = $"\n\n=====> Inner exception:\n{exception.InnerException.Message} \n(-> Inner exception source: {exception.InnerException.Source})";

            string source = "";
            if (!string.IsNullOrEmpty(exception.Source) && exception is not InvalidOperationException) source = $"\n(source: {exception.Source})";
            message = string.Concat(exception.Message, source, innerExceptionMessage);
            if (logEventLevel != LogEventLevel.Debug) ShowMessageBox();
        }
        LogException(logEventLevel);

        #region Local Methods
        void ShowMessageBox()
        {
            Application.Current?.Dispatcher.Invoke(new Action(() =>
            {
                if (Application.Current != null && Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility == Visibility.Visible)
                {
                    _ = MessageBox.Show(Application.Current.MainWindow, message, caption, MessageBoxButton.OK, messageBoxImage);
                }
                else _ = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        void LogException(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Fatal:
                    Log.Information("\n\n################################################################################");
                    Log.Fatal("\nIn {filePath}\non line {lineNumber}: {caption}\n{message} ", filePath, lineNumber, caption, message);
                    Log.Information("###########################################\n\n");
                    Application.Current.Shutdown();
                    return;

                case LogEventLevel.Error: Log.Error("In {filePath} on line {lineNumber}:\n{caption}{message} ", filePath, lineNumber, caption, message); return;
                case LogEventLevel.Warning: Log.Warning("In {filePath} on line {lineNumber}:\n{caption}{message} ", filePath, lineNumber, caption, message); return;
                case LogEventLevel.Verbose: Log.Verbose("In {filePath} on line {lineNumber}:\n{caption}{message} ", filePath, lineNumber, caption, message); return;

                case LogEventLevel.Debug: Log.Debug("In {filePath} on line {lineNumber}:\n{message} ", filePath, lineNumber, message); return;

                case LogEventLevel.Information: Log.Information("{message}", message); return;
            }
            throw new InvalidProgramException($"Unknown LogEventLevel {level}");
        }
        #endregion
    }
}

