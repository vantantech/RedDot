namespace RedDot
{
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Win32;
   
   


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WebCam
    {
        private string m_filename ;
        private string m_path;
       
        public WebCam(string path)
        {
            InitializeComponent();

            InitializeComboBox();

            m_filename = "img_" + Utility.RandomPin(6).ToString() + ".bmp";
            m_path = path;
           
        }

        private void InitializeComboBox()
        {
            comboBox.ItemsSource = webCameraControl.GetVideoCaptureDevices();

            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedItem = comboBox.Items[0];
          
            }            
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            startButton.IsEnabled = e.AddedItems.Count > 0;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var cameraId = (WebCameraId)comboBox.SelectedItem;
            webCameraControl.StartCapture(cameraId);
        }

        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            webCameraControl.StopCapture();
        }

        private void OnImageButtonClick2(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "Bitmap Image|*.bmp" };
            if (dialog.ShowDialog() == true)
            {
                webCameraControl.GetCurrentImage().Save(dialog.FileName);
            
            }
        }

        private void OnImageButtonClick(object sender, RoutedEventArgs e)
        {
           
                webCameraControl.GetCurrentImage().Save(m_path + m_filename);
                if (webCameraControl.IsCapturing) webCameraControl.StopCapture();
                this.Close();
         
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {

            if(webCameraControl.IsCapturing) webCameraControl.StopCapture();
            m_filename = "";
            this.Close();

        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (comboBox.Items.Count == 1)
            {
                var cameraId = (WebCameraId)comboBox.SelectedItem;
                webCameraControl.StartCapture(cameraId);
            }
        }

        public string FileName
        {
            get { return m_filename; }
        }
    }
}
