﻿using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using WebApi.DTO;
using System.Data.Entity.Infrastructure;
using NLog;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        User user = new User();
        static Logger logger = LogManager.GetCurrentClassLogger();


        [HttpGet]
        [Route("api/User/Prescription/{id}")]
        public IHttpActionResult Prescription(int id)
        {
            List<tblPrescriptions> allPrescriptions = DB.tblPrescriptions.Where(x => x.Patients_id == id).OrderByDescending(x => x.date_time).Select(x => new tblPrescriptions() { id=x.id, date_time= x.date_time, subject= x.subject, value= x.value }).ToList();
            return Content(HttpStatusCode.OK, allPrescriptions);
        }

        [HttpGet]
        [Route("api/User/assistant_phone/{id}")]
        public IHttpActionResult assistant_phone(int id)
        {
            string assisant_phone = "";

            try
            {
                tblPatients patient = DB.tblPatients.Where(x => x.id == id).SingleOrDefault();
                if (patient != null)
                {
                    assisant_phone = patient.assistant_phone.ToString();
                }

                return Content(HttpStatusCode.OK, assisant_phone);
            }
            catch (Exception e)
            {
                logger.Error("do not found assistant_phone");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpGet]
        [Route("api/User/userDetails/{email}/{password}")]
        public IHttpActionResult userDetails(string email, string password)
        {
            try
            {//ToDo function for doctor
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
            }
            //handel erorrs from DB like uniqe value
            catch (DbUpdateException e)
            {
                logger.Fatal("DB Eror");
                return Content(HttpStatusCode.BadRequest, e.InnerException.InnerException.Message);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/setNewpassword/{email}/{password}")]
        public IHttpActionResult setNewpassword(string email, string password)
        {
            try
            {//ToDo function for doctor and checnge method to post
                tblPatients Patients = DB.tblPatients.Where(x => x.email == email).SingleOrDefault();
                if (Patients != null)
                {
                    Patients.password = password;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, Patients);
                }
                throw new Exception("do not found user- worng email");

            }
            //handel erorrs from DB like uniqe value
            catch (DbUpdateException e)
            {
                logger.Fatal("DB Eror");
                return Content(HttpStatusCode.BadRequest, e.InnerException.InnerException.Message);
            }
            catch (Exception e)
            {
               logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/getPassword/{email}")]
        [AllowAnonymous]
        public IHttpActionResult getPassword(string email)
        {
            try
            {
                email = email.Replace("=",".");
                Random rnd = new Random();
                string code = rnd.Next(100, 1000).ToString();//generet 3 digits code
                string userType = user.GetTypeByMail(email);

                if (userType == "Patient")
                {
                    code += "1";
                }
                else if (userType == "doctor")
                {
                    code += "0";
                }
                else
                {
                    throw new Exception("do not found email");
                }

                if (user.SendMial(email, "reset password", $"please enter this code-{code} in app to reset password"))
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
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }
        [HttpGet]
        [Route("api/User/getInsulinType")]
        public IHttpActionResult getInsulinType()
        {
            try
            {
                List<tblInsulinTypeDto> Types = DB.tblInsulinType.Select(x => new tblInsulinTypeDto() { id = x.id, name = x.name, type = x.type }).ToList();
                if (Types != null)
                {
                    return Content(HttpStatusCode.OK, Types);
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "no types");
                }

            }
            //handel erorrs from DB like uniqe value
            catch (DbUpdateException e)
            {
                logger.Fatal("DB Eror");
                return Content(HttpStatusCode.BadRequest, e.InnerException.InnerException.Message);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}