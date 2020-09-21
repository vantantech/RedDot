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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for ReceiptControl.xaml
    /// </summary>
    public partial class RetailReceiptControl : UserControl
    {
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private double limit;
        public RetailReceiptControl()
        {
            InitializeComponent();
        }

        private void ListItemScrollDown_Click(object sender, RoutedEventArgs e)
        {


            ScrollviewerLineItem.PageDown();
        }

        private void ListItemScrollUp_Click(object sender, RoutedEventArgs e)
        {


            ScrollviewerLineItem.PageUp();
        }



        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {

            if (this.ScrollviewerLineItem.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollviewerLineItem.HorizontalOffset;
                scrollStartOffset.Y = ScrollviewerLineItem.VerticalOffset;
                limit = ScrollviewerLineItem.TransformToAncestor(this).Transform(new Point(0, 0)).X + ScrollviewerLineItem.ViewportWidth - 40;

                // Update the cursor if can scroll or not.
                if (scrollStartPoint.X < limit)
                    this.Cursor = (ScrollviewerLineItem.ExtentWidth >
                       ScrollviewerLineItem.ViewportWidth) ||
                        (ScrollviewerLineItem.ExtentHeight >
                        ScrollviewerLineItem.ViewportHeight) ?
                       Cursors.ScrollAll : Cursors.Arrow;

            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == System.Windows.Input.Cursors.ScrollAll)
            {


                if (ScrollviewerLineItem.IsMouseOver)
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
                    // ScrollviewerLineItem.ScrollToHorizontalOffset(    this.scrollStartOffset.X + delta.X);
                    ScrollviewerLineItem.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
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
    }
}
