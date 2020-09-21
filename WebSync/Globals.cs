using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WebSync
{
    public class GlobalSettings
    {
        private  static GlobalSettings _Instance;
        private static DBSettings _dbsettings;
        private Object _remotescreen;
        private Object _notificationscreen;

        public static GlobalSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GlobalSettings();
                    _dbsettings = new DBSettings();
                }
                    
                return _Instance;
            }
        }


     
         
           

     

       


   

 /***
 *    ██╗███╗   ██╗████████╗███████╗ ██████╗ ███████╗██████╗ 
 *    ██║████╗  ██║╚══██╔══╝██╔════╝██╔════╝ ██╔════╝██╔══██╗
 *    ██║██╔██╗ ██║   ██║   █████╗  ██║  ███╗█████╗  ██████╔╝
 *    ██║██║╚██╗██║   ██║   ██╔══╝  ██║   ██║██╔══╝  ██╔══██╗
 *    ██║██║ ╚████║   ██║   ███████╗╚██████╔╝███████╗██║  ██║
 *    ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝
 *                                                           
 */

        /// <summary>
        /// Integer Type Settings
        /// </summary>




        int _portno = 0;
        public int PortNo
        {
            get
            {
                if (_portno == 0) _portno = int.Parse(Utility.GetINIString("Port", "DataBase", "3306"));
                return _portno;
            }
      
        }



        int _webuserid = 0;
        public int WebUserID
        {
            get { if(_webuserid==0) _webuserid =  _dbsettings.IntGetSetting("Webservice","WebUserID","Web User ID","0");
            return _webuserid;
            }
            
        }



        int _websynccheckinterval = 0;
        public int WebSyncCheckInterval
        {
            get { if(_websynccheckinterval ==0) _websynccheckinterval =  _dbsettings.IntGetSetting("Webservice", "WebSyncCheckInterval", "Web Sync Check Interval", "5");
            return _websynccheckinterval;
            }
           
        }











 /***
 *    ██████╗ ███████╗ ██████╗██╗███╗   ███╗ █████╗ ██╗     
 *    ██╔══██╗██╔════╝██╔════╝██║████╗ ████║██╔══██╗██║     
 *    ██║  ██║█████╗  ██║     ██║██╔████╔██║███████║██║     
 *    ██║  ██║██╔══╝  ██║     ██║██║╚██╔╝██║██╔══██║██║     
 *    ██████╔╝███████╗╚██████╗██║██║ ╚═╝ ██║██║  ██║███████╗
 *    ╚═════╝ ╚══════╝ ╚═════╝╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝
 *                                                          
 */








        /// <summary>
        /// Decimal Type settings
        /// </summary>
   
    


/*

        ██████╗  ██████╗  ██████╗ ██╗     ███████╗ █████╗ ███╗   ██╗
        ██╔══██╗██╔═══██╗██╔═══██╗██║     ██╔════╝██╔══██╗████╗  ██║
        ██████╔╝██║   ██║██║   ██║██║     █████╗  ███████║██╔██╗ ██║
        ██╔══██╗██║   ██║██║   ██║██║     ██╔══╝  ██╔══██║██║╚██╗██║
        ██████╔╝╚██████╔╝╚██████╔╝███████╗███████╗██║  ██║██║ ╚████║
        ╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═══╝
                                                            

        */

        /// <summary>
        /// Bool Type Settings
        /// </summary>
        /// 

  





/*
        ███████╗████████╗██████╗ ██╗███╗   ██╗ ██████╗ ███████╗
        ██╔════╝╚══██╔══╝██╔══██╗██║████╗  ██║██╔════╝ ██╔════╝
        ███████╗   ██║   ██████╔╝██║██╔██╗ ██║██║  ███╗███████╗
        ╚════██║   ██║   ██╔══██╗██║██║╚██╗██║██║   ██║╚════██║
        ███████║   ██║   ██║  ██║██║██║ ╚████║╚██████╔╝███████║
        ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚═╝  ╚═══╝ ╚═════╝ ╚══════╝
                                                       
*/

        /// <summary>
        /// String settings --------------------------------------------------------- STRINGS -----------------------------------------------------------------
        /// </summary>
        /// 



        string _databaseserver = "";
        public string DatabaseServer
        {
            get
            {
                if (_databaseserver == "") _databaseserver = Utility.GetINIString("Server", "DataBase", "localhost");

                return _databaseserver;
            }
       
        }


        public string BackupDirectory
        {
            get { return _dbsettings.StringGetSetting("System", "BackupDirectory", "Backup Directory", ""); }
            set { _dbsettings.SaveSetting("BackupDirectory", value.ToString()); }
        }

  

        string _databasename = "";
        public string DatabaseName
        {
            get
            {
                if (_databasename == "")
                {

                    _databasename = Utility.GetINIString("Database", "DataBase", "salon");
                }


                return _databasename;
            }
     
        }


        string _websyncupdatetime = "";
        public string WebSyncUpdateTime
        {
            get
            {
                if (_websyncupdatetime == "") _websyncupdatetime = _dbsettings.StringGetSetting("Webservice", "WebSyncUpdateTime", "WebSync Update Time", "");
                return _websyncupdatetime;
            }


        }
    }
}
