using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfLocalization;

namespace RedDot
{




    public class SettingsVM : INPCBase
    {

        DBConnect _dbconnect = new DBConnect();

        public ICommand SettingClicked { get; set; }

        public ObservableCollection<SettingsTabItem> Tabs { get; set; }

        public int SelectedIndex { get; set; }

        private bool CanExecute = true;


        public List<LanguageType> LanguageList { get; set; }

        public string DataBaseName
        {
            get { return GlobalSettings.Instance.DatabaseName; }
            set
            {
                GlobalSettings.Instance.DatabaseName = value;
                NotifyPropertyChanged("DataBaseName");
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


            DataBases = _dbconnect.GetData("show databases");


        }

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


        private void LoadSettings()
        {
            int sel = SelectedIndex;

            Tabs = new ObservableCollection<SettingsTabItem>();
            DataTable cat = m_settingsmodel.GetSettingCategories();
            foreach (DataRow row in cat.Rows)
            {
                SettingsTabItem tabitem = new SettingsTabItem();
                string category = row["category"].ToString();
                tabitem.Header = category;
                tabitem.ImgSrc = "/media/star.png";
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


                switch (AppSetting.Type)
                {

                    case "image":
                        AppSetting.Value = Utility.GetImageFilePath(AppSetting.Value);
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
                        AppSetting.Value = GetDecimalValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "integer":
                        AppSetting.Value = GetIntegerValue(AppSetting.Value);
                        AppSetting.Save();
                        break;

                    case "list":
                        AppSetting.Value = GetListValue(AppSetting.Value, AppSetting.TypeData);
                        AppSetting.Save();
                        break;
                    case "color":
                        AppSetting.Value = GetColorValue(AppSetting.Value);
                        AppSetting.Save();
                        break;
                    case "printer":
                        AppSetting.Value = GetPrinterValue(AppSetting.Value);
                        AppSetting.Save();
                        break;
                }


                //reload settings to screen
                LoadSettings();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("SettingClicked:" + e.Message);
            }




        }



        private string GetPrinterValue(string oldvalue)
        {

            PrinterPicker tp = new PrinterPicker(oldvalue);
            Utility.OpenModal(m_parent, tp);
            return tp.ReturnText;
        }

        private string GetColorValue(string oldvalue)
        {

            ColorPicker tp = new ColorPicker(oldvalue);
            Utility.OpenModal(m_parent, tp);
            return tp.ReturnText;
        }

       



        private string GetBoolValue(string oldvalue)
        {
            Selection sel = new Selection("True", "False");
            Utility.OpenModal(m_parent, sel);
            if (sel.Action == "") return oldvalue;
            else return sel.Action;
        }




        private string GetStringValue(string oldvalue)
        {

            TextPad tp = new TextPad("Enter text value:", oldvalue);
            Utility.OpenModal(m_parent, tp);
            return tp.ReturnText.Replace("\\", "\\\\");
        }


        private string GetDecimalValue(string oldvalue)
        {

            NumPad tp = new NumPad("Decimal Value", false, false);
            tp.Amount = oldvalue;
            Utility.OpenModal(m_parent, tp);

            if (tp.Amount == "") tp.Amount = "0";

            return tp.Amount;
        }

        private string GetIntegerValue(string oldvalue)
        {

            NumPad tp = new NumPad("Integer Value", true, false);
            tp.Amount = oldvalue;
            Utility.OpenModal(m_parent, tp);

            if (tp.Amount == "") tp.Amount = "0";

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

    public class SettingsTabItem : INPCBase
    {
        private DataTable m_settings;
        public string Header { get; set; }
        public string ImgSrc { get; set; }
        public DataTable Settings
        {
            get { return m_settings; }
            set
            {
                m_settings = value;
                NotifyPropertyChanged("Settings");
            }
        }
    }
}
