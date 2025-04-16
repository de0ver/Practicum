using System;
using System.Windows;

namespace practicum_march_april_2025
{
    /// <summary>
    /// Логика взаимодействия для Hub.xaml
    /// </summary>
    public partial class Hub : Window
    {
        public Hub()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
