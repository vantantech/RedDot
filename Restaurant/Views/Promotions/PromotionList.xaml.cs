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
    /// Interaction logic for Promotions.xaml
    /// </summary>
    public partial class PromotionList : Window
    {

        private PromotionsVM m_promotionsvm;
        public PromotionList()
        {
           
            InitializeComponent();
            m_promotionsvm = new PromotionsVM();
            this.DataContext = m_promotionsvm;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
           
            TapTipKeyboard.HideKeyboard();
            this.Close();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (GlobalSettings.Instance.EnableVirtualKeyboard)
                TapTipKeyboard.ShowKeyboard();
        }
    }
}
