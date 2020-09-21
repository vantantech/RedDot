using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for PickCategory.xaml
    /// </summary>
    public partial class PickCategory : Window
    {

        
        public int CategoryID=0;
        PickCategoryVM pickcategoryvm;
        public PickCategory()
        {
            InitializeComponent();
            pickcategoryvm = new PickCategoryVM();

            this.DataContext = pickcategoryvm;
        }



        private void CategoryClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            CategoryID = int.Parse(picked.Tag.ToString());
            this.Close();
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
