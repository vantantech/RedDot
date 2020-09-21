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
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : Window
    {
        UpdateViewModel updatevm;
        public Update()
        {
            InitializeComponent();
            updatevm = new UpdateViewModel();

            this.DataContext = updatevm;

         


        }


        public string Message { get; set; }

 
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

     
    }
}
