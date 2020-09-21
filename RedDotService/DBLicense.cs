using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDotService
{
    public class DBLicense
    {

        DBConnect _dbconnect;

        public DBLicense()
        {
            _dbconnect = new DBConnect();
        }

        public string GetPrivateKey()
        {
            return  "<RSAKeyValue><Modulus>yQ/uP0rOX41e6on5A44HCmGSxDBN+kKY8zolcYu1cN4GTwcyzWM4wv6ZRKBf3+5hH5Yt335NyisyUu+cfkfvs1ST5wwRgfV1Ql2azXADDKMGPZRhGux/x7oQEDufwDm4yY4qfJ/kx/IyHrs+tl60J1wiiE+s/CnxQMvrbSaFnVE=</Modulus><Exponent>AQAB</Exponent><P>/MHVwFcaFUHQLvLl5blzBhZptceckbYofX90zjU1vX5uFOKLOQX1r1vUg+ng3Qt5CPS+FPi3mPLHD931zMPnvQ==</P><Q>y6ROkZ+f2h5iXrXDs7UrCFyPsYWhx015TG5ff5K6fHXj2TGZE98gDOIgRsAS7KcdPZsmwBst4GnlE5b6REALJQ==</Q><DP>v0ztvQ+vnBsduAr7WW2M0zSveXfE1rvp1WJcQ54eOHeyVXhJKzWJh9mW9OhU2rhOOSsTmsfMHaTSaP3zhbFYeQ==</DP><DQ>K/yVrAbatHaTsPl6CDs9zFSSBTpkM3ScmtHMdvXuqiucx7Fa61vqxF2jsySR8eQ3ALOerygvxKWbAZw++rcKsQ==</DQ><InverseQ>dlt+ktdRHgbP6Gl+oTD2+E6oQTu2V/Fe70hXcZ6IS8+iG1/CNhzNqiX9nN7okl2blQVV9RCxGOYprjvC+3KKdA==</InverseQ><D>wWkssv+/5BT3IEDXJL9EMI1KBKW+7SWRQjBGAqL1R/ycLvtquD5hRNprD6QCdkQ2c48g06QJKphBEZzjemqyvaVuA6rIxm9rZUmQ0T7CjFkqDOQb/gx12XzVQbb66mBMWxLJS++Tfpf+JzYBs+homGsbI1101GnKk/I2AP9mO1E=</D></RSAKeyValue>";

        }



        public LicenseRequest GetPermission(LicenseRequest request )
        {
            try
            {
                LicenseRequest response = new LicenseRequest();


                string sql = "Select * from License where machineid ='" + request.MachineID + "' and application='" + request.Application + "'";

                DataTable dt = _dbconnect.GetData(sql);

                if (dt.Rows.Count > 0)
                {
                    string application = dt.Rows[0]["application"].ToString();
                    string machineid = dt.Rows[0]["machineid"].ToString();


                    string applevel = dt.Rows[0]["applevel"].ToString();
                    string startdate = dt.Rows[0]["startdate"].ToString();
                    string enddate = dt.Rows[0]["enddate"].ToString();
                    string createdate = dt.Rows[0]["createdate"].ToString();
                    string permission = dt.Rows[0]["permission"].ToString();


                    response.Application = application;
                    response.MachineID = machineid;
                    response.CreateDate = createdate + " PST";
                    response.CodeString = application.Trim() + "," + applevel.Trim() + "," + startdate.Trim() + "," + enddate.Trim() + "," + permission.TrimEnd();
                   response.Comment = dt.Rows[0]["comment"].ToString();


                    if (permission.Trim() == "")
                    {
                        response.Activated = false;
                      
                    }
                    else response.Activated = true;




                }
                else
                {
                    string insert = "insert into License (machineid,application) values ('" + request.MachineID + "','" + request.Application + "')";
                    _dbconnect.Command(insert);

                    response.Application = request.Application;
                    response.MachineID = request.MachineID;
                    response.CreateDate = DateTime.Now.ToShortDateString();
                    response.Activated = false;
                    response.Comment = "License Requested on " + response.CreateDate + " PST";
                }


                return response;
            }
            catch(Exception ex)
            {
                LicenseRequest response = new LicenseRequest();
                response.Comment = ex.Message;
                return response;
            }
          
        }
           

        }
}