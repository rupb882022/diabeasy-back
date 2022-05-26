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

                    dynamic resulte = JsonConvert.DeserializeObject(json.results.ToString());
                    if (resulte.Count == 0)
                    {
                        logger.Debug("no resulte for "+ name);
                        return null;
                    }


                    logger.Debug(json.results);
                    if (resulte.Count > 1)
                    {
                        for (int i = 1; i < resulte.Count; i++)
                        {
                            await get_Food_information_api(resulte[i]);
                            if (i == 5)
                            {
                                break;
                            }
                        }

                    }
                    var firstFood = json.results[0];
                    return await get_Food_information_api(firstFood);
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
        public async Task<string> get_Food_information_api(dynamic foodObject)
        {

            HttpClient client = new HttpClient();
            try
            {
                string id = foodObject.id;
                int DBres;
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
                        DBres= await insertFoodByApiToDB(responseBody);
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
        Task<int> insertFoodByApiToDB(string foodByAPi)
        {
            try
            {
                dynamic Foodjson = JsonConvert.DeserializeObject(foodByAPi);
                string imagePath = "https://spoonacular.com/cdn/ingredients_100x100/";

                if(Foodjson.image!= "no.jpg")
                {
                    imagePath += Foodjson.image;
                }
                else
                {
                    imagePath = "";
                }
                //crete new Ingredient in DB
                DB.Ingredients.Add(new Ingredients()
                {
                    name = Foodjson.name,
                    image = imagePath ,
                    api_id = Foodjson.id
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

                string query = $"insert into PartOf_Ingredients values ";
                int categoryId = 0;
                if (categoryName.Count > 0)
                {
                    logger.Debug(categoryName);
                    var DBcaegories = DB.tblCategory.Select(c => new { id = c.id, name = c.name }).ToList();
                    //add all new categories from api
                    for (int i = 0; i < categoryName.Count; i++)
                    {
                        for (int z = 0; z < DBcaegories.Count; z++)
                        {
                            string name = DBcaegories[z].name.ToLower();
                            if (name.Contains(categoryName[i].ToString()))
                            {
                                logger.Debug(DBcaegories[z].ToString());
                                categoryId = DBcaegories[z].id;
                                query += $"({newIngredient.id},{categoryId}),";
                            }
                        }

                    }
                    
                }
                if (categoryId == 0)
                {
                    categoryId = getCategoryId("general");
                    query += $"({newIngredient.id},{categoryId})";
                }
                else
                {
                    query = query.Substring(0, query.Length - 1);
                }

                //if (categoryId == 0)
                //{
                //    //add new category from api
                //    DB.tblCategory.Add(new tblCategory()
                //    {
                //        name = categoryName[0].ToString(),
                //    });
                //    DB.SaveChanges();
                //        categoryId= getCategoryId(categoryName[0].ToString());
                //}



                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                Task<int> res = Task.FromResult(cmd.ExecuteNonQuery());
                if (res.Result < 1)
                {
                    throw new Exception("cannot insert values into tblPartOf_Ingredients");
                }
                DB.SaveChanges();
                return res;
            }
            catch (Exception ex)
            {
               
                logger.Fatal(ex.Message);
                return null;
            }
            finally
            {
                con.Close();
            }
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
