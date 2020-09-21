using DPUruNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RedDot
{
    
    public class EnrollFingerPrintVM:INPCBase
    {

        private Reader reader;

        private int fingerindex;//indexfinger
        private int count;
        private List<Fmd> preEnrollmentFmd;
        private DataResult<Fmd> enrollmentFmd, fmd1, fmd2;
        private bool done = false;
        public bool Done
        {
            get { return done; }
            set { done = value;
                NotifyPropertyChanged("Done");
            }
        }


        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                NotifyPropertyChanged("Message");
            }
        }


        private string message1;
        public string Message1
        {
            get { return message1; }
            set
            {
                message1 = value;
                NotifyPropertyChanged("Message1");
            }
        }


        private string message2;
        public string Message2
        {
            get { return message2; }
            set
            {
                message2 = value;
                NotifyPropertyChanged("Message2");
            }
        }
        public ICommand StartEnrollmentClicked { get; set; }
        public ICommand BackClicked { get; set; }

        private Employee m_currentemployee;
        public Employee CurrentEmployee
        {
            get { return m_currentemployee; }
            set { m_currentemployee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }
        Window m_parent;
        public EnrollFingerPrintVM(Window parent, Employee currentemployee)
        {
           m_currentemployee = currentemployee;
            m_parent = parent;

            StartEnrollmentClicked = new RelayCommand(ExecuteStartEnrollmentClicked, param => this.CanEnroll);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
        }

        public bool CanEnroll
        {
            get { return !Done; }
        }


        private bool  InitializeReaders()
        {
            ReaderCollection rc = ReaderCollection.GetReaders();
            if (rc.Count == 0)
            {
                //UpdateEnrollMessage("Fingerprint Reader not found. Please check if reader is plugged in and try again", null);
                TouchMessageBox.Show("Fingerprint Reader not found. Please check if reader is plugged in and try again");
                return false;
            }
            else
            {
                reader = rc[0];
                Constants.ResultCode readersResult = reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                return true;
            }
        }


        void ExecuteStartEnrollmentClicked(object obj)
        {
           
            try
            {
                if (InitializeReaders())
                {
                    Constants.ResultCode result = reader.GetStatus();
                    if (result == Constants.ResultCode.DP_SUCCESS)
                    {
                        StartEnrollment(result);
                    }
                    else
                        TouchMessageBox.Show("Reader status is:" + result);
                }
                else
                    m_parent.Close();
              
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }

        }

        public void ExecuteBackClicked(object butt)
        {
            CancelCaptureAndCloseReader();
            m_parent.Close();
        }

        public void CancelCaptureAndCloseReader()
        {
            if (reader != null)
            {
                reader.CancelCapture();
                reader.Dispose();
            }
        }

        private void StartEnrollment(Constants.ResultCode readerResult)
        {
            fingerindex = 0;
            preEnrollmentFmd = new List<Fmd>();

            CheckReaderStatus();

            if (readerResult == Constants.ResultCode.DP_SUCCESS)
            {
                reader.On_Captured += new Reader.CaptureCallback(reader_On_Captured);
                EnrollmentCapture();
                Message = "Place first finger on Reader.";
            }
            else
            {
                Message = "Could not perform capture. Reader result code :" + readerResult.ToString();
            }

        }

        private void EnrollmentCapture()
        {
            count = 0;
            Constants.ResultCode captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);
            if (captureResult != Constants.ResultCode.DP_SUCCESS)
            {
                TouchMessageBox.Show("CaptureResult: " + captureResult.ToString());
            }
        }


        //Async so all message back needs to be INVOKED
        void reader_On_Captured(CaptureResult capResult)
        {

            if (capResult.Quality == Constants.CaptureQuality.DP_QUALITY_GOOD)
            {
                count++;
                DataResult<Fmd> fmd = FeatureExtraction.CreateFmdFromFid(capResult.Data, Constants.Formats.Fmd.DP_PRE_REGISTRATION);

                // Get view bytes to create bitmap.
                foreach (Fid.Fiv fiv in capResult.Data.Views)
                {
                    //Ask user to press finger to get fingerprint
                    if (fingerindex == 0)
                        UpdateEnrollMessage1(" Count:" + count.ToString());
                     else
                        UpdateEnrollMessage2(" Count:" + count.ToString());

                    //UpdateEnrollImage(Utility.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                    UpdateEnrollImage( Utility.CopyDataToBitmap(fiv.RawImage, fiv.Width, fiv.Height));

                    break;
                }


                //pre enrollment code decides how many times to scan finger before it returns a successful preenrollment
                preEnrollmentFmd.Add(fmd.Data);
                enrollmentFmd = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.DP_REGISTRATION, preEnrollmentFmd);


                //enrollment is success for finger so goes to next
                if (enrollmentFmd.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    if (fingerindex == 0)
                    {
                        fmd1 = enrollmentFmd;
                        fingerindex++;
                        count = 0;
                        preEnrollmentFmd.Clear();
                        UpdateEnrollMessage1("Finger 1 DONE");
                        UpdateEnrollMessage("Place second finger on reader");
                    }
                    else if (fingerindex == 1)
                    {
                        fmd2 = enrollmentFmd;
                        preEnrollmentFmd.Clear();
                        UpdateEnrollMessage2("Finger 2 DONE");

                        UpdateEnrollMessage("User  Successfully Enrolled!");
                        string userid = CurrentEmployee.ID.ToString();

                        CurrentEmployee.FMD1 = Fmd.SerializeXml(fmd1.Data);
                        CurrentEmployee.FMD2 = Fmd.SerializeXml(fmd2.Data);

                        MarkDone();
               
                     
                    }

                }

            }





        }


        private BitmapImage enrollpic;
        public BitmapImage EnrollPic
        {
            get
            {
                return enrollpic;
            }
            set
            {
                enrollpic = value;
                NotifyPropertyChanged("EnrollPic");
            }
        }


 
        private void UpdateEnrollImage(Bitmap image)
        {
            m_parent.Dispatcher.Invoke(new System.Action(() =>
            {

                if (image != null)
                {
                    EnrollPic = ToBitMapImage(image);
                 
                }
            }));

        }

        private void MarkDone()
        {
            m_parent.Dispatcher.Invoke(new System.Action(() =>
            {
                Done = true;
                NotifyPropertyChanged("Done");
                NotifyPropertyChanged("CanEnroll");

                TouchMessageBox.Show("User Successfully Enrolled.");
              

            }));
        }

        private void UpdateEnrollMessage(string text)
        {
            m_parent.Dispatcher.Invoke(new System.Action(() =>
            {
                Message = text;

            }));

        }


        private void UpdateEnrollMessage1(string text)
        {
            m_parent.Dispatcher.Invoke(new System.Action(() =>
            {
                Message1 = text;

            }));

        }

        private void UpdateEnrollMessage2(string text)
        {
            m_parent.Dispatcher.Invoke(new System.Action(() =>
            {
                Message2 = text;

            }));

        }

        public void CheckReaderStatus()
        {
            //If reader is busy, sleep
            if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY)
            {
                Thread.Sleep(50);
            }
            else if ((reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                reader.Calibrate();
            }
            else if ((reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Reader Status - " + reader.Status.Status);
            }
        }

        public BitmapImage ToBitMapImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }
    }
}
