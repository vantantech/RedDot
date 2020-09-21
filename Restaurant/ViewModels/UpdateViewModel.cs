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

namespace RedDot
{
    class UpdateViewModel:INPCBase
    {
        string m_message;
        bool _uptodate = false;
        public const int DefaultCheckInterval = 900; // 900s == 15 min
        public const int FirstCheckDelay = 15;

        #region Fields
     
        private volatile bool _updating;
        private readonly Manifest _localConfig;
        private Manifest _remoteConfig;
        private readonly FileInfo _localConfigFile;
        #endregion



        /// <summary>
        /// The default configuration file
        /// </summary>
        public const string DefaultConfigFile = "update.xml";

        public const string WorkPath = "work";

        public ICommand UpdateClicked { get; set; }

        public UpdateViewModel()
        {
            FileInfo configFile = new FileInfo(DefaultConfigFile);
            Log.Debug = true;
            Log.Console = true;

            _localConfigFile = configFile;
            Log.Write("Loaded.");
            Log.Write("Initializing using file '{0}'.", configFile.FullName);
            if (!configFile.Exists)
            {
                Log.Write("Config file '{0}' does not exist, stopping.", configFile.Name);
                return;
            }

            string data = File.ReadAllText(configFile.FullName);
            this._localConfig = new Manifest(data);



            UpdateClicked = new RelayCommand(ExecuteUpdateClicked, param => true);

            Message = "Checking for updates..." + (char)13;


          
            // updater.StartMonitoring();
            if (Check())
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
                Update();
               
            }
            else
            {
                Message = "Software is up to date .. ";
            }
        }

         private bool Check()
         {

             Message = Message + "Checking for updates..." + (char)13;

             if (_updating)
             {
                 Log.Write("Updater is already updating.");
                 Log.Write("Check ending.");
                 Message = Message + "Updater is already updating." + (char)13;
             }
             var remoteUri = new Uri(this._localConfig.RemoteConfigUri);

             Log.Write("Fetching '{0}'.", remoteUri.AbsoluteUri);
             Message = Message + "Fetching :" + remoteUri.AbsoluteUri + (char)13;

             var http = new Fetch { Retries = 5, RetrySleep = 30000, Timeout = 30000 };
             http.Load(remoteUri.AbsoluteUri);
             if (!http.Success)
             {
                 Log.Write("Fetch error: {0}", http.Response.StatusDescription);
                 this._remoteConfig = null;
                 MessageBox.Show("Not able to connect to VTT Update site");
                 return false;
             }

             string data = Encoding.UTF8.GetString(http.ResponseData);
             this._remoteConfig = new Manifest(data);

             if (this._remoteConfig == null) return false;

             if (this._localConfig.SecurityToken != this._remoteConfig.SecurityToken)
             {
                 Log.Write("Security token mismatch.");
                 MessageBox.Show("Security token mismatch.");
   
                 return false;
             }
             Log.Write("Remote config is valid.");
             Log.Write("Local version is  {0}.", this._localConfig.Version);
             Log.Write("Remote version is {0}.", this._remoteConfig.Version);

             if (this._remoteConfig.Version == this._localConfig.Version)
             {
                 Log.Write("Versions are the same.");
                 Log.Write("Check ending.");
       
                 return false;
             }


             if (this._remoteConfig.Version < this._localConfig.Version)
             {
                 Log.Write("Remote version is older. That's weird.");
                 Log.Write("Check ending.");
       
                 return false;
             }


             Log.Write("Remote version is newer.");
             Message = Message + "Remote version is newer..." + (char)13;
             return true;

         }

         private void Update()
         {


             _updating = true;
             Log.Write("Updating '{0}' files.", this._remoteConfig.Payloads.Length);
             Message = Message + "Updating " + this._remoteConfig.Payloads.Length + " files" + (char)13;
             Thread.Sleep(300);

             // Clean up failed attempts.
             if (Directory.Exists(WorkPath))
             {
                 Log.Write("WARNING: Work directory already exists.");
                 Message = Message + "WARNING: Work directory already exists." + (char)13;
                 Thread.Sleep(300);


                 try { Directory.Delete(WorkPath, true); }
                 catch (IOException)
                 {
                     Log.Write("Cannot delete open directory '{0}'.", WorkPath);
                     Message = Message + "Cannot delete open directory..." + (char)13;
                     return;
                 }
             }

             Directory.CreateDirectory(WorkPath);

             // Download files in manifest.
             foreach (string update in this._remoteConfig.Payloads)
             {
                 Log.Write("Fetching '{0}'.", update);
                 Message = Message + "Fetching : " + update + (char)13;
                 Thread.Sleep(300);


                 var url = this._remoteConfig.BaseUri + update;
                 var file = Fetch.Get(url);
                 if (file == null)
                 {
                     Log.Write("Fetch failed.");
                     Message = Message + "Fetch failed." + (char)13;
                     return;
                 }
                 var info = new FileInfo(Path.Combine(WorkPath, update));
                 Directory.CreateDirectory(info.DirectoryName);
                 File.WriteAllBytes(Path.Combine(WorkPath, update), file);

                 // Unzip
                 if (Regex.IsMatch(update, @"\.zip"))
                 {
                     try
                     {
                         var zipfile = Path.Combine(WorkPath, update);
                         using (var zip = ZipFile.Read(zipfile))
                             zip.ExtractAll(WorkPath, ExtractExistingFileAction.Throw);
                         File.Delete(zipfile);
                     }
                     catch (Exception ex)
                     {
                         Log.Write("Unpack failed: {0}", ex.Message);
                         Message = Message + "Unpack failed:" + ex.Message +  (char)13;
                         Thread.Sleep(300);
                         return;
                     }
                 }
             }

             // Change the currently running executable so it can be overwritten.
             Process thisprocess = Process.GetCurrentProcess();
             string me = thisprocess.MainModule.FileName;
             string bak = me + ".bak";
             Log.Write("Renaming running process to '{0}'.", bak);
             Message = Message + "Renaming running process to: " + bak +  (char)13;
             Thread.Sleep(300);


             if (File.Exists(bak))
                 File.Delete(bak);
             File.Move(me, bak);
             File.Copy(bak, me);

             // Write out the new manifest.
             _remoteConfig.Write(Path.Combine(WorkPath, _localConfigFile.Name));

             // Copy everything.
             var directory = new DirectoryInfo(WorkPath);
             var files = directory.GetFiles("*.*", SearchOption.AllDirectories);
             foreach (FileInfo file in files)
             {
                 string destination = file.FullName.Replace(directory.FullName + @"\", "");
                 Log.Write("installing file '{0}'.", destination);
                 Message = Message + "Installing file: " + destination +  (char)13;
                 Thread.Sleep(300);

                 Directory.CreateDirectory(new FileInfo(destination).DirectoryName);
                 file.CopyTo(destination, true);
             }

             // Clean up.
             Log.Write("Deleting work directory.");
             Message = Message + "Deleting work directory." + (char)13;
             Thread.Sleep(300);

             Directory.Delete(WorkPath, true);

             // Restart.
             Log.Write("Spawning new process.");
             Message = Message + "Spawning new process.." + (char)13;
             Thread.Sleep(300);

             MessageBox.Show("Restarting....");


             var spawn = Process.Start(me);
             Log.Write("New process ID is {0}", spawn.Id);
             Message = Message + "New process ID is:" + spawn.Id +  (char)13;
             Thread.Sleep(300);

             Log.Write("Closing old running process {0}.", thisprocess.Id);
             Message = Message + "Closing old running process: " + thisprocess.Id + (char)13;
            
             thisprocess.CloseMainWindow();
             thisprocess.Close();
             thisprocess.Dispose();
             Application.Current.Shutdown();
             Environment.Exit(0);
         }

    }
}
