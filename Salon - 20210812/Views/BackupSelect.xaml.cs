using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for BackupSelect.xaml
    /// </summary>
    public partial class BackupSelect : Window
    {

        public ObservableCollection<DriveInfo> DriveList { get; set; }

        public string ChosenDrive;
        public string Preset { get; set; }
        public BackupSelect(string preset)
        {

            this.DataContext = this;
            Preset = preset;

           DriveList = new ObservableCollection<DriveInfo>();
           DriveInfo[] allDrives = DriveInfo.GetDrives();


          foreach (DriveInfo d in allDrives)
          {
                DriveList.Add(d);
          }
            InitializeComponent();
            
        }
        private void DriveClick(object sender, RoutedEventArgs e)
        {
            Button btt = sender as Button;

            ChosenDrive = btt.Tag.ToString();
            this.Close();
        }
        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
