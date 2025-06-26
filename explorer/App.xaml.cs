using System;
using System.Windows;

namespace FastImageViewer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            base.OnStartup(e);

            // Глобальная обработка исключений
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            var message = exception?.Message ?? "Неизвестная ошибка";

            MessageBox.Show(
                $"Критическая ошибка приложения:\n{message}\n\nПриложение будет закрыто.",
                "Критическая ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Логирование ошибки
            LogException(exception);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Ошибка в пользовательском интерфейсе:\n{e.Exception.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            // Логирование ошибки
            LogException(e.Exception);

            // Помечаем как обработанную, чтобы приложение не закрылось
            e.Handled = true;
        }

        private void LogException(Exception exception)
        {
            try
            {
                var logPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "FastImageViewer", "Logs");

                System.IO.Directory.CreateDirectory(logPath);

                var logFile = System.IO.Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {exception}\n\n";

                System.IO.File.AppendAllText(logFile, logEntry);
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }
    }
}
