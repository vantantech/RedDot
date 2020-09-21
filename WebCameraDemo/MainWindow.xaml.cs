﻿namespace RedDot
{
    using System.Windows;
    using System.Windows.Controls;

    using Microsoft.Win32;


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeComboBox();

          
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
           
                webCameraControl.GetCurrentImage().Save("c:\\temp\\test.bmp");
         
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (comboBox.Items.Count == 1)
            {
                var cameraId = (WebCameraId)comboBox.SelectedItem;
                webCameraControl.StartCapture(cameraId);
            }
        }
    }
}
