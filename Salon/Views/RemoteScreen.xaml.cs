using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for RemoteScreen.xaml
    /// </summary>
    public partial class RemoteScreen : Window
    {
        public RemoteScreenVM remotescreenvm { get; set; }
        DispatcherTimer timer;
        int ctr = 0;
        int max = 50;
        public RemoteScreen()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += new EventHandler(timer_Tick);

            remotescreenvm = new RemoteScreenVM();
            this.DataContext = remotescreenvm;
        }


        void timer_Tick(object sender, EventArgs e)
        {
            ctr++;
            if (ctr > max)
            {
                ctr = 1;
            }

            // string filename = ((ctr < 10) ? "pack://siteoforigin:,,,/" + "images/Plane0" + ctr + ".jpeg" : "pack://siteoforigin:,,,/" + "images/Plane" + ctr + ".jpeg");
            string filename = ((ctr < 10) ? "c:/reddot/images/ad" + ctr + ".jpg" : "c:/reddot/images/ad" + ctr + ".jpg");
            if (File.Exists(filename))
            {

                PlaySlideShow(filename);
            }
            else
            {
                //resets max once it hits a file not found
                max = ctr;
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctr = 1;


            timer.Start();

        }
        private void PlaySlideShow(string filename)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();


                image.UriSource = new Uri(filename);

                image.EndInit();
                myImage.Source = image;
                myImage.Stretch = Stretch.Uniform;
                progressBar1.Value = ctr;



            }
            catch
            {

            }

        }




    }
}
