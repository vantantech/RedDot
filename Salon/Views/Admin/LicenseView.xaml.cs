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
    /// Interaction logic for License.xaml
    /// </summary>
    public partial class LicenseView : Window
    {
        private LicenseVM licensevm;
        public LicenseView()
        {
            InitializeComponent();
            licensevm = new LicenseVM();
            this.DataContext = licensevm;
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // LicenseModel.CreateMachineFile();
           TouchMessageBox.Show("License Request File has been created in your Red Dot folder");
        }
    }
}
