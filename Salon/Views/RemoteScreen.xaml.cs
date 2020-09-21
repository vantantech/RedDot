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
using System.Windows.Threading;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for RemoteScreen.xaml
    /// </summary>
    public partial class RemoteScreen : Window
    {
        DispatcherTimer timer;
        int ctr = 0;
        public RemoteScreen()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += new EventHandler(timer_Tick);
        }


        void timer_Tick(object sender, EventArgs e)
        {
            ctr++;
            if (ctr > 21)
            {
                ctr = 1;
            }
            PlaySlideShow(ctr);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctr = 1;
            PlaySlideShow(ctr);
            timer.Start();

        }
        private void PlaySlideShow(int ctr)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            string filename = ((ctr < 10) ? "pack://siteoforigin:,,,/" + "images/Plane0" + ctr + ".jpeg" : "pack://siteoforigin:,,,/" + "images/Plane" + ctr + ".jpeg");
           image.UriSource = new Uri(filename);
            
            image.EndInit();
            myImage.Source = image;
            myImage.Stretch = Stretch.Uniform;
            progressBar1.Value = ctr;
        }


 
  
    }
}
