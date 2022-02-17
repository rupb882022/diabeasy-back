using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class FoodController :  ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();

        
        public IHttpActionResult Get()
        {
            try
            {
                var allCategory = DB.tblCategory.Select(x => new { id = x.id, name = x.name }).ToList();
                if (allCategory==null)
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
    }
}
