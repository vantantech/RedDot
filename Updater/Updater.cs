using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Ionic.Zip;
using System.Windows;
using System.Reflection;


namespace RedDotDependents
{
    public class Updater
    {
        #region Constants
        /// <summary>
        /// The default check interval
        /// </summary>
        public const int DefaultCheckInterval = 900; // 900s == 15 min
        public const int FirstCheckDelay = 15;

        /// <summary>
        /// The default configuration file
        /// </summary>
        public string DefaultConfigFile = "update\\update.xml";
        
        public string WorkPath =  "work";
        #endregion

        #region Fields
     
        private volatile bool _updating;
        private Manifest _localConfig;
        private Manifest _remoteConfig;
        private FileInfo _localConfigFile;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Updater"/> class.
        /// </summary>


        /// <summary>
        /// Initializes a new instance of the <see cref="Updater"/> class.
        /// </summary>
        /// <param name="configFile">The configuration file.</param>
        public Updater()
        {
            Log.Debug = true;

           WorkPath =  Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + WorkPath;
            DefaultConfigFile = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + DefaultConfigFile;



            FileInfo configFile = new FileInfo(DefaultConfigFile);
            _localConfigFile =configFile;

            Log.Write("Loaded.");
            Log.Write("Initializing using file '{0}'.", configFile.FullName);
            if (!configFile.Exists)
            {
                Log.Write("Config file '{0}' does not exist, stopping.", configFile.Name);
                return;
            }

            string data = File.ReadAllText(configFile.FullName);
            this._localConfig = new Manifest(data);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the monitoring.
        /// </summary>



   

        /// <summary>
        /// Checks the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        public bool Check ()
        {
            Log.Write("Check starting.");

            if (_updating)
            {
                Log.Write("Updater is already updating.");
                Log.Write("Check ending.");
            }
            var remoteUri = new Uri(this._localConfig.RemoteConfigUri);

            Log.Write("Fetching '{0}'.", remoteUri.AbsoluteUri);
            var http = new Fetch { Retries = 5, RetrySleep = 30000, Timeout = 30000 };
            http.Load(remoteUri.AbsoluteUri);
            if (!http.Success)
            {
                Log.Write("Fetch error: {0}", http.Response.StatusDescription);
                this._remoteConfig = null;
                TouchMessageBox.Show("Not able to connect to VTT Update site");
                return false;
            }

            string data = Encoding.UTF8.GetString(http.ResponseData);
            this._remoteConfig = new Manifest(data);

            if (this._remoteConfig == null)  return false;

            if (this._localConfig.SecurityToken != this._remoteConfig.SecurityToken)
            {
                Log.Write("Security token mismatch.");
                TouchMessageBox.Show("Security token mismatch.");
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
            return true;
           
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update ()
        {

         
            _updating = true;
            Log.Write("Updating '{0}' files.", this._remoteConfig.Payloads.Length);

            // Clean up failed attempts.
            if (Directory.Exists(WorkPath))
            {
                Log.Write("WARNING: Work directory already exists.");
                try { Directory.Delete(WorkPath, true); }
                catch (IOException)
                {
                    Log.Write("Cannot delete open directory '{0}'.", WorkPath);
                    return;
                }
            }

            Directory.CreateDirectory(WorkPath);

            TouchMessageBox.Show("Downloading latest updates ... Click on OK to Continue.",10);
            
            // Download files in manifest.
            foreach (string updatefile in this._remoteConfig.Payloads)
            {
                Log.Write("Fetching '{0}'.", updatefile);
                var url = this._remoteConfig.BaseUri + updatefile;
                var file = Fetch.Get(url);



                if (file == null)
                {
                    Log.Write("Fetch failed.");
                    return;
                }
                var info = new FileInfo(Path.Combine(WorkPath, updatefile));
                Directory.CreateDirectory(info.DirectoryName);
                File.WriteAllBytes(Path.Combine(WorkPath, updatefile), file);

                // Unzip
                if ( Regex.IsMatch(updatefile, @"\.zip"))
                {
                    try
                    {
                        var zipfile = Path.Combine( WorkPath, updatefile);
                        using (var zip = ZipFile.Read(zipfile))
                            zip.ExtractAll(WorkPath, ExtractExistingFileAction.Throw);
                        File.Delete(zipfile);
                    }
                    catch (Exception ex)
                    {
                        Log.Write("Unpack failed: {0}", ex.Message);
                        return;
                    }
                }
            }

            // Change the currently running executable so it can be overwritten.
            Process thisprocess = Process.GetCurrentProcess();
            string me = thisprocess.MainModule.FileName;
            string bak = me + ".bak";
            Log.Write("Renaming running process to '{0}'.", bak);
            if(File.Exists(bak))
                File.Delete(bak);
            File.Move(me, bak);
            File.Copy(bak, me);

            // Write out the new manifest.
            // _remoteConfig.Write(Path.Combine(WorkPath, _localConfigFile.Name));
            _remoteConfig.Write(DefaultConfigFile);

            TouchMessageBox.Show("Installing new updates ... ", 10);
            // Copy everything.
            var directory = new DirectoryInfo(WorkPath);
            var files = directory.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                string destination = file.FullName.Replace(directory.FullName+@"\", "");
                Log.Write("installing file '{0}'.", destination);
                Directory.CreateDirectory(new FileInfo(destination).DirectoryName);
                file.CopyTo(destination, true);
            }

            // Clean up.
            Log.Write("Deleting work directory.");
            Directory.Delete(WorkPath, true);

            TouchMessageBox.Show("Installation completed .. restarting..", 10);
            // Restart.
            Log.Write("Spawning new process.");
            var spawn = Process.Start(me);
            Log.Write("New process ID is {0}", spawn.Id);
            Log.Write("Closing old running process {0}.", thisprocess.Id);
            thisprocess.CloseMainWindow();
            thisprocess.Close();
            thisprocess.Dispose();
            Application.Current.Shutdown();
            Environment.Exit(0);
        }


        public static void CheckAndUpdate(bool NeedToConfirm=false)
        {

            Updater update = new Updater();
            if (update.Check())
            {
                if(NeedToConfirm)
                {
                    Confirm dlg = new Confirm("Download and install updates?");
                    dlg.ShowDialog();
                    if (dlg.Action.ToUpper() != "YES") return;
                }
                update.Update();
            }

        }


        public static bool  CheckForUpdates()
        {

            Updater update = new Updater();
            return update.Check();
        }

        public static void GetUpdates()
        {
            Updater update = new Updater();
            update.Update();

        }
        #endregion
    }
}
