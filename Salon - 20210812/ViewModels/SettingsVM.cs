using Microsoft.Win32;
using RedDot.DataManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfLocalization;

namespace RedDot
{

    public class LanguageType
    {
        public string Language { get; set; }
        public string LanguageCode { get; set; }
        public string Flag { get; set; }
    }


    public class SettingsVM:INPCBase
    {

        DBConnect _dbconnect = new DBConnect();

        public ICommand SettingClicked { get; set; }

        public ObservableCollection<SettingsTabItem> Tabs { get; set; }

        public int SelectedIndex { get; set; }

        private bool CanExecute = true;


        public List<LanguageType> LanguageList { get; set; }

        public List<ListPair> CreditCardProcessorList { get; set; }

        public string DataBaseName
        {
            get { return GlobalSettings.Instance.DatabaseName; }
            set
            {
                GlobalSettings.Instance.DatabaseName = value;
                NotifyPropertyChanged("DataBaseName");
            }
        }

        public string CreditCardProcessor
        {
            get { return GlobalSettings.Instance.CreditCardProcessor; }
            set
            {
                GlobalSettings.Instance.CreditCardProcessor = value;
                NotifyPropertyChanged("CreditCardProcessor");
            }
        }

        public string ElementExpressURL
        {
            get { return GlobalSettings.Instance.ElementExpressURL; }
            set
            {
                GlobalSettings.Instance.ElementExpressURL = value;
            }
        }

        public string BoltURL
        {
            get { return GlobalSettings.Instance.BoltBaseURL; }
            set
            {
                GlobalSettings.Instance.BoltBaseURL = value;
            }
        }

        public string CardPointeURL
        {
            get { return GlobalSettings.Instance.CardConnectURL; }
            set
            {
                GlobalSettings.Instance.CardConnectURL = value;
            }
        }

        public string APIUsernamePassword
        {
            get { return GlobalSettings.Instance.CardConnectUsernamePassword; }
            set
            {
                GlobalSettings.Instance.CardConnectUsernamePassword = value;
            }
        }

        public string APIAuthorization
        {
            get { return GlobalSettings.Instance.CardConnectAuthorization; }
            set
            {
                GlobalSettings.Instance.CardConnectAuthorization = value;
            }
        }

        public string MerchantID
        {
            get { return GlobalSettings.Instance.MerchantID; }
            set
            {
                GlobalSettings.Instance.MerchantID = value;
            }
        }

        public string IPAddress
        {
            get { return GlobalSettings.Instance.SIPDefaultIPAddress; }
            set
            {
                GlobalSettings.Instance.SIPDefaultIPAddress = value;
            }
        }

        public string SIPPort
        {
            get { return GlobalSettings.Instance.SIPPort; }
            set
            {
                GlobalSettings.Instance.SIPPort = value;
            }
        }

        private string devicelist="";
        public  string DeviceList {
            get
            {
                return devicelist;
            }

            set
            {
                devicelist = value;
                NotifyPropertyChanged("DeviceList");
            }
            }

        public string SelectedLanguage
        {
            get { return GlobalSettings.Instance.LanguageCode; }
            set
            {
                GlobalSettings.Instance.LanguageCode = value;
                NotifyPropertyChanged("SelectedLanguage");


                //Thread.CurrentThread.CurrentCulture = new CultureInfo(value);
                // Thread.CurrentThread.CurrentUICulture = new CultureInfo(value);
                // FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


                var culture = CultureInfo.GetCultureInfo(value);
                // Dispatcher.Thread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                LocalizationManager.UpdateValues();
            }
        }



        private SettingsModel m_settingsmodel;
        private Window m_parent;

        public SettingsVM(Window parent)
        {

            m_parent = parent;
          
         
            SettingClicked = new RelayCommand(ExecuteSettingClicked, param => this.CanExecute);

            m_settingsmodel = new SettingsModel();


            LoadSettings();

            SelectedLanguage = GlobalSettings.Instance.LanguageCode;


            var culture = CultureInfo.GetCultureInfo(SelectedLanguage);
            // Dispatcher.Thread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LocalizationManager.UpdateValues();


            LanguageList = new List<LanguageType>();
            LanguageList.Add(new LanguageType() { Language = "English (US)", LanguageCode = "en-US", Flag = "/media/USA Flag.png" });
            LanguageList.Add(new LanguageType() { Language = "Tiếng Việt", LanguageCode = "vi-VN", Flag = "/media/vietnam.png" });
            LanguageList.Add(new LanguageType() { Language = "Française", LanguageCode = "fr-FR", Flag = "/media/french.png" });

            CreditCardProcessorList = new List<ListPair>();
            CreditCardProcessorList.Add(new ListPair() {Description= "Clover" ,StrValue = "Clover"});
            CreditCardProcessorList.Add(new ListPair() { Description = "Card Connect", StrValue = "CardConnect" });
            CreditCardProcessorList.Add(new ListPair() { Description = "External", StrValue = "External" });
            CreditCardProcessorList.Add(new ListPair() { Description = "VANTIV", StrValue = "VANTIV" });
            CreditCardProcessorList.Add(new ListPair() { Description = "HeartSIP", StrValue = "HeartSIP" });
            CreditCardProcessorList.Add(new ListPair() { Description = "PAX_S300", StrValue = "PAX_S300" });
            CreditCardProcessorList.Add(new ListPair() { Description = "HSIP_ISC250", StrValue = "HSIP_ISC250" });



            DataBases = _dbconnect.GetData("show databases");


          
        }



        bool found = false;
      
        const bool resolveNames = true;

        private DataTable m_databases;
        public DataTable DataBases
        {
            get { return m_databases; }
            set
            {
                m_databases = value;
                NotifyPropertyChanged("DataBases");
            }
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

   

        private void LoadSettings()
        {
            int sel = SelectedIndex;

            Tabs = new ObservableCollection<SettingsTabItem>();
            DataTable cat = m_settingsmodel.GetSettingCategories();
            foreach(DataRow row in cat.Rows)
            {
                SettingsTabItem tabitem = new SettingsTabItem();
                string category = row["category"].ToString();
                tabitem.Header = category;
                tabitem.Settings = m_settingsmodel.GetSettingsByCategory(category);
                Tabs.Add(tabitem);
            }
            NotifyPropertyChanged("Tabs");
            SelectedIndex = sel;
            NotifyPropertyChanged("SelectedIndex");
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

                    case "decimal":

                    case "numeric":
                        AppSetting.Value = GetNumericValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "integer":
                        AppSetting.Value = GetIntegerValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "list":
                        AppSetting.Value = GetListValue(AppSetting.Value,AppSetting.TypeData);
                        AppSetting.Save();
                        break;

                    case "networkdevice":

                        //  AppSetting.Value = GetListValue(AppSetting.Value, DeviceList + "Manual Entry");
                        AppSetting.Value = GetStringValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "color":
                        AppSetting.Value = GetColorValue(AppSetting.Value);
                        AppSetting.Save();
                        break;
                    

                }


                //reload settings to screen
                LoadSettings();

            }catch(Exception e)
            {

                TouchMessageBox.Show("SettingClicked:" + e.Message);
            }
         



        }







        private string GetImageFilePath(string oldvalue)
        {

            Selection sel = new Selection("Choose Image", "Clear Image") { Topmost = true };
            sel.ShowDialog();

            if (sel.Action == "Clear Image") return ""; //clear
            else if (sel.Action == "") return oldvalue.Replace("\\", "\\\\"); //cancel
            


            OpenFileDialog picfile = new OpenFileDialog();
            picfile.DefaultExt = "png";
            picfile.Filter = "All files (*.*)|*.*|PNG Files (*.png)|*.png|JPG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|GIF Files(*.gif)|*.gif";
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


        private string GetColorValue(string oldvalue)
        {

            ColorPicker tp = new ColorPicker(oldvalue);
            Utility.OpenModal(m_parent, tp);
            return tp.ReturnText;
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

            if (tp.Amount == "") tp.Amount = "0";

            return tp.Amount;
        }

        private string GetIntegerValue(string oldvalue)
        {

            NumPad tp = new NumPad("Integer Value", true);
            tp.Amount = oldvalue;
            Utility.OpenModal(m_parent, tp);

            if (tp.Amount == "") tp.Amount = "0";

            return tp.Amount;
        }

        private string GetListValue(string oldvalue, string list)
        {
            List<CustomList> newlist = new List<CustomList>();
            var listarray = list.Split(',');
            foreach(var item in listarray)
            {
                newlist.Add(new CustomList { returnstring = item, description = item });
            }


            ListPad tp = new ListPad("List", newlist);
            tp.ReturnString = oldvalue;
            Utility.OpenModal(m_parent, tp);

            return tp.ReturnString;
        }


    }

    public class SettingsTabItem:INPCBase
    {
        private DataTable m_settings;
        public string Header { get; set; }
        public DataTable Settings
        {
            get { return m_settings;}
            set{
                m_settings = value;
                NotifyPropertyChanged("Settings");
            }
        }
    }
}
