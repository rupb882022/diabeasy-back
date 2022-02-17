using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();

        
        // GET api/values
        [HttpGet]
        public IEnumerable <string> Get()
        {
            try
            {
                List<string> s = DB.tblPatients.Select(x => x.firstname + " " + x.lastname + "  email:" + x.email).ToList();
                //return Content(HttpStatusCode.OK,s);
                return s;
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
