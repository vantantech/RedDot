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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowVM ordervm;

        public MainWindow()
        {
            InitializeComponent();
            ordervm = new MainWindowVM(this);
            this.DataContext = ordervm;
        }

        public string BottomMessage
        {
            set
            {
                ordervm.Message = value;
            }

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

    

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

  

   
    }
}
