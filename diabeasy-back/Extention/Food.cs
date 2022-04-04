using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NLog;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace diabeasy_back
{
    public class Food
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);

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
                client.BaseAddress = new Uri("https://api.spoonacular.com/food/ingredients/" + id + "/information");

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
                    if (responseBody != null)
                    {
                        insertFoodByApiToDB(responseBody);
                    }
                    dynamic Foodjson = JsonConvert.DeserializeObject(responseBody);
                    return Foodjson;
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
        bool insertFoodByApiToDB(string foodByAPi)
        {
            try
            {
                dynamic Foodjson = JsonConvert.DeserializeObject(foodByAPi);

                //crete new Ingredient in DB
                DB.Ingredients.Add(new Ingredients()
                {
                    name = Foodjson.name,
                    image = Foodjson.image,
                    addByUserId = 3,

                });
                DB.SaveChanges();

                //get the new Ingredient
                Ingredients newIngredient = DB.Ingredients.OrderByDescending(x => x.id).First();
                //get Ingredient detailes
                double carbs = 0, suger = 0;
                int unit_id = getUnitID(Foodjson.unit.ToString());
                JArray nutrients = (JArray)Foodjson.nutrition.nutrients;
                for (int i = 0; i < nutrients.Count; i++)
                {
                    if (nutrients[i]["name"].ToString() == "Carbohydrates")
                    {
                        carbs = double.Parse(nutrients[i]["amount"].ToString());
                    }
                    if (nutrients[i]["name"].ToString() == "Sugar")
                    {
                        suger = double.Parse(nutrients[i]["amount"].ToString());
                    }
                }
                //add Ingredient detailes
                DB.tblBelong.Add(new tblBelong()
                {
                    Ingredient_id = newIngredient.id,
                    UnitOfMeasure_id = unit_id,
                    carbohydrates = carbs,
                    sugars = suger,
                    weightInGrams = 100
                });
                //check if category exist

                JArray categoryName = (JArray)Foodjson.categoryPath;
                string query = "";
                int categoryId = 0;
                if (categoryName.Count > 0)
                {
                    categoryId = getCategoryId(categoryName[0].ToString());
                }
                else
                {
                    categoryId = getCategoryId("general");
                }

                if (categoryId > 0)
                {
                    query = $"insert into PartOf_Ingredients values ({newIngredient.id},{categoryId})";
                }
                else
                {
                    query = $"insert into PartOf_Ingredients values ({newIngredient.id},{categoryName[0]})";
                }

                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot insert values into tblPartOf_Ingredients");
                }
                DB.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
            }
            return false;
        }
        int getCategoryId(string name)
        {
            try
            {
                tblCategory c = DB.tblCategory.Where(x => x.name == name).SingleOrDefault();
                return c != null ? c.id : 0;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return 0;
            }
        }
    }
}
