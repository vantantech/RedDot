
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

    public class EnrollFingerPrintVM : INPCBase
    {

     

        private int fingerindex;//indexfinger
        private int count;
        private FingerPrint enrollfingerprint;
       // private List<Fmd> preEnrollmentFmd;
       // private DataResult<Fmd> enrollmentFmd, fmd1, fmd2;
        private bool done = false;
        public bool Done
        {
            get { return done; }
            set
            {
                done = value;
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
            set
            {
                m_currentemployee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }
        Window m_parent;
        public EnrollFingerPrintVM(Window parent, Employee currentemployee)
        {

            enrollfingerprint = new FingerPrint();

            m_currentemployee = currentemployee;
            m_parent = parent;

            StartEnrollmentClicked = new RelayCommand(ExecuteStartEnrollmentClicked, param => this.CanEnroll);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);


        }

        public bool CanEnroll
        {
            get { return !Done; }
        }





        void ExecuteStartEnrollmentClicked(object obj)
        {

            try
            {
                if (enrollfingerprint.InitializeReaders())
                {
                    string result = enrollfingerprint.StartEnrollment();

                    TouchMessageBox.Show(result);
                }
                else
                {
                    TouchMessageBox.Show("Not able to read fingerprint");
                    m_parent.Close();
                }

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }

        }

        public void ExecuteBackClicked(object butt)
        {
            enrollfingerprint.CancelCaptureAndCloseReader();
            m_parent.Close();
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
