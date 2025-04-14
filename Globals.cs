using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Media;

namespace Globals
{
    public class Global
    {
        public SqlConnection connection;
        public string conn_user = "admin";
        public string conn_pass = "admin";
        public string conn_db = "practicum";
        public string conn_server = Environment.MachineName;
        private string[] tables = { "Users", "Roles", "Groups" };
        public RegistryKey registryKey = Registry.CurrentUser;
        public RegistryKey kristaApp;

        public enum PopUpType
        {
            OK = 0,
            Error,
            Warning,
        };

        public class PopUp
        {
            public MessageBoxResult Show(string text, string title, PopUpType type)
            {
                MessageBoxResult result = 0;

                switch ((int)type)
                {
                    case 0:
                        result = MessageBox.Show(
                            text,
                            title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        break;
                    case 1:
                        result = MessageBox.Show(
                            text,
                            title,
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        break;
                    case 2:
                        result = MessageBox.Show(
                            text,
                            title,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning
                        );
                        break;
                    default:
                        result = 0;
                        break;
                }

                return result;
            }

            public MessageBoxResult Show(string text, PopUpType type)
            {
                MessageBoxResult result = 0;

                switch ((int)type)
                {
                    case 0:
                        result = MessageBox.Show(
                            text,
                            "Информация",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        break;
                    case 1:
                        result = MessageBox.Show(
                            text,
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        break;
                    case 2:
                        result = MessageBox.Show(
                            text,
                            "Предупреждение",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning
                        );
                        break;
                    default:
                        result = 0;
                        break;
                }

                return result;
            }
        }

        public string[] check_register()
        {
            string[] user = new string[7];

            try
            {
                if ((kristaApp = registryKey.OpenSubKey("krista_app", true)) != null)
                {
                    user[0] = kristaApp.GetValue("login").ToString();
                    user[1] = kristaApp.GetValue("password").ToString();
                    user[2] = kristaApp.GetValue("remember_me").ToString();
                    user[3] = kristaApp.GetValue("server_name").ToString();
                    user[4] = kristaApp.GetValue("server_login").ToString();
                    user[5] = kristaApp.GetValue("server_password").ToString();
                    user[6] = kristaApp.GetValue("server_db").ToString();
                }
                else
                {
                    kristaApp = registryKey.CreateSubKey("krista_app");
                }
            } catch (Exception)
            {

            }

            return user;
        }

        public bool check_services()
        {
            ServiceController[] service;
            service = ServiceController.GetServices();

            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].DisplayName.Contains("MSSQL") && service[i].Status == ServiceControllerStatus.Running)
                {
                    conn_server += "\\" + service[i].ServiceName.Replace("MSSQL$", "");
                    return true;
                }
            }

            Show("Службы SQL Server не найдены!\n", PopUpType.OK);

            return false;
        }

        public bool check_connection()
        {
            if (!check_services())
                return false;

            connection = connect(
                $"data source={conn_server};initial catalog={conn_db};user id={conn_user};password={conn_pass};MultipleActiveResultSets = True"
            );

            if (connection.State == System.Data.ConnectionState.Closed)
                return false;

            if (!check_tables())
                return false;

            return !(connection.State == System.Data.ConnectionState.Closed);
        }

        public bool check_connection(string user, string pass, string db, string server)
        {
            if (!check_services())
                return false;

            connection = connect(
                $"data source={server};initial catalog={db};user id={user};password={pass};MultipleActiveResultSets = True"
            );

            if (connection.State == System.Data.ConnectionState.Closed)
                return false;

            if (!check_tables())
                return false;

            return !(connection.State == System.Data.ConnectionState.Closed);
        }

        private bool check_tables()
        {
            SqlDataReader response;
            SqlCommand comm;
            for (int i = 0; i < tables.Length; i++)
            {
                try
                {
                    comm = command($"select * from dbo.{tables[i]}");
                    response = comm.ExecuteReader();
                    if (!response.HasRows) { }
                }
                catch (SqlException ex)
                {
                    if (
                        Show($"Не найдена таблица {tables[i]}! Создать?", PopUpType.Warning)
                        == MessageBoxResult.Yes
                    )
                    {
                        if (create_table(tables[i]))
                            Show("Таблица создана!", PopUpType.OK);
                        else
                            Show(ex.Message, PopUpType.Error);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private SqlConnection connect(string conn_text)
        {
            SqlConnection conn = new SqlConnection(conn_text);
            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case -1:
                        Show("Подключение к серверу не установлено, сервер не найден или недоступен. Измените подключение в настройках.", PopUpType.Error);
                        break;
                    case 233:
                        Show("Подключение к серверу установлено, неверно указано подключение к базе!. Измените подключение в настройках.", PopUpType.Error);
                        break;
                    default:
                        Show(ex.Message, PopUpType.Error);
                     break;
                }
            }

            return conn;
        }

        public SqlCommand command(string text)
        {
            return new SqlCommand(text, connection);
        }

        private bool create_table(string name)
        {
            try
            {
                //todo create tables script
                return command(
                            $"USE [{conn_db}]\r\n\r\n/****** Object:  Table [dbo].[{name}]    Script Date: 14.04.2025 6:53:09 ******/\r\nSET ANSI_NULLS ON\r\n\r\nSET QUOTED_IDENTIFIER ON\r\n\r\nCREATE TABLE [dbo].[{name}](\r\n\t[ID] [int] NOT NULL,\r\n\t[login] [nvarchar](50) NULL,\r\n\t[password] [nvarchar](50) NULL,\r\n\t[name] [nvarchar](max) NULL,\r\n\t[role_id] [smallint] NULL,\r\n\t[group_id] [smallint] NULL\r\n) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\r\n"
                        ).ExecuteNonQuery() > 0;
            }
            catch (SqlException ex)
            {
                return Show(ex.Message, PopUpType.Error) == MessageBoxResult.None;
            }
        }

        public MessageBoxResult Show(string text, string title, PopUpType type)
        {
            PopUp popUp = new PopUp();

            return popUp.Show(text, title, type);
        }

        public MessageBoxResult Show(string text, PopUpType type)
        {
            PopUp popUp = new PopUp();

            return popUp.Show(text, type);
        }

        public Dictionary<string, SolidColorBrush> colors = new Dictionary<
            string,
            SolidColorBrush
        >()
        {
            ["Red"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x00)),
            ["Green"] = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x00)),
            ["Blue"] = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xFF)),
        };

        public class User
        {
            public int id { get; set; }
            public string name { get; set; }
            public string login { get; set; }
            public int role_id { get; set; }
            public int group_id { get; set; }

            public User(int id, string name, string login, int role_id, int group_id)
            {
                this.id = id;
                this.name = name;
                this.login = login;
                this.role_id = role_id;
                this.group_id = group_id;
            }

            public string get_role()
            {
                return "";
                /*
                PopUp popUp = new PopUp();

                SqlDataReader reader;
                string role = "nil";
                try {
                    reader = command("select NAME from ROLES where ROLE_ID = Users.ROLE_ID").ExecuteReader(); //TODO
                    reader.Read();
                    role = reader.GetString(0);
                    reader.Close();
                } catch (SqlException ex) {
                    popUp.Show(ex.ToString(), PopUpType.Error);
                }

                return role;
                */
            }
        }
    }
}
