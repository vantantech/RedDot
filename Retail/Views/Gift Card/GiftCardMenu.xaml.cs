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
    /// Interaction logic for GiftCardMenu.xaml
    /// </summary>
    public partial class GiftCardMenu : Window
    {
        Security m_security;
        public GiftCardMenu(Security security)
        {
            InitializeComponent();
            m_security = security;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            string action = "";
            Button chosen = sender as Button;
            action = chosen.Tag.ToString().Trim();
            Validate(action);
            this.Close();
        }

        private void Validate(string action)
        {

            if(action == "Verify")
            {
                GCVerify gcv = new GCVerify();
                Utility.OpenModal(this,gcv);

            }

            if(action=="View")
            {
                GCManage gcm = new GCManage();
                Utility.OpenModal(this, gcm);

            }

         
        }
    }
}
