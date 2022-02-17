using System;
using System.Collections.Generic;
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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for DeleteConfirm.xaml
    /// </summary>
    public partial class Confirm : Window
    {
        public string Action = "";
        public string Response = "";
        public static string OK = "Yes";
        public Confirm(string message="")
        {
            InitializeComponent();
            if (message != "") tbMessage.Text = message;
        }


        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            Response = chosen.Tag.ToString().Trim();
            this.Close();
        }

        public static bool Ask(string message)
        {
            Confirm conf = new Confirm(message)
            {
                Topmost = true,
                ShowInTaskbar = false
            };
            conf.ShowDialog();
            if (conf.Response == Confirm.OK) return true; else return false;

        }
    }
}
