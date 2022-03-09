using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace diabeasy_back
{
    public class User
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();


        public bool SendMial(string mail, string subject, string Body)
        {

            string from = "diabeasyapp@gmail.com"; //From address    
            MailMessage message = new MailMessage(from, mail);
            message.Subject = subject;
            message.Body = Body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com",587); //Gmail smtp    
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("diabeasyapp", "talgalidan");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception )
            {
                logger.Fatal("the " + message + " was not send");
                return false;
            }

        }
        public string GetTypeByMail(string mail)
        {
            try
            {

                tblPatients p = DB.tblPatients.Where(x => x.email == mail).SingleOrDefault();
                if (p == null)
                {
                    tblDoctor d = DB.tblDoctor.Where(x => x.email == mail).SingleOrDefault();
                    if (d == null)
                    {
                        throw new Exception("do not found email");
                    }
                    else
                    {
                        return "doctor";
                    }
                }
                return "Patient";
            }
            catch (Exception )
            {
                logger.Fatal("do not found user in DB");
                return null;
            }

        }
    }
}
