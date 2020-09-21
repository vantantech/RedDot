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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;
using System.Windows.Controls.Primitives;

using System.Windows.Media.Effects;
using RedDot;
using System.Globalization;
using WpfLocalization;





namespace RedDot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RetailSales : Window
    {

      

        private SalesVM salesViewModel;

        //private bool _loaded = false;

        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private double limit;
      


        public RetailSales(Security security, int id = 0)
        {
            InitializeComponent();
            salesViewModel = new SalesVM(this, security, id);
            this.DataContext = salesViewModel; //link to Sales View Model
          //  _loaded = true;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
        private void ScrollUp_Click(object sender, RoutedEventArgs e)
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(lbCategory, 0) as Decorator;
            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            scrollViewer.PageUp();
        }

        private void ScrollDown_Click(object sender, RoutedEventArgs e)
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(lbCategory, 0) as Decorator;
            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            scrollViewer.PageDown();
        }
        */
        private void ScrollUp_Click2(object sender, RoutedEventArgs e)
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(lbProduct, 0) as Decorator;
            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            scrollViewer.PageUp();
        }

        private void ScrollDown_Click2(object sender, RoutedEventArgs e)
        {
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(lbProduct, 0) as Decorator;
            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            scrollViewer.PageDown();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

   





        /// <summary>
        /// Finger Scrolling code
        /// </summary>
        /// <param name="e"></param>

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewerCat.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewerCat.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewerCat.VerticalOffset;
                limit = ScrollViewerCat.ViewportWidth - 40;

                // Update the cursor if can scroll or not.
                if (scrollStartPoint.X < limit)
                    this.Cursor = (ScrollViewerCat.ExtentWidth >
                       ScrollViewerCat.ViewportWidth) ||
                        (ScrollViewerCat.ExtentHeight >
                        ScrollViewerCat.ViewportHeight) ?
                       Cursors.ScrollAll : Cursors.Arrow;

            }

            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == System.Windows.Input.Cursors.ScrollAll)
            {
                if (ScrollViewerCat.IsMouseOver)
                {
                    // Get the new scroll position.
                    Point point = e.GetPosition(this);

                    // Determine the new amount to scroll.
                    Point delta = new Point(
                        (point.X > this.scrollStartPoint.X) ?
                            -(point.X - this.scrollStartPoint.X) :
                            (this.scrollStartPoint.X - point.X),

                        (point.Y > this.scrollStartPoint.Y) ?
                            -(point.Y - this.scrollStartPoint.Y) :
                            (this.scrollStartPoint.Y - point.Y));

                    // Scroll to the new position.
                    // ScrollViewerCat.ScrollToHorizontalOffset( this.scrollStartOffset.X + delta.X);
                    if (point.X < limit) ScrollViewerCat.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }



            }


            base.OnPreviewMouseMove(e);
        }


        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
            this.ReleaseMouseCapture();

            base.OnPreviewMouseUp(e);
        }


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {

            //   RawString = RawString + e.Text;

            //  if (e.Text == "?") trackcount++;

            //  if (e.Text == "\r") ProcessInput(RawString);

            //  this.Title = RawString;

        }

  




    }
}
