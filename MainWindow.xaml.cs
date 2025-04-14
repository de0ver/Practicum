using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Globals;
using krista_app;
using ModernWpf;

namespace practicum_march_april_2025
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Global globals = new Global();
        Global.User user;
        Global.PopUp popUp = new Global.PopUp();

        public MainWindow()
        {
            InitializeComponent();

            globals.check_connection();
        }

        private void LogoClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://krista.ru");
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (CheckUser(tboxLogin.Text, pboxPassword.Password))
            {
                if (user.role_id == 1)
                {
                    Admin admin_page = new Admin(globals);

                    admin_page.Show();
                }
                else
                {
                    Hub hub = new Hub();

                    hub.Show();
                }

                Hide();
            }
            else
            {
                popUp.Show("Неправильный логин или пароль!", Global.PopUpType.Error);
                pboxPassword.Clear();

                tboxLogin.Focus();
            }
        }

        bool CheckUser(string login, string password)
        {
            SqlDataReader response;
            bool result;
            try
            {
                SqlCommand command = new SqlCommand(
                    $"select ID, LOGIN, NAME, ROLE_ID, GROUP_ID from dbo.Users where login='{login}' and password='{password}'",
                    globals.connection
                );
                response = command.ExecuteReader();
            }
            catch (Exception)
            {
                return false;
            }

            if (result = response.HasRows)
            {
                response.Read();

                user.id = response.GetInt32(0);
                user.login = response.GetString(1);
                user.name = response.GetString(2);
                user.role_id = response.GetInt16(3);
                user.group_id = response.GetInt16(4);
            }

            response.Close();

            return result;
        }

        private void MoonClick(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Light)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                Moon.Source = new BitmapImage(
                    new Uri(@"/img/moon_dark.png", UriKind.RelativeOrAbsolute)
                );
                Settings.Source = new BitmapImage(
                    new Uri(@"/img/gear_dark.png", UriKind.RelativeOrAbsolute)
                );
                Border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                Moon.Source = new BitmapImage(
                    new Uri(@"/img/moon_light.png", UriKind.RelativeOrAbsolute)
                );
                Settings.Source = new BitmapImage(
                    new Uri(@"/img/gear_light.png", UriKind.RelativeOrAbsolute)
                );
                Border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            }
        }

        private void ForgotPassword(object sender, MouseButtonEventArgs e)
        {
            popUp.Show(
                "Для сброса пароля, напишите на почту mailto:help@krista.ru.\nВ письме укажите логин и причину для сброса пароля.",
                Global.PopUpType.OK
            );
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings settings = new Settings(globals);

            settings.ShowDialog();
        }
    }
}
