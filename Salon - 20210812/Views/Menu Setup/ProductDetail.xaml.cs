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
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : Window
    {
        ProductVM m_productvm;
        bool enablekeyboard = GlobalSettings.Instance.EnableVirtualKeyboard;

        public ProductDetail(Window parent,Category cat, MenuItem product)
        {
            InitializeComponent();
            m_productvm = new ProductVM(this,cat, product);
            this.DataContext = m_productvm;
            this.Left = parent.Left;
            this.Top = parent.Top ;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            TapTipKeyboard.HideKeyboard();
            this.Close();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if(enablekeyboard)
            TapTipKeyboard.ShowKeyboard();
        }
    }
}
