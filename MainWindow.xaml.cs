using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Globals;
using krista_app;
using Microsoft.Win32;
using ModernWpf;
using Windows.System.UserProfile;

namespace practicum_march_april_2025
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Global globals = new Global();
        Global.User user = new Global.User(-1, "nil", "nil", 0, 0);
        Global.PopUp popUp = new Global.PopUp();
        string[] app_user;
        string guest = "guest";

        public MainWindow()
        {
            InitializeComponent();

            if (globals.check_connection() == false)
            {
                if (popUp.Show("Так как не удалось подключиться к серверу, включить Режим предпросмотра?", Global.PopUpType.Warning) == MessageBoxResult.Yes)
                {
                    globals.preview_mode = true;
                }
            }

            if (!globals.preview_mode)
            {
                app_user = globals.check_register();

                if (app_user != null)
                {
                    tboxLogin.Text = app_user[0];
                    pboxPassword.Password = app_user[1];
                    RememberMeBox.IsChecked = app_user[2] == "True";
                }
            } else
            {
                tboxLogin.Text = guest;
                pboxPassword.Password = guest;
                RememberMeBox.IsChecked = true;
            }
        }

        private void LogoClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://krista.ru");
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (CheckUser(tboxLogin.Text, pboxPassword.Password).id != -1)
            {
                if (RememberMeBox.IsEnabled)
                    RememberMe();

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

        Global.User CheckUser(string login, string password)
        {
            if (!globals.preview_mode)
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
                    return user;
                }

                if (result = response.HasRows)
                {
                    response.Read();

                    user = new Global.User(
                       response.GetInt32(0),
                       response.GetString(1),
                       response.GetString(2),
                       response.GetInt16(3),
                       response.GetInt16(4)
                    );
                }

                response.Close();
            } else
            {
                user = new Global.User(1, guest, guest, 1, 1);
            }

            return user;
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

        private void RememberMe(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!globals.preview_mode)
            {
                globals.check_register();

                globals.kristaApp.SetValue("login", tboxLogin.Text.Length > 0 ? tboxLogin.Text : "");
                globals.kristaApp.SetValue("password", pboxPassword.Password.Length > 0 ? pboxPassword.Password : "");
            }
        }

        private void RememberMe()
        {
            if (!globals.preview_mode)
            {
                globals.check_register();

                if (RememberMeBox.IsChecked == true)
                {
                    globals.kristaApp.SetValue("login", tboxLogin.Text.Length > 0 ? tboxLogin.Text : "");
                    globals.kristaApp.SetValue("password", pboxPassword.Password.Length > 0 ? pboxPassword.Password : "");
                }
                globals.kristaApp.SetValue("remember_me", RememberMeBox.IsChecked);
            }
        }
    }
}
