using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
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

        private static void OnTimedEvent(Object o, ElapsedEventArgs e)
        {

            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  

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
            //aTimer.Enabled = false;

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
        public dynamic GetInjectionRecommend(int id, int blood_sugar_level, string injectionType)
        {
            try
            {


                string query = "";
                dynamic res = null;
                DataSet ds = new DataSet();
                SqlDataAdapter adpter;
                if (injectionType == "food" && blood_sugar_level <= 155)
                {
                    query = @"
				select AVG(pd1.totalCarbs/pd1.value_of_ingection) as 'ratio'
                from(
                select *,ROW_NUMBER() OVER (Order by date_time) AS RowNumber
                from tblPatientData
                where Patients_id=@id )as pd1 inner join (select date_time,blood_sugar_level,ROW_NUMBER() OVER (Order by date_time) AS RowNumber from tblPatientData where Patients_id=@id) as pd2 on
                pd1.RowNumber+1=pd2.RowNumber
                where DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4 and pd1.injectionType in('food') 
                and pd1.blood_sugar_level>70 and
                (DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4) and pd2.blood_sugar_level between 70 and 155 and injectionType='food'
				and pd1.blood_sugar_level<=155";
                    adpter = new SqlDataAdapter(query, con);
                    adpter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adpter.SelectCommand.Parameters.AddWithValue("@value", blood_sugar_level);

                    adpter.Fill(ds, "DataForRecommend");
                    DataTable dt = ds.Tables["DataForRecommend"];
                    res = new { food = dt.Rows[0]["ratio"] };
                }
                else if (injectionType == "food" || injectionType == "fix")
                {

                    query = @"declare @max int,@min int
 	            set @max=(select top 1 blood_sugar_level
				from tblPatientData
				where injectionType='fix' and Patients_id=@id
				order by blood_sugar_level desc)
				set @min=(select top 1 blood_sugar_level
				from tblPatientData
				where injectionType='fix' and Patients_id=@id
				order by blood_sugar_level)
	           if @value not between @min and @max
 			  if @value >= @max
				set  @value =@max
				else if
				 @value <= @min
				set  @value =@min
                select avg(ratio)  as ratio
                from
                (select
                pd1.blood_sugar_level,avg(((100-pd1.blood_sugar_level)*-1)/pd1.value_of_ingection) as ratio,ROW_NUMBER()OVER (Order by pd1.blood_sugar_level)  'num'
                from(
                select *,ROW_NUMBER() OVER (Order by date_time) AS RowNumber
                from tblPatientData
                where Patients_id=@id )as pd1 inner join (select date_time,blood_sugar_level,ROW_NUMBER() OVER (Order by date_time) AS RowNumber from tblPatientData where Patients_id=@id) as pd2 on
                pd1.RowNumber+1=pd2.RowNumber
                where DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4 and pd1.injectionType in('fix') 
                and pd1.blood_sugar_level>70 and
                (DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4) and pd2.blood_sugar_level between 70 and 155 and injectionType='fix'
                 group by pd1.blood_sugar_level
                ) t1 inner join
                (select
                pd1.blood_sugar_level,ROW_NUMBER()OVER (Order by pd1.blood_sugar_level)  'num'
                from(
                select *,ROW_NUMBER() OVER (Order by date_time) AS RowNumber
                from tblPatientData
                where Patients_id=@id )as pd1 inner join (select date_time,blood_sugar_level,ROW_NUMBER() OVER (Order by date_time) AS RowNumber from tblPatientData where Patients_id=@id) as pd2 on
                pd1.RowNumber+1=pd2.RowNumber
                where DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4 and pd1.injectionType in('fix') 
                and pd1.blood_sugar_level>70 and
                (DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4) and pd2.blood_sugar_level between 70 and 155 and injectionType='fix' 
                group by pd1.blood_sugar_level
                ) t2 on t1.num+1=t2.num
                where @value between t1.blood_sugar_level and t2.blood_sugar_level";


                    adpter = new SqlDataAdapter(query, con);
                    adpter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adpter.SelectCommand.Parameters.AddWithValue("@value", blood_sugar_level);


                    adpter.Fill(ds, "DataForRecommend");
                    DataTable dt = ds.Tables["DataForRecommend"];
                    res = new { fix = dt.Rows[0]["ratio"] };


                    if (injectionType == "food" && blood_sugar_level > 155)
                    {
                        query = @"declare @max int,@min int
                     set @max=(select top 1 blood_sugar_level
				from tblPatientData
				where injectionType='food' and Patients_id=@id
				order by blood_sugar_level desc)
				set @min=(select top 1 blood_sugar_level
				from tblPatientData
				where injectionType='fix' and Patients_id=@id
				order by blood_sugar_level)
				if @value not between @min and @max
				 if @value>@max
				 set @value=@max
				 else if @value<@min
				 set @value=@min

				select top 1 pd1.totalCarbs/(value_of_ingection-ROUND(((100-pd1.blood_sugar_level)*-1)/@ratio, 0)) as 'carbsRatio'
                from(
                select *,ROW_NUMBER() OVER (Order by date_time) AS RowNumber
                from tblPatientData
                where Patients_id=@id )as pd1 inner join (select date_time,blood_sugar_level,ROW_NUMBER() OVER (Order by date_time) AS RowNumber from tblPatientData where Patients_id=@id) as pd2 on
                pd1.RowNumber+1=pd2.RowNumber
                where DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4 and pd1.injectionType in('food') 
                and pd1.blood_sugar_level>70 and
                (DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4) and pd2.blood_sugar_level between 70 and 155 and injectionType='food'
				and @value<=pd1.blood_sugar_level and (value_of_ingection-ROUND(((100-pd1.blood_sugar_level)*-1)/@ratio, 0)) <>0
				order by pd1.blood_sugar_level  ";

                        adpter = new SqlDataAdapter(query, con);
                        adpter.SelectCommand.Parameters.AddWithValue("@id", id);
                        adpter.SelectCommand.Parameters.AddWithValue("@value", blood_sugar_level);
                        adpter.SelectCommand.Parameters.AddWithValue("@ratio", dt.Rows[0]["ratio"]);
                        adpter.Fill(ds, "DataForRecommendFood");
                        DataTable dt2 = ds.Tables["DataForRecommendFood"];

                        res = new { fix = dt.Rows[0]["ratio"], food = dt2.Rows[0]["carbsRatio"] };
                    }
                }



                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }

    }


}
