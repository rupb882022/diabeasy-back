using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data;
using NLog;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using WebApi.DTO;
using Newtonsoft.Json.Linq;
using System.Web;

namespace WebApi.Controllers
{
    public class FoodController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
        static Logger logger = LogManager.GetCurrentClassLogger();
        Images image = new Images();
        User user = new User();
        Food food = new Food();
        [HttpGet]
        [Route("api/Food/Category")]
        public IHttpActionResult GetCategory()
        {
            try
            {
                List<tblCategoryDto> allCategory = DB.tblCategory.Select(x => new tblCategoryDto() { id = x.id, name = x.name }).ToList();
                if (allCategory == null)
                {
                    throw new NullReferenceException();
                }
                return Content(HttpStatusCode.OK, allCategory);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }
        [HttpGet]
        [Route("api/Food/getIngredients/{foodName}/{useId}")]
        public IHttpActionResult GetIngredients(string foodName, int useId)
        {
            try
            {
                string query = @"select  I.id, I.name as IngrediantName, I.image,C.id as categoryID,C.name as categoryName,UM.id as UM_ID, UM.name as UM_name,UM.image as UM_image, B.carbohydrates, B.sugars,B.weightInGrams,I.addByUserId,
                                case WHEN  FI.Ingredient_id is not null then FI.Ingredient_id else 0 END as favorit
								from Ingredients I
								 inner join PartOf_Ingredients TPI on I.id= TPI.Ingredients_id
                                inner join tblCategory C on TPI.Category_id= C.id
								inner join tblBelong B on B.Ingredient_id= I.id
                                 inner join tblUnitOfMeasure UM on UM.id= B.UnitOfMeasure_id
								 left join (select Ingredient_id from tblFavoritesIngredients where Patient_id=@useId) FI on FI.Ingredient_id=I.id
                                 where (I.addByUserId is null or I.addByUserId=@useId) ";

                if (foodName != "all")
                {
                    query += " and I.name like @search ";
                }
                query += " order by I.id,C.id";
                SqlDataAdapter adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@useId", useId);

                if (foodName != "all")
                { adpter.SelectCommand.Parameters.AddWithValue("@search", "%" + foodName + "%"); }

                DataSet ds = new DataSet();
                adpter.Fill(ds, "Ingredients");
                DataTable dt = ds.Tables["Ingredients"];
                List<IngrediantDto> ingrediants = new List<IngrediantDto>();
                IngrediantDto ingrediant = new IngrediantDto();
                if (dt.Rows.Count > 0) { 
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0 && (int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        ingrediants.Add(ingrediant);
                    }
                    tblUnitOfMeasureDto Unit = new tblUnitOfMeasureDto()
                    {
                        id = (int)dt.Rows[i]["UM_ID"],
                        name = dt.Rows[i]["UM_name"].ToString(),
                        carbs = double.Parse(dt.Rows[i]["carbohydrates"].ToString()),
                        suger = double.Parse(dt.Rows[i]["sugars"].ToString()),
                        weightInGrams = float.Parse(dt.Rows[i]["weightInGrams"].ToString()),
                        image = dt.Rows[i]["UM_image"].ToString()
                    };
                    if (i == 0 || (int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        ingrediant = new IngrediantDto()
                        {
                            id = (int)dt.Rows[i]["id"],
                            name = dt.Rows[i]["IngrediantName"].ToString(),
                            image = dt.Rows[i]["image"].ToString(),
                            addByUserId = dt.Rows[i]["addByUserId"].ToString(),
                        };
                        ingrediant.UnitOfMeasure.Add(Unit);
                    }

                    if (ingrediant.category.Count == 0 || (int)dt.Rows[i]["categoryID"] != (int)dt.Rows[i - 1]["categoryID"])
                    {

                        ingrediant.category.Add(new tblCategoryDto() { id = (int)dt.Rows[i]["categoryID"], name = dt.Rows[i]["categoryName"].ToString() });
                    }
                    else if (i == 0 || (int)dt.Rows[i]["id"] == (int)dt.Rows[i - 1]["id"] && ingrediant.category.Count <= 1)
                    {
                        ingrediant.UnitOfMeasure.Add(Unit);
                    }

                    if ((int)dt.Rows[i]["favorit"] != 0 && (i == 0 || (int)dt.Rows[i]["favorit"] != (int)dt.Rows[i - 1]["favorit"]))
                    {
                        ingrediant.category.Add(new tblCategoryDto() { id = 4, name = "Favorites" });
                        ingrediant.favorit = true;
                    }
                }
                //add last ingrediant in query list
                ingrediants.Add(ingrediant);
                }
                else
                {
                    var resulte =  food.search_by_name_api(foodName);
                    dynamic res = JsonConvert.DeserializeObject(resulte.ToString());
                    double carbs=0,suger=0;
                    if (res != null)
                    {
                        for (int i = 0; i < res.nutrition.nutrients.length; i++)
                        {
                            if (res.nutrition.nutrients[i].name== "Carbohydrates")
                            {
                                carbs = double.Parse(res.nutrition.nutrients[i].amount);
                            }
                            if (res.nutrition.nutrients[i].name== "Sugar")
                            {
                                suger = double.Parse(res.nutrition.nutrients[i].amount);
                            }
                        }
                    }
                    ingrediant=new IngrediantDto()
                    {
                        name=foodName,
                        image= res.image
                    };
                    ingrediant.UnitOfMeasure.Add(new tblUnitOfMeasureDto()
                    {
                        name = res.unitLong,
                        carbs = carbs,
                        suger = suger,
                        weightInGrams = int.Parse(res.nutrition.weightPerServing.amount)
                    });
                    ingrediant.category.Add(new tblCategoryDto()
                    {
                        id = 0,
                        name = res.categoryPath[0]
                    });
                }

                return Content(HttpStatusCode.OK, ingrediants);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpGet]
        [Route("api/Food/getRecipes/{foodName}/{useId}")]
        public IHttpActionResult GetRecipes(string foodName, int useId)
        {
            try
            {
                string recipe_ids = "";
                string query = @"select R.*,CF.Ingredient_id,I.name as Ingredients_name,i.image as Ingredients_image, amount,UM.id as Ingredient_MeasureId,UM.name as Ingredient_MeasureName,UM2.id as recipe_Unit_id,UM2.name as recipe_Unit_name,BTR.carbohydrates,BTR.sugars,BTR.weightInGrams
                                 , case WHEN  FI.Recipes_id is not null then FI.Recipes_id else 0 END as favorit
                                from Recipes R 
                                inner join tblConsistOf CF on R.id=CF.Recipe_id
								inner join Ingredients I on I.id=CF.Ingredient_id
                                inner join tblUnitOfMeasure UM on CF.UnitOfMeasure_id=UM.id
                                inner join tblBelongToRecipe BTR on BTR.Recipe_id=R.id 
								inner join  tblUnitOfMeasure UM2 on UM2.id=BTR.UnitOfMeasure_id
	                            left join (select Recipes_id from tblFavoritesRecipes where Patient_id=@useId) FI on FI.Recipes_id=R.id 
                                where (R.addByUserId is null or R.addByUserId= @useId) ";

                if (foodName != "all")
                {
                    query += " and ( R.name like @foodName or I.name like @foodName or cookingMethod like @foodName ) ";
                }
                query += " order by R.id,recipe_Unit_id";
                SqlDataAdapter adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@useId", useId);

                if (foodName != "all")
                { adpter.SelectCommand.Parameters.AddWithValue("@foodName", "%" + foodName + "%"); }

                DataSet ds = new DataSet();
                adpter.Fill(ds, "tblRecipes");
                DataTable dt = ds.Tables["tblRecipes"];
                List<tblRecipesDto> Recipes = new List<tblRecipesDto>();
                tblRecipesDto Recipe = new tblRecipesDto();
                int unitID = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != 0 && (int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        Recipes.Add(Recipe);
                        recipe_ids += $"{dt.Rows[i]["id"]},";
                    }

                    if (i == 0 || (int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        Recipe = new tblRecipesDto()
                        {
                            id = (int)dt.Rows[i]["id"],
                            name = dt.Rows[i]["name"].ToString(),
                            image = dt.Rows[i]["image"].ToString(),
                            totalCarbohydrates = (double)dt.Rows[i]["totalCarbohydrates"],
                            totalsugars = (double)dt.Rows[i]["totalsugars"],
                            totalWeigthInGrams = (double)dt.Rows[i]["totalWeigthInGrams"],
                            cookingMethod = dt.Rows[i]["cookingMethod"].ToString(),
                            addByUserId = dt.Rows[i]["addByUserId"].ToString(),
                        };
                        //set unit id every new recipe,then in the ingrediant if condition it will insert all ingrediant by the first unit in the table
                        unitID = (int)dt.Rows[i]["recipe_Unit_id"];
                    }
                    if (Recipe.UnitOfMeasure.Count == 0 || (int)dt.Rows[i]["recipe_Unit_id"] != (int)dt.Rows[i - 1]["recipe_Unit_id"])
                    {
                        tblUnitOfMeasureDto Unit = new tblUnitOfMeasureDto()
                        {
                            id = (int)dt.Rows[i]["recipe_Unit_id"],
                            name = dt.Rows[i]["recipe_Unit_name"].ToString(),
                            carbs = double.Parse(dt.Rows[i]["carbohydrates"].ToString()),
                            suger = double.Parse(dt.Rows[i]["sugars"].ToString()),
                            weightInGrams = float.Parse(dt.Rows[i]["weightInGrams"].ToString()),
                            //image = dt.Rows[i]["UM_image"].ToString()
                        };

                        Recipe.UnitOfMeasure.Add(Unit);
                    }
                    //will insert all Ingredients of the recipe
                    if (i == 0 || unitID == (int)dt.Rows[i]["recipe_Unit_id"])
                    {
                        IngredientsForRecipeDto ingrediant = new IngredientsForRecipeDto()
                        {
                            id = (int)dt.Rows[i]["Ingredient_id"],
                            name = dt.Rows[i]["Ingredients_name"].ToString(),
                            //image = dt.Rows[i]["Ingredients_image"].ToString(),
                            amount = (double)dt.Rows[i]["amount"],
                            unitId = (int)dt.Rows[i]["Ingredient_MeasureId"],
                            unitName = dt.Rows[i]["Ingredient_MeasureName"].ToString(),
                        };
                        Recipe.Ingrediants.Add(ingrediant);
                    }
                    if ((int)dt.Rows[i]["favorit"] != 0 && (i == 0 || (int)dt.Rows[i]["favorit"] != (int)dt.Rows[i - 1]["favorit"]))
                    {
                        Recipe.category.Add(new tblCategoryDto() { id = 4, name = "Favorites" });
                        Recipe.favorit = true;
                    }
                }
                //add last ingrediant in query listgimer
                Recipes.Add(Recipe);
                recipe_ids += Recipe.id;

                //set the category of recipes
                query = @"select R.id,C.id as categoryID,C.name as categoryName
                        from Recipes R  inner join tblPartOf_Recipes TPR on R.id= TPR.Recipe_id
                        inner join tblCategory C on TPR.Category_id= C.id
                     where(R.addByUserId is null or R.addByUserId = @useId) ";

                if (foodName != "all")
                {
                    query += $" and R.id in ({recipe_ids}) ";
                }
                query += " order by R.id";

                adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@useId", useId);

                adpter.Fill(ds, "tblCategory");
                dt = ds.Tables["tblCategory"];

                int index = -1;//prsent the index of recipe in json
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == 0 || (int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        index++;
                    }
                    Recipes[index].category.Add(new tblCategoryDto()
                    {
                        id = (int)dt.Rows[i]["categoryID"],
                        name = dt.Rows[i]["categoryName"].ToString()
                    });
                }
                return Content(HttpStatusCode.OK, Recipes);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }



        [HttpGet]
        [Route("api/Food/getUnitOfMeasure")]
        public IHttpActionResult GetUnitOfMeasure()
        {
            try
            {
                List<tblUnitOfMeasureDto> unit = DB.tblUnitOfMeasure.Select(x => new tblUnitOfMeasureDto() { id = x.id, name = x.name, image = x.image }).ToList();
                return Content(HttpStatusCode.OK, unit);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("api/Food/AddIngredient")]
        public IHttpActionResult AddIngredient([FromBody] dynamic ingredient)
        {
            try
            {

                JArray categories = (JArray)ingredient.category;
                string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");

                string imageName = image.CreateNewNameOrMakeItUniqe("Ingredient") + ".jpg";
                string name = user.NameToUpper((string)ingredient.name);


                if (image.ImageFileExist(imageName, rootPath) == null)
                    imageName = null;
                else
                    imageName = "http://proj.ruppin.ac.il/bgroup88/prod/uploadFiles/" + imageName;

                DB.Ingredients.Add(new Ingredients() { name = name, image = imageName, addByUserId = ingredient.userId });

                DB.SaveChanges();

                //get the new ingredient for connection tables 
                Ingredients newingredient = DB.Ingredients.OrderByDescending(x => x.id).FirstOrDefault();

                DB.tblBelong.Add(new tblBelong() { UnitOfMeasure_id = ingredient.unit, Ingredient_id = newingredient.id, carbohydrates = ingredient.carbs, sugars = ingredient.suger, weightInGrams = ingredient.weightInGrams });

                Nullable<int> unit_id = food.getUnitID("grams");
                Nullable<double> carbs = food.calc100Grams((int)ingredient.unit, (int)ingredient.weightInGrams, (double)ingredient.carbs);
                Nullable<double> sugar = 0;
                if (ingredient.sugar != null)
                    food.calc100Grams((int)ingredient.unit, (int)ingredient.weightInGrams, (double)ingredient.sugars);


                if (unit_id != null && carbs != null)
                {
                    DB.tblBelong.Add(new tblBelong() { UnitOfMeasure_id = (int)unit_id, Ingredient_id = newingredient.id, carbohydrates = carbs, sugars = sugar, weightInGrams = 100 });
                }


                //insert into category connection
                string query = "insert into PartOf_Ingredients values ";
                for (int i = 0; i < categories.Count; i++)
                {
                    query += $"({newingredient.id},{categories[i]}),";
                }
                query = query.Substring(0, query.Length - 1);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot insert values into tblPartOf_Ingredients");
                }
                DB.SaveChanges();
                return Created(new Uri(Request.RequestUri.AbsoluteUri), "OK");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        [HttpPost]
        [Route("api/Food/AddRecipe")]
        public IHttpActionResult AddRecipe([FromBody] dynamic recpie)
        {
            try
            {

                JArray categories = (JArray)recpie.category;
                JArray Ingridents = (JArray)recpie.Ingridents;
                string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");
                string imageName = image.CreateNewNameOrMakeItUniqe("recipe") + ".jpg";
                string name = user.NameToUpper((string)recpie.name);

                if (image.ImageFileExist(imageName, rootPath) == null)
                    imageName = null;
                else
                    imageName = "http://proj.ruppin.ac.il/bgroup88/prod/uploadFiles/" + imageName;


                //add the new recipe to DB
                DB.Recipes.Add(new Recipes()
                {
                    name = name,
                    image = imageName,
                    totalCarbohydrates = recpie.TotalCarbs,
                    totalsugars = recpie.TotalSuger,
                    totalWeigthInGrams = recpie.TotalGrams,
                    cookingMethod = recpie.cookingMethod,
                    addByUserId = recpie.userId
                });

                DB.SaveChanges();

                //get the new recipe for connection tables 
                Recipes newRecipe = DB.Recipes.OrderByDescending(x => x.id).FirstOrDefault();


                //insert ingredient of recipe to connection table
                //insert the ingridents for recipe
                for (int i = 0; i < Ingridents.Count; i++)
                {
                    string unitName = recpie.Ingridents[i].unit;
                    int unitId = food.getUnitID(unitName);
                    DB.tblConsistOf.Add(new tblConsistOf()
                    {
                        Recipe_id = newRecipe.id,
                        Ingredient_id = (int)recpie.Ingridents[i].id,
                        amount = (int)recpie.Ingridents[i].amount,
                        UnitOfMeasure_id = unitId
                    });
                }
                //insert the details of selected unit
                DB.tblBelongToRecipe.Add(new tblBelongToRecipe()
                {
                    carbohydrates = recpie.carbs,
                    sugars = recpie.suger,
                    weightInGrams = recpie.weightInGrams,
                    Recipe_id = newRecipe.id,
                    UnitOfMeasure_id = (int)recpie.unit
                });

                //unit of recipe
                Nullable<int> unit_id = food.getUnitID("Unit");
                if (unit_id != (int)recpie.unit)
                {
                    DB.tblBelongToRecipe.Add(new tblBelongToRecipe()
                    {
                        UnitOfMeasure_id = (int)unit_id,
                        Recipe_id = newRecipe.id,
                        carbohydrates = recpie.TotalCarbs,
                        sugars = recpie.TotalSuger,
                        weightInGrams = recpie.TotalGrams
                    });
                }
                unit_id = food.getUnitID("grams");

                if (unit_id != (int)recpie.unit)
                {
                    Nullable<double> carbs = food.calc100Grams((int)recpie.unit, (int)recpie.weightInGrams, (double)recpie.carbs);
                    Nullable<double> sugar = 0;
                    if (recpie.sugar != null)
                        food.calc100Grams((int)recpie.unit, (int)recpie.weightInGrams, (double)recpie.sugars);


                    if (unit_id != null && carbs != null)
                    {
                        DB.tblBelongToRecipe.Add(new tblBelongToRecipe()
                        {
                            UnitOfMeasure_id = (int)unit_id,
                            Recipe_id = newRecipe.id,
                            carbohydrates = carbs,
                            sugars = sugar,
                            weightInGrams = 100
                        });
                    }
                }
                //insert into category connection
                string query = "insert into tblPartOf_Recipes values ";
                for (int i = 0; i < categories.Count; i++)
                {
                    query += $"({newRecipe.id},{categories[i]}),";
                }
                query = query.Substring(0, query.Length - 1);
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot insert values into tblPartOf_Recipes");
                }
                DB.SaveChanges();
                return Created(new Uri(Request.RequestUri.AbsoluteUri), "OK");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }



        [HttpPost]
        [Route("api/Food/addFavorites")]
        public IHttpActionResult addFavorites([FromBody] tblFavoritesDto obj)
        {
            try
            {
                string query;

                if (obj.Rcipe_id != null)
                    query = $"insert into tblFavoritesRecipes values(,{(int)obj.Rcipe_id}{(int)obj.user_id})";
                else
                    query = $"insert into tblFavoritesIngredients values({(int)obj.Ingredient_id},{(int)obj.user_id})";

                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot insert values into" + query);
                }
                return Created(new Uri(Request.RequestUri.AbsoluteUri), obj);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        [HttpDelete]
        [Route("api/Food/deleteFavorites")]
        public IHttpActionResult deleteFavorites([FromBody] tblFavoritesDto obj)
        {
            try
            {
                string query;

                if (obj.Rcipe_id != null)
                    query = $"delete from tblFavoritesRecipes where Recipes_id={obj.Rcipe_id} and Patient_id={obj.user_id}";
                else
                    query = $"delete from tblFavoritesIngredients where Ingredient_id={obj.Ingredient_id} and Patient_id={obj.user_id}";

                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot insert values into" + query);
                }
                return Content(HttpStatusCode.OK, obj);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        [HttpDelete]
        [Route("api/Food/deleteIngredient/{id}")]
        public IHttpActionResult deleteIngredient(int id)
        {
            try
            {
                string query = @" delete from PartOf_Ingredients where Ingredients_id=@id

                                 delete from tblBelong where Ingredient_id =@id

                                 delete from tblFavoritesIngredients where Ingredient_id = @id

                                 delete from Ingredients where id = @id";

                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot delete " + query);
                }
                return Content(HttpStatusCode.OK, id + " was deleted");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        [HttpDelete]
        [Route("api/Food/deleteRecipe/{id}")]
        public IHttpActionResult deleteRecipe(int id)
        {
            try
            {
                string query = @" 
								delete from tblConsistOf where Recipe_id=@id
								delete from tblBelongToRecipe where Recipe_id=@id
								delete from tblFavoritesRecipes where Recipes_id=@id
								delete from tblPartOf_Recipes where Recipe_id=@id
								delete from Recipes where id=@id";

                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                int res = cmd.ExecuteNonQuery();
                if (res < 1)
                {
                    throw new Exception("cannot delete " + query);
                }
                return Content(HttpStatusCode.OK, id + " was deleted");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
            finally
            {
                con.Close();
            }
        }
        [HttpPost]
        [Route("api/Food/addunit/{foodID}")]
        public IHttpActionResult addunit([FromBody] tblUnitOfMeasureDto unit,int foodID)
        {
            try
            {
                if (foodID % 2 == 0)
                {
                    DB.tblBelongToRecipe.Add(new tblBelongToRecipe()
                    {
                        carbohydrates = unit.carbs,
                        sugars = unit.suger,
                        weightInGrams = int.Parse(unit.weightInGrams.ToString()),
                        UnitOfMeasure_id = unit.id,
                        Recipe_id = foodID
                    });

                }
                else
                {
                    DB.tblBelong.Add(new tblBelong()
                    {
                        Ingredient_id = foodID,
                        UnitOfMeasure_id = unit.id,
                        carbohydrates = unit.carbs,
                        sugars = unit.suger,
                        weightInGrams = int.Parse(unit.weightInGrams.ToString())
                    });
                }

                DB.SaveChanges();
                return Created(new Uri(Request.RequestUri.AbsoluteUri), unit);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpGet]
        [Route("api/Food/test/{foodName}")]
        public IHttpActionResult test(string foodName)
        {
            try
            {
                var res =  food.search_by_name_api(foodName);

                if (res!=null)
                {
                    return Content(HttpStatusCode.OK, res);
                }
                return Content(HttpStatusCode.BadRequest, res);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
