using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;

namespace diabeasy_back
{
    public class Food
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public Nullable<double> calc100Grams(int unid_id, int weightInGrams, double value)
        {
            try
            {
                tblUnitOfMeasure U = DB.tblUnitOfMeasure.Where(x => x.id == unid_id).SingleOrDefault();
                if (U.name == "grams")
                    return null;
                double calcValue = (value / weightInGrams) * 100;
                return calcValue;
            }
            catch (Exception ex)
            {
                logger.Fatal("do not found unit id in DB" + ex.Message);
                return null;
            }
        }
        public int getUnitID(string name)
        {
            try
            {
                tblUnitOfMeasure U = DB.tblUnitOfMeasure.Where(x => x.name == name).SingleOrDefault();
                if (U == null)
                    throw new NullReferenceException("invalid unit id");

                return U.id;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return int.MinValue;
            }
        }
        //public async Task<HttpResponseMessage> search_by_name_api(string foodName)
        //{
        //    var httpClient = HttpClientFactory.Create();
        //    string url = "https://api.spoonacular.com/food/ingredients/search?query=" + foodName + "&apiKey=c2f5f275954a42edaf91a07cb28f3343";
        //    try
        //    {
        //        HttpResponseMessage data = await httpClient.GetAsync(url);
        //        if (data.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            return data;
        //        }
        //        return null;

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Fatal(ex.Message);
        //        return null;
        //    }
        //}
        public async Task<object> search_by_name_api(string name)
        {

            HttpClient client = new HttpClient();
            try
            {

                string urlParameters = $"?query={name}&apiKey=c2f5f275954a42edaf91a07cb28f3343";
                client.BaseAddress = new Uri("https://api.spoonacular.com/food/ingredients/search");

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(responseBody);
                    var firstFood = json.results[0];
                    return get_Food_information_api(firstFood);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }
            finally
            {

                // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
                client.Dispose();
            }
        }
        public async Task<object> get_Food_information_api(dynamic foodObject)
        {

            HttpClient client = new HttpClient();
            try
            {
                string id = foodObject.id;
                string urlParameters = $"?apiKey=c2f5f275954a42edaf91a07cb28f3343&unit=grams&amount=100";
                client.BaseAddress = new Uri("https://api.spoonacular.com/food/ingredients/"+id+"/information");

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = JsonConvert.DeserializeObject(responseBody);
                    return json;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }
            finally
            {

                // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
                client.Dispose();
            }
        }
    }
}
