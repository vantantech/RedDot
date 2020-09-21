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
    /// Interaction logic for GCManage.xaml
    /// </summary>
    public partial class GCManage : Window
    {
        private GiftCardVM m_giftcardvm;
        public GCManage()
        {
            InitializeComponent();
            m_giftcardvm = new GiftCardVM();
            this.DataContext = m_giftcardvm;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
