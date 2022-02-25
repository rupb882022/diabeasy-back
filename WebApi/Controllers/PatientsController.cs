using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    public class PatientsController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        public IHttpActionResult Get(string url, int id)
        {
            string assisant_phone="";

            try
            {
                switch (url)
                {
                    case "assistant_phone":
                        tblPatients patient = DB.tblPatients.Where(x => x.id == id).SingleOrDefault();
                        if (patient != null)
                        {
                            assisant_phone = patient.assistant_phone.ToString();
                        }
                        break;

                    default:
                        return null;
                }
                return Content(HttpStatusCode.OK, assisant_phone);
            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        public IHttpActionResult Get(string url, string email, string password)
        {

            try
            {
                switch (url)
                {
                    case "userDetails":
                        tblPatients user = DB.tblPatients.Where(x => x.email == email && x.password == password).SingleOrDefault();
                        if (user != null)
                        {
                            var userDetalis = new { id = user.id, profileimage = user.profileimage, name = user.firstname + " " + user.lastname };
                            return Content(HttpStatusCode.OK, userDetalis);
                        }
                        else
                        {
                            throw new Exception();
                        }

                    default:
                        break;  
                }
                return null;

            }
            catch (Exception e)
            {

                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }


    }
}
