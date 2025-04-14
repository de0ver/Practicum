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

            ConnectStatusSet(globals.connection.State == System.Data.ConnectionState.Open);

            ServerName.Text = globals.conn_server;
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

        private void Reset(object sender, RoutedEventArgs e)
        {
            ServerName.Text = UserName.Text = ServerPass.Password = DbName.Text = "";
            ConnectStatusSet(false);
        }
        private void Done(object sender, RoutedEventArgs e)
        {
            ConnectStatusSet(globals.check_connection(UserName.Text, ServerPass.Password, DbName.Text, ServerName.Text));
        }
    }
}
