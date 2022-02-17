using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NLog;

namespace RedDot
{
    public class INPCBase : INotifyPropertyChanged
    {
        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;
        protected static Logger logger = LogManager.GetCurrentClassLogger();


        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

       public string WindowBackgroundColor
        {
            get { return GlobalSettings.Instance.WindowBackgroundColor; }
        }

        #endregion

    }
}
