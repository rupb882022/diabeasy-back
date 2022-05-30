using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;
using NLog;

namespace diabeasy_back
{
    public class User
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
       // static Timer aTimer;
        public static object objectToSend;
        public bool SendMial(string Email, string Subject, string Body)
        {

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress("diabeasyapp@gmail.com");
                mail.Subject = Subject;
                mail.Body = Body;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("diabeasyapp@gmail.com", "talgalidan");
                smtp.Send(mail);
                return true;




                //string to = Email;
                //string subject = Subject;
                //string body = Body;
                //using (MailMessage mm = new MailMessage("diabeasyapp@gmail.com", Email))
                //{
                //    mm.Subject = Subject;
                //    mm.Body = Body;

                //    mm.IsBodyHtml = false;
                //    logger.Debug(mm.Body + "===============>cheak");
                //    mm.To.Add(Email);
                //    SmtpClient smtp = new SmtpClient();
                //    smtp.Host = "smtp.gmail.com";
                //    smtp.EnableSsl = true;
                //    NetworkCredential NetworkCred = new NetworkCredential("diabeasyapp", "talgalidan");
                //    smtp.UseDefaultCredentials = true;
                //    smtp.Credentials = NetworkCred;
                //    smtp.Port = 587;
                //    logger.Debug("check point");
                //    smtp.Send(mm);
                //    return true;
                //}







                //MailMessage mail = new MailMessage();
                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                //mail.From = new MailAddress("rupi652022@gmail.com");
                //mail.To.Add(Email);
                //mail.Subject = subject;
                ////mail.Body = "שלום" + Environment.NewLine +
                ////"הסיסמא הזמנית הינה:  " + UserRandomPassword + Environment.NewLine +
                ////"ניתן להחליף סיסמא בלינק הבא" + Environment.NewLine + "http://proj.ruppin.ac.il/igroup65/test2/B/proj1/Login_v1/SetNewPass.html";
                //mail.Body=Body;
                //SmtpServer.Port = 587;
                //SmtpServer.Credentials = new System.Net.NetworkCredential("diabeasyapp", "talgalidan");
                //SmtpServer.EnableSsl = true;
                //SmtpServer.Send(mail);
                //return true;
            }
            catch (Exception ex)
            {
                logger.Fatal("the " + Subject + " was not send " + ex.Message + "\ninner Exception: " + ex.InnerException);
                return false;
            }

            //string from = "diabeasyapp@gmail.com"; //From address    
            //MailMessage message = new MailMessage(from, mail);
            //message.Subject = subject;
            //message.Body = Body;
            //message.BodyEncoding = Encoding.UTF8;
            //message.IsBodyHtml = true;
            //SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            //client.UseDefaultCredentials = false;
            //System.Net.NetworkCredential basicCredential1 = new
            //System.Net.NetworkCredential("diabeasyapp", "talgalidan");
            //client.EnableSsl = true;
            //client.UseDefaultCredentials = false;
            //client.Credentials = basicCredential1;


            //SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            //{
            //    Credentials = new NetworkCredential("diabeasyapp@gmail.com", "talgalidan"),
            //    EnableSsl = true
            //};
            //client.Send("diabeasyapp@gmail.com", "idanlavee10@gmail.com", "test", "testbody");







        }


        //public async Task<bool> PushNotification(int seconds)
        //{
        //    try
        //    {


        //        Timer aTimer = new Timer();
        //        aTimer.Interval = seconds <= 0 ? 1000 : seconds * 1000; // Interval must be greater then 0; Default => 1 sec;

        //        objectToSend = new
        //        {
        //            to = "ExponentPushToken[2S01zuIBNraplwZePN2Leh]",
        //            title = "DiabeasyApp",
        //            body = "Boker Tov! Hezrakta hayom??? ",
        //            badge = 0,
        //            ttl = 1,
        //        };

        //        aTimer.Enabled = true;
        //        aTimer.Elapsed += OnTimedEvent;


        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //        throw;
        //    }
        //}

     

        public string PushNotificationNow(int id,string body) { 
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
            // Set the Method property of the request to POST.  
            request.Method = "POST";
                // Create POST data and convert it to a byte array.  
                string token;
                if (id % 2 == 0)
                {
                    tblDoctor d = DB.tblDoctor.SingleOrDefault(x => x.id == id);
                    if (d.pushtoken == null)
                    {
                        return $"No doctor Token id {id}";
                    }
                    token = d.pushtoken;
                }
                else
                {
                    tblPatients p = DB.tblPatients.SingleOrDefault(x => x.id == id);
                    if (p.pushtoken == null)
                    {
                        return $"No patient Token id {id}";
                    }
                    token = p.pushtoken;
                }
                    var objectToSend = new
            {
                //  to = "ExponentPushToken[I8PGiZHdZMMnFcg_USV5r_]",      "ExponentPushToken[2S01zuIBNraplwZePN2Leh]"
                to = token,
                title = "Diabeasy App",
                body = body,
                badge = 0,
                //data = new { name = "nir", grade = 100, seconds = DateTime.Now.Second }
            };

            string postData = new JavaScriptSerializer().Serialize(objectToSend);

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            string returnStatus = ((HttpWebResponse)response).StatusDescription;
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            //Console.WriteLine(responseFromServer);
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();

            return "success:) --- " + responseFromServer + ",-- " + returnStatus;
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

                logger.Fatal("do not found doctor in DB " + ex.Message);
                return null;
            }

        }
        public bool checkUniqeMail(string email)
        {
            try
            {
                tblPatients p = DB.tblPatients.Where(x => x.email == email).SingleOrDefault();
                if (p == null)
                {
                    tblDoctor d = DB.tblDoctor.Where(x => x.email == email).SingleOrDefault();
                    return d == null;
                }
                return false;



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
    public class PushNotData
    {
        public string to { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int badge { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }

       //  public int id { get; set; }

    }

    public class Data
    {
        public string to { get; set; }
        public string title { get; set; }
        public string status { get; set; }
    }

}
