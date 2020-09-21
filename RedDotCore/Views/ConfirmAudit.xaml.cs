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
    /// Interaction logic for ConfirmAudit.xaml
    /// </summary>
    public partial class ConfirmAudit : Window
    {
        public string Reason { get { return tbReason.Text; } }
        public ConfirmAudit()
        {
            InitializeComponent();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            if(tbReason.Text == "")
            {
                MessageBox.Show("Please enter reason");
            } else
            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            tbReason.Text = "";
            this.Close();
        }

        private void Fixed_OK(object sender, RoutedEventArgs e)
        {
            Button butt = (Button) sender;

            tbReason.Text = butt.Content.ToString();
            this.Close();
        }
    }
}
