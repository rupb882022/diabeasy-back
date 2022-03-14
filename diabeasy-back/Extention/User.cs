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
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("diabeasyapp", "talgalidan");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;


            //    SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            //{
            //    Credentials = new NetworkCredential("diabeasyapp@gmail.com", "talgalidan"),
            //    EnableSsl = true
            //};
            //client.Send("diabeasyapp@gmail.com", "idanlavee10@gmail.com", "test", "testbody");


            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal("the " + message + " was not send"+ ex.Message);
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
            catch (Exception ex)
            {
                logger.Fatal("do not found user in DB" + ex.Message);
                return null;
            }

        }
        public Nullable<int> checkDoctorMail(string email)
        {
            try
            {
                return DB.tblDoctor.Where(x => x.email == email).Select(x => x.id).SingleOrDefault();

            }
            catch (Exception ex)
            {

                logger.Fatal("do not found doctor in DB "+ex.Message);
                return null;
            }
   
        }
        public bool checkUniqeMail(string email,bool isDoctor)
        {
            try
            {
                if (isDoctor)
                {
                   tblDoctor d= DB.tblDoctor.Where(x => x.email == email).SingleOrDefault();
                    return d == null;
                }
                else
                {
                    tblPatients p = DB.tblPatients.Where(x => x.email == email).SingleOrDefault();
                    return p == null;

                }

            }
            catch (Exception ex)
            {

                logger.Fatal("do not found doctor in DB " + ex.Message);
                return false;
            }

        }
        public string NameToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

    }


}
