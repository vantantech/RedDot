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
    /// Interaction logic for TextPad.xaml
    /// </summary>
    public partial class TextPad : Window
    {
        public string ReturnText { get; set; }
        public string Action { get; set; }
        private string m_oldtext;
        private bool caps = false;
        private bool shift = false;

        public static string OK = "Ok";
        public static string Cancel = "Cancel";

        public TextPad(string title,string oldtext)
        {
            InitializeComponent();
            m_oldtext = oldtext;
            this.tbText.Text = oldtext;
            this.tbTitle.Text = title;

            tbText.Focus();
            tbText.SelectionStart = tbText.Text.Length;
            tbText.SelectionLength = 0;
           
        }

      
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (caps)
            {
                if (shift)
                {
                    TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText, button.Tag.ToString()));
                    shift = false;
                    btnShiftLeft.Background = Brushes.AliceBlue;
                    btnShiftRight.Background = Brushes.AliceBlue;

                }
                else
                    TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText, button.Tag.ToString().ToUpper()));
            }
            else
            {
                if (shift)
                {
                    
                    TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText, button.Tag.ToString().ToUpper()));
                    shift = false;
                    btnShiftLeft.Background = Brushes.AliceBlue;
                    btnShiftRight.Background = Brushes.AliceBlue;

                }
                else
                    //tbText.Text = tbText.Text + button.Tag.ToString();
                TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText, button.Tag.ToString()));
            }

                tbText.Focus();
               // tbText.SelectionStart = tbText.Text.Length;
              //  tbText.SelectionLength = 0;
        }

        private void btnnumber_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var val = button.Tag.ToString().Split(',');
           
                if (shift)
                {
                   
                    TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText,  val[1]));
                    shift = false;
                    btnShiftLeft.Background = Brushes.AliceBlue;
                    btnShiftRight.Background = Brushes.AliceBlue;

                }
                else
                    TextCompositionManager.StartComposition(new TextComposition(InputManager.Current, tbText, val[0]));

           

            tbText.Focus();
           // tbText.SelectionStart = tbText.Text.Length;
           // tbText.SelectionLength = 0;
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Action = "Cancel";
            ReturnText = m_oldtext;
            this.Close();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            Action = "Ok";
            ReturnText = tbText.Text;
            this.Close();
        }

        private void Button_Back(object sender, RoutedEventArgs e)
        {
            //backspace

            EditingCommands.Backspace.Execute(null, tbText);


           // if (tbText.Text != "")  tbText.Text = tbText.Text.Substring(0, tbText.Text.Length - 1);
            tbText.Focus();
          //  tbText.SelectionStart = tbText.Text.Length;
          //  tbText.SelectionLength = 0;


        }

        private void btn_CapsClicked(object sender, RoutedEventArgs e)
        {
            if(caps)
            {
                caps = false;
                this.btnCaps.Background = Brushes.AliceBlue;
            }
            else
            {
                caps = true;
                this.btnCaps.Background = Brushes.Red;
            }

            tbText.Focus();
           // tbText.SelectionStart = tbText.Text.Length;
           // tbText.SelectionLength = 0;
        }

        private void btn_ShiftClicked(object sender, RoutedEventArgs e)
        {
            if (shift)
            {
                shift = false;
                btnShiftLeft.Background = Brushes.AliceBlue;
                btnShiftRight.Background = Brushes.AliceBlue;

            }
            else
            {
                shift = true;
                btnShiftLeft.Background = Brushes.Red;
                btnShiftRight.Background = Brushes.Red;

            }

            tbText.Focus();
           // tbText.SelectionStart = tbText.Text.Length;
           // tbText.SelectionLength = 0;
        }

 
  

        private void btn_Enter(object sender, RoutedEventArgs e)
        {

         
            int currentpos = tbText.SelectionStart;

            tbText.Text = tbText.Text.Insert(currentpos, "" + (char)13 + (char)10);
            tbText.Focus();
            tbText.SelectionStart = currentpos + 1;
           
        }

        private void btn_TabClick(object sender, RoutedEventArgs e)
        {

            int currentpos = tbText.SelectionStart;

            tbText.Text = tbText.Text.Insert(currentpos, "" +  (char)9);
            tbText.Focus();
            tbText.SelectionStart = currentpos + 1;
        }

        private void btn_ClearAllClicked(object sender, RoutedEventArgs e)
        {
            tbText.Text = "";
            tbText.Focus();
            tbText.SelectionStart = tbText.Text.Length;
            tbText.SelectionLength = 0;
        }
    }
}
