using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace RedDot
{
    public class EmailModel
    {


        public void EmailPDF(int SalesID, Customer CurrentCustomer)
        {

            try
            {

                string attachfile;
                string AppPath;

                string SMTPUserName = GlobalSettings.Instance.SMTPUserName;
                string SMTPPassword = GlobalSettings.Instance.SMTPPassword;
                string SMTPCopyTo   = GlobalSettings.Instance.SMTPCopyTo;
                string SMTPServer   = GlobalSettings.Instance.SMTPServer;
                string StoreEmail   = GlobalSettings.Instance.StoreEmail;
                int SMTPPort        = GlobalSettings.Instance.SMTPPort;


                AppPath = System.AppDomain.CurrentDomain.BaseDirectory;
               // string path = System.IO.Directory.GetCurrentDirectory();

                attachfile = AppPath + "pdf\\Ticket" + SalesID.ToString() + ".pdf";

                if (!File.Exists(attachfile))
                {
                    TouchMessageBox.Show("PDf file not found:  Please Print PDF first");
                    return;
                }

                EmailCustomer eml = new EmailCustomer(CurrentCustomer.Email, "Ticket:" + SalesID.ToString(), "Your Ticket:" + SalesID.ToString() + " has been attached", "Ticket" + SalesID.ToString() + ".pdf");
                eml.Topmost = true;
                eml.ShowDialog();

                if (eml.Action == "cancel") return;


                if (eml.To.Length < 1)
                {
                    TouchMessageBox.Show("Address is invalid");
                    return;
                }

                MailMessage mail = new MailMessage(StoreEmail, eml.To);
                SmtpClient client = new SmtpClient();
                client.Port = SMTPPort;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = SMTPServer;
                client.Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword);

                MailAddress copy = new MailAddress(SMTPCopyTo);
                mail.CC.Add(copy);

                mail.Subject = eml.Subject;
                mail.Body = eml.Message;
                mail.Attachments.Add(new Attachment(attachfile));
                client.Send(mail);
                TouchMessageBox.Show("Email Sent!");
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Email: " + e.Message);
            }


        }
    }
}
