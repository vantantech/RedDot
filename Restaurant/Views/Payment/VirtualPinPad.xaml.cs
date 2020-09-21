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
    /// Interaction logic for VirtualPinPad.xaml
    /// </summary>
    public partial class VirtualPinPad : Window
    {

        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string EntryMode { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public decimal Tip { get; set; }

        public VirtualPinPad(string mode, decimal amt)
        {
            InitializeComponent();
            Mode = mode;
            tbscreen.Text = "Total:" + amt.ToString();
            Amount = amt;
            TotalAmount = 0;
            Tip = 0;
        }
        private void SwipeClick(object sender, RoutedEventArgs e)
        {
            EntryMode = "SWIPE";
            Status = "APPROVED";
            if (Mode == "SALE") Tip = Amount * 0.15m;
            TotalAmount = Amount + Tip;
            this.Close();
        }

        private void InsertClick(object sender, RoutedEventArgs e)
        {
            EntryMode = "INSERT";
            Status = "APPROVED";
            if (Mode == "SALE") Tip = Amount * 0.15m;
            TotalAmount = Amount + Tip;
            this.Close();
        }


        private void BackClick(object sender, RoutedEventArgs e)
        {
            Amount = 0;
            TotalAmount = Amount + Tip;
            Status = "CANCELLED";
            this.Close();
        }

        private void ErrorClick(object sender, RoutedEventArgs e)
        {
            Amount = 0;
            TotalAmount = Amount + Tip;
            Status = "ERROR";
            this.Close();
        }
    }
}
