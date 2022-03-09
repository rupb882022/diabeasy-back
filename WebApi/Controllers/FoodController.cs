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

namespace WebApi.Controllers
{
    public class FoodController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);


        public IHttpActionResult Get()
        {
            try
            {
                var allCategory = DB.tblCategory.Select(x => new { id = x.id, name = x.name }).ToList();
                if (allCategory == null)
                {
                    throw new NullReferenceException();
                }
                return Content(HttpStatusCode.OK, allCategory);
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }
        [HttpGet]
        [Route("api/Food/getIngredients")]
        public IHttpActionResult GetIngredients()
        {
            try
            {
                string query = @"select C.name, I.id, I.name as IngrediantName, I.image, UM.name as UnitOfMeasure, B.carbohydrates, B.sugars
                                 from tblIngredients I inner join tblPartOf_Ingredients TPI on I.id= TPI.Ingredients_id
                                inner join tblCategory C on TPI.Category_id= C.id inner join tblBelong B on B.Ingredients_id= I.id
                                 inner join tblUnitOfMeasure UM on UM.id= B.UnitOfMeasure_id
                                    order by C.name";


                
                SqlDataAdapter adpter = new SqlDataAdapter(query,con);
                DataSet ds = new DataSet();
                adpter.Fill(ds, "tblIngredients");
                DataTable dt = ds.Tables["tblIngredients"];

                object[] ingrediant = new object[dt.Rows.Count];

                object obj;
                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    obj = new { CategoryName = dt.Rows[index]["name"].ToString(), id = dt.Rows[index]["id"].ToString(), IngrediantName = dt.Rows[index]["IngrediantName"].ToString(), image = dt.Rows[index]["image"].ToString(), UnitOfMeasure = dt.Rows[index]["UnitOfMeasure"].ToString(), carbohydrates = dt.Rows[index]["carbohydrates"].ToString(), sugars = dt.Rows[index]["sugars"].ToString()};
                    ingrediant[index] = obj;
                }

                return Content(HttpStatusCode.OK, ingrediant);
            }
            catch (Exception e)
            {
            
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }
        //ingrediant details


    }
}
