using System;
using System.Threading.Tasks;
using System.Windows;

namespace practicum_march_april_2025
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ShutdownMode = ShutdownMode.OnLastWindowClose;

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling() //паста https://stackoverflow.com/questions/793100/globally-catch-exceptions-in-a-wpf-application
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException(
                    (Exception)e.ExceptionObject,
                    "AppDomain.CurrentDomain.UnhandledException"
                );

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(
                    e.Exception,
                    "Application.Current.DispatcherUnhandledException"
                );
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System
                    .Reflection.Assembly.GetExecutingAssembly()
                    .GetName();
                message = string.Format(
                    "Unhandled exception in {0} v{1}",
                    assemblyName.Name,
                    assemblyName.Version
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Exception in LogUnhandledException",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                MessageBox.Show(
                    message,
                    exception.ToString(),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
