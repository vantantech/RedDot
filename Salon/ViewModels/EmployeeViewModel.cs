using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class EmployeeViewModel : INPCBase
    {

        private Employee _employee;
        public ICommand DeleteEmployeeClicked { get; set; }

       public ICommand PictureClicked { get; set; }
       public ICommand WebCamClicked { get; set; }
        public ICommand MSRSetClicked { get; set; }
        public ICommand EnrollFingerPrintClicked { get; set; }

        public List<PayType> PayTypes { get; set; }
        public List<SecurityType> SecurityLevels { get; set; }
        private bool CanExecute = true;
        private bool CanDelete = false;
        private Window _parent;

        private SecurityModel _security;

        public Visibility VisibleAdmin { get; set; }

      

        public EmployeeViewModel(Window parent, SecurityModel security, Employee employee, bool candelete, bool admin)
        {
            _employee = employee;
            _parent = parent;
            _security = security;
            CanDelete = candelete;
         

            DeleteEmployeeClicked = new RelayCommand(ExecuteDeleteEmployeeClicked, param => this.CanDeleteEmployee);

            WebCamClicked = new RelayCommand(ExecuteWebCamClicked, param => this.CanExecute);

            PictureClicked = new RelayCommand(ExecutePictureClicked, param => this.CanExecute);
            MSRSetClicked = new RelayCommand(ExecuteMSRSetClicked, param => this.CanExecute);
            EnrollFingerPrintClicked = new RelayCommand(ExecuteEnrollFingerPrintClicked, param => this.CanExecute);

            if (admin) VisibleAdmin = Visibility.Visible;
            else VisibleAdmin = Visibility.Collapsed;

            PayTypes = new List<PayType>();
            PayTypes.Add(new PayType() { Name = "commission" });
            PayTypes.Add(new PayType() { Name = "salary" });
            PayTypes.Add(new PayType() { Name = "comission/salary" });


            SecurityLevels = new List<SecurityType>();
            SecurityLevels.Add(new SecurityType() { Name = "No Access",Color="White", Value = 0 });
            SecurityLevels.Add(new SecurityType() { Name = "Employee",Color="Yellow", Value = 10 });
            SecurityLevels.Add(new SecurityType() { Name = "Manager",Color="Orange", Value = 50 });
            SecurityLevels.Add(new SecurityType() { Name = "Admin/Owner",Color="Red", Value = 100 });



            employee.Refresh();
         

        }

  
  public bool CanDeleteEmployee
        {
            get {
                if (_employee == null) return false;

                if (this.CanDelete)
                {

                     SalesModel _salesmodel;
                     _salesmodel = new SalesModel(null);
                    if (_salesmodel.GetSalonSalesCount(_employee.ID) > 0)
                        return false;
                    else
                    return true;

                }
                else return false;
                
                
            
            
            }

        }


        public Employee CurrentEmployee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }





        public void ExecuteMSRSetClicked(object button)
        {
            CardScanner crd = new CardScanner();
            Utility.OpenModal(_parent, crd);
            if (crd.CardNumber != "")
            {
                CurrentEmployee.MSRCard = crd.CardNumber;

            }

        }
        public void ExecutePictureClicked(object button)
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "*.*";
            picfile.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                CurrentEmployee.ImageSrc = selectedPath.Replace("\\", "\\\\");
            }
            // Utility.PlaySound();

        }


        public void ExecuteEnrollFingerPrintClicked(object obj)
        {

            if (_security.WindowAccess("BackOffice"))
            {

                FingerPrint fingerPrint = new FingerPrint();
                fingerPrint.StartEnrollment();

                EnrollFingerPrint ep = new EnrollFingerPrint(CurrentEmployee);
                Utility.OpenModal(_parent, ep);

                //Loads finger print database into memory
               // GlobalSettings.Instance.LoadAllFmdsUserIDs();
            }


        }



        public void ExecuteWebCamClicked(object button)
        {
           
            
            /*

            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();


            WebCam webcam = new WebCam(AppPath + "Images\\");

            webcam.ShowDialog();

            if(webcam.FileName != "")
            {
                selectedPath = "Images\\" + webcam.FileName;
                selectedPath = selectedPath.ToUpper();
                CurrentEmployee.ImageSrc = selectedPath.Replace("\\", "\\\\");
            }
      

            webcam.Close();

            */

        }


        public void ExecuteDeleteEmployeeClicked(object button)
        {

            if (_employee.DeleteEmployee())
            {
                TouchMessageBox.Show("Deleted Successfully");

                CurrentEmployee = null;
                _parent.Close();
            }
            else TouchMessageBox.Show("Delete Fail");


        }

  

    }


    public class PayType
    {
        public string Name { get; set; }
    }

    public class SecurityType
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int Value { get; set; }
    }
}
