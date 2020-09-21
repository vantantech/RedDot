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
    /// Interaction logic for DeleteEdit.xaml
    /// </summary>
    public partial class EditDelete : Window
    {
         public string Action = "";
         public string Item { get; set; }
         public string Category { get; set; }

        public EditDelete(Window parent,string item, string category)
        {
            Item = item;
            Category = category;
            InitializeComponent();
            if (category == "") btnRemove.Visibility = Visibility.Collapsed;
            else btnDelete.Visibility = Visibility.Collapsed;

            this.Left = parent.Left ;
            this.Top = parent.Top + 40;
            this.DataContext = this;
        }


        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }
    }
}
