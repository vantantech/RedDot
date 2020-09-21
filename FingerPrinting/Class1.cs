using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPUruNet;
using RedDot.DataManager;

namespace FingerPrinting
{
    public class Class1
    {
        public Fmd[] GetAllFmd1s { get; private set; }

        public Fmd[] GetAllFmd2s { get; private set; }


        public string[] GetAllUserNames { get; private set; }

        public int[] GetallfingerIDs { get; private set; }

        public string[] GetAllUserPins { get; private set; }
        public int[] GetallPinIDs { get; private set; }


        public void LoadAllFmdsUserIDs()
        {
            DBEmployee dbemployee = new DBEmployee();

            int i = 0;
            int j = 0;

            //first check to see if fmd1 and fmd2 is available
            dbemployee.CheckFMDFields();

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
    }
}
