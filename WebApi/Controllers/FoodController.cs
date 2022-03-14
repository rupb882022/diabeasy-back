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

namespace WebApi.Controllers
{
    public class FoodController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
        static Logger logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        [Route("api/Food/Category")]
        public IHttpActionResult GetCategory()
        {
            try
            {
                List<tblCategoryDto> allCategory = DB.tblCategory.Select(x => new tblCategoryDto(){ id = x.id, name = x.name }).ToList();
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
        [Route("api/Food/getIngredients")]
        public IHttpActionResult GetIngredients()
        {
            try
            {
                string query = @"select  I.id, I.name as IngrediantName, I.image,C.id as categoryID,C.name as categoryName,UM.id as UM_ID, UM.name as UM_name,UM.image as UM_image, B.carbohydrates, B.sugars,B.weightInGrams
                                 from tblIngredients I inner join tblPartOf_Ingredients TPI on I.id= TPI.Ingredients_id
                                inner join tblCategory C on TPI.Category_id= C.id inner join tblBelong B on B.Ingredients_id= I.id
                                 inner join tblUnitOfMeasure UM on UM.id= B.UnitOfMeasure_id
                                    order by I.id";

                SqlDataAdapter adpter = new SqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                adpter.Fill(ds, "tblIngredients");
                DataTable dt = ds.Tables["tblIngredients"];

                List<IngrediantDto> ingrediants = new List<IngrediantDto>();
                IngrediantDto ingrediant = new IngrediantDto();
  
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i!=0&&(int)dt.Rows[i]["id"] != (int)dt.Rows[i - 1]["id"])
                    {
                        ingrediants.Add(ingrediant);
                    }
                    tblUnitOfMeasureDto Unit = new tblUnitOfMeasureDto()
                    {
                        id = (int)dt.Rows[i]["UM_ID"],
                        name = dt.Rows[i]["UM_name"].ToString(),
                        carbs = float.Parse(dt.Rows[i]["carbohydrates"].ToString()),
                        suger = float.Parse(dt.Rows[i]["sugars"].ToString()),
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
                        };
                        ingrediant.UnitOfMeasure.Add(Unit);
                    }

                    if (ingrediant.category.Count == 0 || (int)dt.Rows[i]["categoryID"] != (int)dt.Rows[i - 1]["categoryID"])
                    {
                        
                        ingrediant.category.Add(new tblCategoryDto() { id = (int)dt.Rows[i]["categoryID"], name = dt.Rows[i]["categoryName"].ToString() });
                    }
                    else if (i == 0 || (int)dt.Rows[i]["id"] == (int)dt.Rows[i - 1]["id"]&& ingrediant.category.Count<=1)
                    {
                        ingrediant.UnitOfMeasure.Add(Unit);
                    }
                }
                //add last ingrediant in query list
                ingrediants.Add(ingrediant);


                return Content(HttpStatusCode.OK, ingrediants);
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
                List<tblUnitOfMeasureDto> unit= DB.tblUnitOfMeasure.Select(x => new tblUnitOfMeasureDto() { id = x.id, name = x.name, image = x.image }).ToList();
                return Content(HttpStatusCode.OK, unit);
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        }
}
