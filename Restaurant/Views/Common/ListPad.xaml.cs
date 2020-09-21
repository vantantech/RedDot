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
    /// Interaction logic for Listpad.xaml
    /// </summary>
    public partial class ListPad : Window
    {
     
        public string ReturnString = "";
     
       
        public ListPad(string title, List<CustomList> list, int height=600)
        {
           


            InitializeComponent();
            this.DataContext = list;
            this.Height = height;
            
        }

 

        private void CategoryClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            ReturnString =   picked.Tag.ToString();
            this.Close();
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

 
}
