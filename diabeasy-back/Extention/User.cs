using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
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
            }
            catch (Exception ex)
            {
                logger.Fatal("the " + Subject + " was not send " + ex.Message + "\ninner Exception: " + ex.InnerException);
                return false;
            }

        }
        public string PushNotificationNow(int id, string body)
        {
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
                        double carbsRatio = double.Parse(dt2.Rows[0]["carbsRatio"].ToString());
                        if (carbsRatio <= 0)
                        {
                            query = @"
				select AVG(pd1.totalCarbs/pd1.value_of_ingection) as 'carbsRatio'
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

                            adpter.Fill(ds, "DataForRecommendFood2");
                            dt2 = ds.Tables["DataForRecommendFood2"];
                        }

                        res = new { fix = dt.Rows[0]["ratio"], food = dt2.Rows[0]["carbsRatio"] };
                    }
                }

                query = @"
				delete tempData where 1=1
                insert into tempData
                select datename(dw,pd1.date_time)as 'weekday',
                CASE
	            WHEN DATEPART(HOUR, pd1.date_time) between 0 and 6 then 'night'
		        WHEN DATEPART(HOUR, pd1.date_time) between 6 and 12 then 'morning'
			    WHEN DATEPART(HOUR, pd1.date_time) between 12 and 18 then 'noon'
				WHEN DATEPART(HOUR, pd1.date_time) between 18 and 24 then 'evning'
				end as 'dayTime',
                pd1.blood_sugar_level,
				CASE WHEN pd1.value_of_ingection is not null then pd1.value_of_ingection else 0 end as 'value_of_ingection',
				pd1.totalCarbs,
                CASE
	            WHEN (DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4) and pd2.blood_sugar_level between 70 and 155 THEN 1 
	            else 0 end as 'good injection'
                from(
                select *,ROW_NUMBER() OVER (Order by date_time) AS RowNumber
                from tblPatientData
                where Patients_id=@id )as pd1 inner join (select date_time,blood_sugar_level,ROW_NUMBER() OVER (Order by date_time) AS RowNumber from tblPatientData where Patients_id=@id) as pd2 on
                pd1.RowNumber+1=pd2.RowNumber
                where DATEDIFF(second, pd1.date_time, pd2.date_time) / 3600.0 between 2 and 4 and pd1.injectionType in('food','fix') 
                and pd1.blood_sugar_level>70;
                select count (*) as 'rowNum'
                from tempData";

                adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@id", id);
                adpter.Fill(ds, "tempData");
                DataTable dt3 = ds.Tables["tempData"];
                logger.Debug("Ml tempData rowNum= "+dt3.Rows[0]["rowNum"]);
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }

        //public async Task<object> MLRecommend(int Blood_sugar_level, double TotalCarbs, DateTime date_time)
        //{

        //    HttpClient client = new HttpClient();
        //    Uri url = new Uri("https://diabeasyml.herokuapp.com/predict");

        //    //client.BaseAddress = url;


        //    // Add an Accept header for JSON format.
        //    client.DefaultRequestHeaders.Accept.Add(
        //    new MediaTypeWithQualityHeaderValue("application/json"));
        //    try
        //    {
        //        string Weekday = date_time.DayOfWeek.ToString();
        //        string DayTime = "";
        //        if (date_time.Hour > 0 && date_time.Hour <= 6)
        //        {
        //            DayTime = "night";
        //        }
        //        else if (date_time.Hour > 6 && date_time.Hour <= 12)
        //        {
        //            DayTime = "morning";
        //        }
        //        else if (date_time.Hour > 12 && date_time.Hour <= 18)
        //        {
        //            DayTime = "noon";
        //        }
        //        else if (date_time.Hour > 18 && date_time.Hour <= 23)
        //        {
        //            DayTime = "evning";
        //        }
        //        int Value_of_ingection = 10;
        //        string jsonObject = 
        //        "{"+ $"Weekday = {Weekday}, DayTime = {DayTime}, Blood_sugar_level = {Blood_sugar_level}, Value_of_ingection = {Value_of_ingection}, TotalCarbs = {TotalCarbs}" +"}";


        //        var content = new StringContent(JsonConvert.DeserializeObject(jsonObject).ToString(), Encoding.UTF8, "application/json");
                
        //        var result = client.PostAsync(url, content).Result;

        //        var res = JsonConvert.DeserializeObject(result.ToString());
        //        return res;




        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Fatal(ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        client.Dispose();
        //    }
        //}

    }
    //public class PushNotData
    //{
    //    public string to { get; set; }
    //    public string title { get; set; }
    //    public string body { get; set; }
    //    public int badge { get; set; }
    //    public int ttl { get; set; }
    //    public Data data { get; set; }

    //   //  public int id { get; set; }

    //}

    //public class Data
    //{
    //    public string to { get; set; }
    //    public string title { get; set; }
    //    public string status { get; set; }
    //}

}
