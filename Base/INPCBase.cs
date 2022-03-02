using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using NLog;

namespace RedDotBase
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


        private string m_windowbackgroundcolor;
       public string WindowBackgroundColor
        {
            get { return m_windowbackgroundcolor; }
            set
            {
                m_windowbackgroundcolor = value;
                NotifyPropertyChanged("WindowBackgroundColor");
            }
        }

        #endregion

    }
}
