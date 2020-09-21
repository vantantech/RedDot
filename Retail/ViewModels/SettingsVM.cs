using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class SettingsVM:INPCBase
    {
        public ICommand MainBackgroundClicked { get; set; }
        public ICommand MainBackgroundClearClicked { get; set; }
        public ICommand LogoClicked { get; set; }

        public ICommand SettingClicked { get; set; }


        public ICommand LogoClearClicked { get; set; }
        private bool CanExecute = true;

        private DataTable m_systemsettings;
        private DataTable m_ticketsettings;
        private DataTable m_applicationsettings;

        private SettingsModel m_settingsmodel;
        private Window m_parent;

        public SettingsVM(Window parent)
        {

            m_parent = parent;

            MainBackgroundClicked = new RelayCommand(ExecuteMainBackgroundClicked, param => this.CanExecute);
            MainBackgroundClearClicked = new RelayCommand(ExecuteMainBackgroundClearClicked, param => this.CanExecute);
            LogoClicked = new RelayCommand(ExecuteLogoClicked, param => this.CanExecute);
            LogoClearClicked = new RelayCommand(ExecuteLogoClearClicked, param => this.CanExecute);
            SettingClicked = new RelayCommand(ExecuteSettingClicked, param => this.CanExecute);

            m_settingsmodel = new SettingsModel();



            SystemSettings = m_settingsmodel.GetSettingsByCategory("System");
            ApplicationSettings = m_settingsmodel.GetSettingsByCategory("Application");
            TicketSettings = m_settingsmodel.GetSettingsByCategory("Ticket");
        }
        public string StoreLogo
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; }
        }

        public string MainBackgroundImage
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.MainBackgroundImage; }
            set
            {
                GlobalSettings.Instance.MainBackgroundImage = value;
                NotifyPropertyChanged("MainBackgroundImage");
            }
        }

        public string LogoImage
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; }
            set
            {
                GlobalSettings.Instance.StoreLogo = value;
                NotifyPropertyChanged("LogoImage");
            }
        }

        public DataTable SystemSettings
        {
            get { return m_systemsettings; }
            set { m_systemsettings = value;
            NotifyPropertyChanged("SystemSettings");
            }
        }


        public DataTable ApplicationSettings
        {
            get { return m_applicationsettings; }
            set
            {
                m_applicationsettings = value;
                NotifyPropertyChanged("ApplicationSettings");
            }
        }

        public DataTable TicketSettings
        {
            get { return m_ticketsettings; }
            set
            {
                m_ticketsettings = value;
                NotifyPropertyChanged("TicketSettings");
            }
        }

        public void ExecuteMainBackgroundClearClicked(object salesid)
        {
            MainBackgroundImage = "";
        }

        public void ExecuteLogoClearClicked(object salesid)
        {
            LogoImage = "";
        }
        public void ExecuteMainBackgroundClicked(object salesid)
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "jpg";
            picfile.Filter = "PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                MainBackgroundImage = selectedPath.Replace("\\", "\\\\");
            }


        }

        public void ExecuteLogoClicked(object salesid)
        {
            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "jpg";
            picfile.Filter = "PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                LogoImage = selectedPath.Replace("\\", "\\\\");
            }
        }


        public void ExecuteSettingClicked(object setid)
        {
            int id;

            try
            {
                id = (int)setid;

                Setting AppSetting = new Setting(id);


                switch(AppSetting.Type)
                {

                    case "image":
                        AppSetting.Value = GetImageFilePath(AppSetting.Value);
                        AppSetting.Save();
                        break;


                    case "bool":
                        AppSetting.Value = GetBoolValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "string":
                        AppSetting.Value = GetStringValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "numeric":
                        AppSetting.Value = GetNumericValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "list":
                        AppSetting.Value = GetListValue(AppSetting.Value, AppSetting.TypeData);
                        AppSetting.Save();
                        break;

                }


                //reload settings to screen

                SystemSettings = m_settingsmodel.GetSettingsByCategory("System");
                ApplicationSettings = m_settingsmodel.GetSettingsByCategory("Application");
                TicketSettings = m_settingsmodel.GetSettingsByCategory("Ticket");

               // TouchMessage.Show("SettingClicked:" + AppSetting.Value);

            }catch(Exception e)
            {

                TouchMessage.Show("SettingClicked:" + e.Message);
            }
         



        }







        private string GetImageFilePath(string oldvalue)
        {


            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "png";
            picfile.Filter = "PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|All files (*.*)|*.*";
            string AppPath;
            string selectedPath;

            AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
            AppPath = AppPath.ToUpper();

            if (picfile.ShowDialog() == true)
            {
                selectedPath = picfile.FileName;
                selectedPath = selectedPath.ToUpper();
                selectedPath = selectedPath.Replace(AppPath, "");

                return selectedPath.Replace("\\", "\\\\");
            }
            else return oldvalue.Replace("\\", "\\\\");

        }



        private string GetBoolValue(string oldvalue)
        {
            Selection sel = new Selection("True", "False");
            Utility.OpenModal(m_parent, sel);
            if (sel.Action == "") return oldvalue;
            else   return sel.Action;
        }




        private string GetStringValue(string oldvalue)
        {

            TextPad tp = new TextPad("Enter text value:", oldvalue);
            Utility.OpenModal(m_parent, tp);
            return tp.ReturnText.Replace("\\", "\\\\"); 
        }


        private string GetNumericValue(string oldvalue)
        {

            NumPad tp = new NumPad("Numeric Value",false);
            tp.Amount = oldvalue;
            Utility.OpenModal(m_parent, tp);
            
            return tp.Amount;
        }

        private string GetListValue(string oldvalue, string list)
        {
            List<CustomList> newlist = new List<CustomList>();
            var listarray = list.Split(',');
            foreach (var item in listarray)
            {
                newlist.Add(new CustomList { returnstring = item, description = item });
            }


            ListPad tp = new ListPad("List", newlist);
            tp.ReturnString = oldvalue;
            Utility.OpenModal(m_parent, tp);

            return tp.ReturnString;
        }
    }


    public class CustomList
    {
        public int id { get; set; }
        public string description { get; set; }
        public string returnstring { get; set; }

    }
}
