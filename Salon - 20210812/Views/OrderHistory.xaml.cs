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
using System.Data;


namespace RedDot
{
    /// <summary>
    /// Interaction logic for OrderHistory.xaml
    /// </summary>
    public partial class OrderHistory : Window
    {

 
        private OrderHistoryVM orderhistoryvm;

        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private double limit;
        public OrderHistory(SecurityModel security, HeartPOS ccp)
        {
            InitializeComponent();
             orderhistoryvm = new OrderHistoryVM(this,security,ccp);
            this.DataContext = orderhistoryvm;

      


           // Closing += (s, e) => ViewModelCreator.Cleanup();
         }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
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

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == System.Windows.Input.Cursors.ScrollAll)
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

    

    }
}
