using Globals;
using System;
using System.Windows;

namespace practicum_march_april_2025
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        Global globals;
        public Settings(Global shit)
        {
            InitializeComponent();

            globals = shit;

            if (!globals.preview_mode)
            {
                ConnectStatusSet(globals.connection.State == System.Data.ConnectionState.Open);
                ServerName.ItemsSource = globals.services;
                ServerName.SelectedIndex = 0;
            }
            else
            {
                ServerName.Text = globals.conn_server + "\\MSSQLSERVER";
                ConnectStatusSet(false);
            }
            
            UserName.Text = globals.conn_user;
            ServerPass.Password = globals.conn_pass;
            DbName.Text = globals.conn_db;
        }

        void ConnectStatusSet(bool open) //красим кружок и меняем текст в зависимости от подключения к бд
        {
            if (open)
            {
                ConnectStatus.Content = "Подключено";
                ConnectStatusIco.Fill = globals.colors["Green"];
            } else {
               ConnectStatus.Content = "Не подключено";
               ConnectStatusIco.Fill = globals.colors["Red"];
            }
        }

        private void Reset(object sender, RoutedEventArgs e) //отключаемся, хз зачем
        {
            ServerName.Text = UserName.Text = ServerPass.Password = DbName.Text = "";
            globals.connection.Close();
            globals.check_register();

            try
            {
                globals.kristaApp.SetValue("server_name", "");
                globals.kristaApp.SetValue("server_login", "");
                globals.kristaApp.SetValue("server_password", "");
                globals.kristaApp.SetValue("server_db", "");
            } catch (Exception)
            {

            }

            ConnectStatusSet(false);
        }
        private void Done(object sender, RoutedEventArgs e) //при нажатии на зеленую кнопочку, проверяем подключение с базой
        {
            globals.check_register();

            try
            {
                globals.kristaApp.SetValue("server_name", ServerName.Text);
                globals.kristaApp.SetValue("server_login", UserName.Text);
                globals.kristaApp.SetValue("server_password", ServerPass.Password);
                globals.kristaApp.SetValue("server_db", DbName.Text);
            } catch (Exception)
            {

            }

            if (!globals.preview_mode)
                ConnectStatusSet(globals.check_connection(UserName.Text, ServerPass.Password, DbName.Text, ServerName.Text));
            else
                ConnectStatusSet(false);
        }
    }
}
