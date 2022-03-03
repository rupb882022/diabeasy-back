using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Text;

namespace WebApi.Controllers
{
    public class PatientsController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        User user = new User();
        public IHttpActionResult Get(string url, int id)
        {
            string assisant_phone = "";

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
            {//ToDo function for doctor
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
                    case "setNewpassword":
                        tblPatients Patients = DB.tblPatients.Where(x => x.email == email).SingleOrDefault();
                        if (Patients != null)
                        {
                            Patients.password = password;
                            DB.SaveChanges();
                            return Content(HttpStatusCode.OK, Patients);
                        }
                        throw new Exception("do not found user- worng email");

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
        public IHttpActionResult Get(string url, string mail)
        {

            try
            {

                switch (url)
                {
                    case "getPassword":
                        try
                        {
                            Random rnd = new Random();
                            string code = rnd.Next(100, 1000).ToString();//generet 3 digits code
                            string userType=user.getTypeByMail(mail);

                            if (userType == "Patient")
                            {
                                code += "1";
                            } else if(userType== "doctor")
                            {
                                code += "0";
                            }
                            else
                            {
                                throw new Exception("do not found email");
                            }

                            bool reqest = user.sendEmial(mail, "reset password", $"please enter this code-{code} in app to reset password");
                            if (reqest)
                            {
                                return Content(HttpStatusCode.OK, code);
                            }
                            else
                            {
                                throw new Exception("email is not send");
                            }


                        }
                        catch (Exception e)
                        {

                            return Content(HttpStatusCode.BadRequest, e.Message);
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
