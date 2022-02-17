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
using System.Text.RegularExpressions;





namespace RedDot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SalonSales : Window
    {


        private SalonSalesVM salesViewModel;

        private string RawString;
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private double limit;
        private bool found = false;
        private string lastkey = "";

        private string generic_string = "";

        public SalonSales( int salesid)
        {
            InitializeComponent();
            salesViewModel = new SalonSalesVM(this , salesid);
            this.DataContext = salesViewModel; //lik to Sales View Model





        }




        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewer0.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer0.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer0.VerticalOffset;
                limit = ScrollViewer0.ViewportWidth;


                // Update the cursor if can scroll or not.
                this.Cursor = (ScrollViewer0.ExtentWidth >
                   ScrollViewer0.ViewportWidth) ||
                    (ScrollViewer0.ExtentHeight >
                    ScrollViewer0.ViewportHeight) ?
                   Cursors.ScrollAll : Cursors.Arrow;

                //this.CaptureMouse();
            }

            if (ScrollViewer1.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer1.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer1.VerticalOffset;
                limit = ScrollViewer1.ViewportWidth;


                // Update the cursor if can scroll or not.
                this.Cursor = (ScrollViewer1.ExtentWidth >
                   ScrollViewer1.ViewportWidth) ||
                    (ScrollViewer1.ExtentHeight >
                    ScrollViewer1.ViewportHeight) ?
                   Cursors.ScrollAll : Cursors.Arrow;

                //this.CaptureMouse();
            }





            base.OnPreviewMouseDown(e);
        }


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {




            //start of generic product processing
            if (e.Text == "\r")
            {
                if (generic_string.Length == 12)
                {
                    salesViewModel.AddItemByBarCode(generic_string);
                }
                generic_string = "";
                RawString = "";
            }
            else
                generic_string += e.Text;




        }


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == System.Windows.Input.Cursors.ScrollAll)
            {
                if (ScrollViewer0.IsMouseOver)
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

                    // Scroll to the new position

                    // ScrollViewer1.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    if (point.X < limit)
                        ScrollViewer0.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }



                if (ScrollViewer1.IsMouseOver)
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

                    // Scroll to the new position

                    // ScrollViewer1.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    if (point.X < limit)
                        ScrollViewer1.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }
            }




            base.OnPreviewMouseMove(e);
        }


        private void GetNewPosition(ScrollViewer scrollviewer, MouseEventArgs e)
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
            // ScrollViewer2.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
            if (point.X < limit) scrollviewer.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);

        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
            this.ReleaseMouseCapture();

            base.OnPreviewMouseUp(e);
        }

        private void ScrollDown_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageDown();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset + 20);
        }

        private void ScrollUp_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageUp();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset - 20);
        }




    }
}
