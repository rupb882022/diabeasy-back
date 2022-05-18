using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Web.Http.Cors;
using diabeasy_back;
using NLog;
using System.Web.Script.Serialization;
using System.Timers;


namespace WebApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]

    public class PushController : ApiController
    {
     diabeasyDBContext DB = new diabeasyDBContext();
     static Logger logger = LogManager.GetCurrentClassLogger();
     private static Timer aTimer;
      public static object objectToSend;


        [Route("api/sendpushnotification")]
        public static string SendPushNotification([FromBody] PushNotData pnd)
        {
            try
            {
            int seconds = pnd.ttl;
            aTimer = new Timer();
            aTimer.Interval= seconds<=0?1000:seconds*1000; // Interval must be greater then 0; Default => 1 sec;
            aTimer.Elapsed += OnTimedEvent;
            objectToSend = new
            {
                to = pnd.to,
                title = pnd.title,
                body = pnd.body,
                badge = pnd.badge,
                sound = "default",
                data = pnd.data         //new { name = "nir", grade = 100 }
            };
            aTimer.Enabled = true;


        //    aTimer.Stop();
            // return "success:) - " + responseFromServer + ", " + returnStatus;
            return $"success:) the message will send in {seconds} seconds";
            }
            catch (Exception e)
            {
                logger.Error("Error catched in SendPushNotification function");
                return e.Message;
            }
        }
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
            aTimer.Enabled = false;

        }

        [Route("api/sendpushnotificationFromLocal")]
        public string Get()
        {
            // Create a request using a URL that can receive a post.   
            WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  
            var objectToSend = new
            {
                to = "ExponentPushToken[I8PGiZHdZMMnFcg_USV5r_]",
                title = "form local Server",
                body = "body from local Server",
                badge = 75,
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

    public class PushNotData
    {
        public string to { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int badge { get; set; }
        public int ttl { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string to { get; set; }
        public string title { get; set; }
        public string status { get; set; }
    }

}




