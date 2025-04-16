using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Globals;

namespace krista_app
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        Global globals;

        public Admin(Global shit)
        {
            InitializeComponent();

            globals = shit;

            get_workers();
        }

        private void get_workers()
        {
            if (!globals.preview_mode)
            {
                try
                {
                    SqlDataReader response = globals
                        .command("select * from dbo.Users")
                        .ExecuteReader();
                }
                catch (SqlException ex)
                {
                    globals.Show(ex.ToString(), Global.PopUpType.Error);
                }
            }
        }

        private void inputRole_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void DeleteWorker(object sender, RoutedEventArgs e) { }

        private void ReturnWorker(object sender, RoutedEventArgs e) { }

        private void AddWorker(object sender, RoutedEventArgs e) { }

        private void CheckPasswords(object sender, RoutedEventArgs e) { }

        private void Back(object sender, RoutedEventArgs e) { }

        private void CheckInputs(object sender, TextChangedEventArgs e) { }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
