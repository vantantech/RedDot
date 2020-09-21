using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Ionic.Zip;
using System.Windows;
using RedDotDependents;

namespace RedDot
{
    class UpdateViewModel:INPCBase
    {
        string m_message;
        bool _uptodate = false;
        public const int DefaultCheckInterval = 900; // 900s == 15 min
        public const int FirstCheckDelay = 15;

       



        /// <summary>
        /// The default configuration file
        /// </summary>


        public ICommand UpdateClicked { get; set; }

        public UpdateViewModel()
        {


          

            UpdateClicked = new RelayCommand(ExecuteUpdateClicked, param => true);

            Message = "Checking for updates..." + (char)13;


            if (Updater.CheckForUpdates())
            {
                Message = Message + "Updated version found." + (char)13;
                _uptodate = false;
            }else
            {
                Message = "Software is up to date .. ";
                _uptodate = true;
            }
        }

        public string Message
        {
            get { return m_message; }
            set { 
                m_message = value;
            NotifyPropertyChanged("Message");
            }
        }
      
         public void ExecuteUpdateClicked(object obj_prodid)
        {
           

            if (!_uptodate)
            {
                Message = "Updating.... ";
                Updater.GetUpdates();
               
            }
            else
            {
                Message = "Software is up to date .. ";
            }
        }

   

    }
}
