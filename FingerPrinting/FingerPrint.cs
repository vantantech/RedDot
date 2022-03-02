using DPUruNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FingerPrinting
{

    public class FingerPrint
    {

        private Reader reader;

        private int fingerindex;//indexfinger
        private int count;
        private List<Fmd> preEnrollmentFmd;
        private DataResult<Fmd> enrollmentFmd, fmd1, fmd2;
        private bool fingerprintavailable = false;

        private List<Fmd> AllFmds;
        private int[] userids;

        private Fmd fmd = null;

        private bool m_enabled = true;
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                m_enabled = value;
            }
        }


        public Fmd[] GetAllFmd1s { get; private set; }

        public Fmd[] GetAllFmd2s { get; private set; }


        public string[] GetAllUserNames { get; private set; }

        public int[] GetallfingerIDs { get; private set; }

        public string[] GetAllUserPins { get; private set; }
        public int[] GetallPinIDs { get; private set; }


        public void LoadAllFmdsUserIDs(DataTable dt)
        {


            int i = 0;
            int j = 0;


            GetAllFmd1s = new Fmd[dt.Rows.Count];
            GetAllFmd2s = new Fmd[dt.Rows.Count];
            GetAllUserNames = new string[dt.Rows.Count];
            GetAllUserPins = new string[dt.Rows.Count];


            GetallfingerIDs = new int[dt.Rows.Count];
            GetallPinIDs = new int[dt.Rows.Count];
            bool hasprints = false;

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr["fmd1"].ToString().Length != 0) && (dr["fmd2"].ToString().Length != 0))
                    {
                        hasprints = true;
                        GetAllFmd1s[i] = Fmd.DeserializeXml(dr["fmd1"].ToString());
                        GetAllFmd2s[i] = Fmd.DeserializeXml(dr["fmd2"].ToString());
                        GetAllUserNames[i] = dr["firstname"].ToString().TrimEnd() + " " + dr["lastname"].ToString().TrimEnd();

                        GetallfingerIDs[i] = int.Parse(dr["id"].ToString());
                        i++;
                    }

                    //need to load pins and separate set of IDs since not all employee might have fingerprints
                    GetAllUserPins[j] = dr["pin"].ToString().TrimEnd();
                    GetallPinIDs[j] = int.Parse(dr["id"].ToString());
                    j++;
                }
            }

            if (!hasprints)
            {
                GetAllFmd1s = null;
                GetAllFmd2s = null;
            }

        }
        public static bool Testforfingerprint()
        {
            try
            {
                ReaderCollection rc = ReaderCollection.GetReaders();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public  bool InitializeReaders()
        {
            if (!m_enabled) return false;


            try
            {
                ReaderCollection rc = ReaderCollection.GetReaders();
                if (rc.Count == 0)
                {
                    //UpdateEnrollMessage("Fingerprint Reader not found. Please check if reader is plugged in and try again", null);
                    // TouchMessageBox.Show("Fingerprint Reader not found. Please check if reader is plugged in and try again");
                    return false;
                }
                else
                {
                    reader = rc[0];
                    Constants.ResultCode readersResult = reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
                    return true;
                }
            }catch(Exception ex)
            {
                if (ex.Source == "DPUruNet")
                {
                   return false;
                }
                else
                    return true;
            }

        }


   



        public void CancelCaptureAndCloseReader()
        {
            if (reader != null)
            {
                reader.CancelCapture();
                reader.Dispose();
            }
        }

        public string StartEnrollment()
        {
            if (!m_enabled) return "Finger Print Disabled";


            fingerindex = 0;
            preEnrollmentFmd = new List<Fmd>();

            Constants.ResultCode result = reader.GetStatus();
        

            CheckReaderStatus();

            if (result== Constants.ResultCode.DP_SUCCESS)
            {
                reader.On_Captured += new Reader.CaptureCallback(reader_On_Captured);
                EnrollmentCapture();
              return "Place first finger on Reader.";
            }
            else
            {
               return "Could not perform capture. Reader result code :" + result.ToString();
            }

        }


        public string StartIndentification()
        {
            if (!m_enabled) return "Finger Print Disabled";

            if (fingerprintavailable == false) return "";


            Constants.ResultCode result = reader.GetStatus();
            CheckReaderStatus();
            if (result == Constants.ResultCode.DP_SUCCESS)
            {
                if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_READY)
                {
                    reader.On_Captured += new Reader.CaptureCallback(reader_On_Captured2);
                    IdentificationCapture();
                 
                }

                return "Started..";
            }
            else
            {
                return "Could not perform capture. Reader result code :" + result.ToString();
            }
        }



        private void EnrollmentCapture()
        {
            count = 0;
            Constants.ResultCode captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);
            if (captureResult != Constants.ResultCode.DP_SUCCESS)
            {
                //TouchMessageBox.Show("CaptureResult: " + captureResult.ToString());
            }
        }

        private void IdentificationCapture()
        {
            Constants.ResultCode captureResult = reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, reader.Capabilities.Resolutions[0]);
            if (captureResult != Constants.ResultCode.DP_SUCCESS)
            {
               // MessageBox.Show("CaptureResult: " + captureResult.ToString());
            }

        }
        //Async so all message back needs to be INVOKED
        void reader_On_Captured(CaptureResult capResult)
        {

            if (capResult.Quality == Constants.CaptureQuality.DP_QUALITY_GOOD)
            {
                count++;
                DataResult<Fmd> fmd = FeatureExtraction.CreateFmdFromFid(capResult.Data, Constants.Formats.Fmd.DP_PRE_REGISTRATION);

                // Get view bytes to create bitmap.
                foreach (Fid.Fiv fiv in capResult.Data.Views)
                {
                    //Ask user to press finger to get fingerprint
                   // if (fingerindex == 0)
                       // UpdateEnrollMessage1(" Count:" + count.ToString());
                   // else
                       // UpdateEnrollMessage2(" Count:" + count.ToString());

                    //UpdateEnrollImage(Utility.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                    //UpdateEnrollImage(Utility.CopyDataToBitmap(fiv.RawImage, fiv.Width, fiv.Height));

                    break;
                }


                //pre enrollment code decides how many times to scan finger before it returns a successful preenrollment
                preEnrollmentFmd.Add(fmd.Data);
                enrollmentFmd = DPUruNet.Enrollment.CreateEnrollmentFmd(Constants.Formats.Fmd.DP_REGISTRATION, preEnrollmentFmd);


                //enrollment is success for finger so goes to next
                if (enrollmentFmd.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    if (fingerindex == 0)
                    {
                        fmd1 = enrollmentFmd;
                        fingerindex++;
                        count = 0;
                        preEnrollmentFmd.Clear();
                        //UpdateEnrollMessage1("Finger 1 DONE");
                        //UpdateEnrollMessage("Place second finger on reader");
                    }
                    else if (fingerindex == 1)
                    {
                        fmd2 = enrollmentFmd;
                        preEnrollmentFmd.Clear();
                       // UpdateEnrollMessage2("Finger 2 DONE");

                       // UpdateEnrollMessage("User  Successfully Enrolled!");
                       // string userid = CurrentEmployee.ID.ToString();

                       // CurrentEmployee.FMD1 = Fmd.SerializeXml(fmd1.Data);
                        //CurrentEmployee.FMD2 = Fmd.SerializeXml(fmd2.Data);

                       // MarkDone();


                    }

                }

            }





        }



        private void reader_On_Captured2(CaptureResult capResult)
        {

            if (capResult.Quality == Constants.CaptureQuality.DP_QUALITY_GOOD)
            {
                DataResult<Fmd> fmdResult = FeatureExtraction.CreateFmdFromFid(capResult.Data, Constants.Formats.Fmd.DP_VERIFICATION);
                //If successfully extracted fmd then assign fmd
                if (fmdResult.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    fmd = fmdResult.Data;
                }
                else
                {
                   // UpdateIdentifyMessage("Could not successfully create a verification FMD");
                }

                // Get view bytes to create bitmap.
                foreach (Fid.Fiv fiv in capResult.Data.Views)
                {
                    // UpdateEnrollImage(Utility.CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                   // UpdateEnrollImage(Utility.CopyDataToBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                   // UpdateIdentifyMessage("Fingerprint Captured");
                    break;
                }


             //   if (GlobalSettings.Instance.GetAllFmd1s == null)
               // {
                //    UpdateIdentifyMessage("No finger print in database");
                //    return;
               // }




                //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                IdentifyResult iResult1 = Comparison.Identify(fmd, 0, AllFmds, 21474, 5);

                bool matchesfound = false;
                //If Identify was successfull
                if (iResult1.ResultCode == Constants.ResultCode.DP_SUCCESS)
                {
                    //If number of matches were greater than 0
                    if (iResult1.Indexes.Length > 0)
                    {
                        matchesfound = true;
                        string usersIdentified = null;
                        int idfound = 0;

                        //Get all usernames list
                       // usernames = GlobalSettings.Instance.GetAllUserNames;
                       // userids = GlobalSettings.Instance.GetallfingerIDs;

                        //for each matched fmd get its index and map that index to userid index
                        for (int i = 0; i < iResult1.Indexes.Length; i++)
                        {
                            int index = iResult1.Indexes[i][0];
                            if (i == 0)
                            {
                              //  usersIdentified = usernames[index];
                                idfound = userids[index];
                            }
                            else
                            {
                               // usersIdentified = usersIdentified + ", " + usernames[index];
                                idfound = 0;
                            }
                        }
                       // UpdateIdentifyMessage("User: " + usersIdentified + " Authorized");
                       // Verified(idfound);
                    }
                }
                if (matchesfound != true)
                {
                    //Perform indentification of fmd of captured sample against enrolledFmds for userid 
                    IdentifyResult iResult2 = Comparison.Identify(fmd, 0, AllFmds, 21474, 5);
                    if (iResult2.ResultCode == Constants.ResultCode.DP_SUCCESS)
                    {
                        //If number of matches were greater than 0
                        if (iResult2.Indexes.Length > 0)
                        {
                            matchesfound = true;
                            string usersIdentified = null;
                            int idfound = 0;
                            //Get all usernames list
                          //  usernames = GlobalSettings.Instance.GetAllUserNames;
                           // userids = GlobalSettings.Instance.GetallfingerIDs;
                            //for each matched fmd get its index and map that index to userid index
                            for (int i = 0; i < iResult2.Indexes.Length; i++)
                            {
                                int index = iResult2.Indexes[i][0];
                                if (i == 0)
                                {
                                   // usersIdentified = usernames[index];
                                    idfound = userids[index];
                                }
                                else
                                {
                                  //  usersIdentified = usersIdentified + ", " + usernames[index];
                                    idfound = 0;
                                }
                            }
                          //  UpdateIdentifyMessage("User: " + usersIdentified + " Authorized");
                          //  Verified(idfound);
                        }
                    }
                }

                if (!matchesfound)
                {
                   // UpdateIdentifyMessage2("NOT Unauthorized");
                }



            }
            else
            {
               // UpdateIdentifyMessage("Please swipe finger again");
            }


        }













        public void CheckReaderStatus()
        {
            //If reader is busy, sleep
            if (reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY)
            {
                Thread.Sleep(50);
            }
            else if ((reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                reader.Calibrate();
            }
            else if ((reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Reader Status - " + reader.Status.Status);
            }
        }


        /*

        public void LoadAllFmdsUserIDs()
        {
            DBEmployee dbemployee = new DBEmployee();

            int i = 0;
            int j = 0;

            // populate all fmds and usernames
            DataTable dt = dbemployee.GetEmployeeActive();

            GetAllFmd1s = new Fmd[dt.Rows.Count];
            GetAllFmd2s = new Fmd[dt.Rows.Count];
            GetAllUserNames = new string[dt.Rows.Count];
            GetAllUserPins = new string[dt.Rows.Count];


            GetallfingerIDs = new int[dt.Rows.Count];
            GetallPinIDs = new int[dt.Rows.Count];
            bool hasprints = false;

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if ((dr["fmd1"].ToString().Length != 0) && (dr["fmd2"].ToString().Length != 0))
                    {
                        hasprints = true;
                        GetAllFmd1s[i] = Fmd.DeserializeXml(dr["fmd1"].ToString());
                        GetAllFmd2s[i] = Fmd.DeserializeXml(dr["fmd2"].ToString());
                        GetAllUserNames[i] = dr["firstname"].ToString().TrimEnd() + " " + dr["lastname"].ToString().TrimEnd();

                        GetallfingerIDs[i] = int.Parse(dr["id"].ToString());
                        i++;
                    }

                    //need to load pins and separate set of IDs since not all employee might have fingerprints
                    GetAllUserPins[j] = dr["pin"].ToString().TrimEnd();
                    GetallPinIDs[j] = int.Parse(dr["id"].ToString());
                    j++;
                }
            }

            if (!hasprints)
            {
                GetAllFmd1s = null;
                GetAllFmd2s = null;
            }

        }

    */
    }
}
