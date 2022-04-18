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
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        User user = new User();
        Images images = new Images();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
        static Logger logger = LogManager.GetCurrentClassLogger();


        [HttpGet]
        [Route("api/User/Prescription/{id}")]
        public IHttpActionResult Prescription(int id)
        {
            List<tblPrescriptionDto> allPrescriptions = DB.tblPrescriptions.Where(x => x.Patients_id == id).OrderByDescending(x => x.date_time).Select(x => new tblPrescriptionDto() { id = x.id, date_time = x.date_time, subject = x.subject, value = x.value, status = x.status }).ToList();
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
                if (patient == null|| patient.assistant_phone==null)
                    throw new Exception("patient assitent phone is null");

                assisant_phone = patient.assistant_phone.ToString();
               

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
            {
                tblPatients user = DB.tblPatients.Where(x => x.email == email && x.password == password).SingleOrDefault();
                if (user != null)
                {
                    var userDetalis = new { id = user.id, profileimage = user.profileimage, name = user.firstname + " " + user.lastname };
                    return Content(HttpStatusCode.OK, userDetalis);
                }

                tblDoctor Doctoruser = DB.tblDoctor.Where(x => x.email == email && x.password == password).SingleOrDefault();
                if (Doctoruser != null)
                {
                    var userDetalis = new { id = Doctoruser.id, profileimage = Doctoruser.profileimage, name = Doctoruser.firstname + " " + Doctoruser.lastname };
                    return Content(HttpStatusCode.OK, userDetalis);
                }

                return Content(HttpStatusCode.BadRequest, "worng user details");
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
                email = email.Replace("=", ".");
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

        [HttpGet]
        [Route("api/User/Doctor/{id}")]
        public IHttpActionResult PatientBelongToDoctor(int id)
        {
            try
            {
                var Patients = DB.tblPatients.Where(x => x.Doctor_id == id).Select(x => new { id = x.id, firstname = x.firstname, lastname = x.lastname, profileimage = x.profileimage, select = false }).ToList();
                return Content(HttpStatusCode.OK, Patients);
            }
            catch (Exception e)
            {
                logger.Error("no patients found");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }


        [HttpGet]
        [Route("api/User/GetdataForGraphs/{id}")]
        public IHttpActionResult GetDataForGraphs(int id)
        {
            try
            {
                string query = @"select *
						         from(select MONTH(date_time) as 'month',YEAR(date_time)as 'year',AVG(blood_sugar_level) as averge,
								SUM(CASE WHEN blood_sugar_level >240 THEN 1 ELSE 0 end) as 'upTo240',
								SUM( CASE WHEN blood_sugar_level <240 and blood_sugar_level>180 THEN 1 ELSE 0 end) as 'upTo180',
								SUM( CASE WHEN blood_sugar_level <180 and blood_sugar_level>75 THEN 1 ELSE 0 end) as 'upTo75',
								SUM( CASE WHEN blood_sugar_level <75 and blood_sugar_level>60 THEN 1 ELSE 0 end) as 'upTo60',
								SUM( CASE WHEN blood_sugar_level <60  THEN 1 ELSE 0 end) as 'upTo0'
								 from tblPatientData
								 where Patients_id=@id
								 group by MONTH(date_time),YEAR(date_time)
								 union
								 select '30' as 'month',YEAR(getDate())as 'year', AVG(blood_sugar_level),
								 SUM(CASE WHEN blood_sugar_level >240 THEN 1 ELSE 0 end) as 'upTo240',
								SUM( CASE WHEN blood_sugar_level <240 and blood_sugar_level>180 THEN 1 ELSE 0 end) as 'upTo180',
								SUM( CASE WHEN blood_sugar_level <180 and blood_sugar_level>75 THEN 1 ELSE 0 end) as 'upTo75',
								SUM( CASE WHEN blood_sugar_level <75 and blood_sugar_level>60 THEN 1 ELSE 0 end) as 'upTo60',
								SUM( CASE WHEN blood_sugar_level <60  THEN 1 ELSE 0 end) as 'upTo0'
								from tblPatientData
							   where Patients_id=@id and DATEDIFF(day,date_time,GETDATE())between 0 and 30)as data
								order by 'year','month'";

                SqlDataAdapter adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@id", id);

                DataSet ds = new DataSet();
                adpter.Fill(ds, "DataForGraphs");
                DataTable dt = ds.Tables["DataForGraphs"];
                List<GrapsDto> JSONresult = new List<GrapsDto>();
                //string JSONresult = JsonConvert.SerializeObject(dt);
                //JSONresult = JSONresult.Replace("\\", "").Replace("\"", "");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JSONresult.Add(new GrapsDto()
                    {
                        month = (int)dt.Rows[i]["month"],
                        averge = (int)dt.Rows[i]["averge"],
                        upTo240 = dt.Rows[i]["upTo240"].ToString(),
                        upTo180 = dt.Rows[i]["upTo180"].ToString(),
                        upTo75 = dt.Rows[i]["upTo75"].ToString(),
                        upTo60 = dt.Rows[i]["upTo60"].ToString(),
                        upTo0 = dt.Rows[i]["upTo0"].ToString(),
                    });
                }
                return Content(HttpStatusCode.OK, JSONresult);
            }
            catch (Exception e)
            {
                logger.Error("no patients found");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }


 
        }
        //[HttpGet]
        //[Route("api/User/GetdataForTable/{id}")]
        //public IHttpActionResult GetdataForTable(int id)
        //{

        //    try
        //    {
        //        var tableData = DB.tblPatientData.Where(x => x.Patients_id == id).Select(x => new tblPatientDataDto(){ date_time =x.date_time, blood_sugar_level = x.blood_sugar_level, value_of_ingection = (double)x.value_of_ingection, totalCarbs = (double)x.totalCarbs, injection_site = x.injection_site }).OrderByDescending(x=>x.date_time).Take(15).ToList();

        //        return Content(HttpStatusCode.OK, tableData);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error("do not found ");
        //        return Content(HttpStatusCode.BadRequest, e.Message);
        //    }

        //}

        [HttpGet]
        [Route("api/User/GetdataForTable/{id}/{fromDate}/{toDate}")]
        public IHttpActionResult GetdataForTable(int id, DateTime fromDate, DateTime toDate)
        {

            try
            {
                DateTime d = toDate;
                DateTime ToDateEnd=  d.AddDays(1);
                var tableData = DB.tblPatientData.Where(x => x.Patients_id == id && x.date_time>fromDate && x.date_time < ToDateEnd).Select(x => new tblPatientDataDto() { date_time = x.date_time, blood_sugar_level = x.blood_sugar_level, value_of_ingection = (double)x.value_of_ingection, totalCarbs = (double)x.totalCarbs, injection_site = x.injection_site }).OrderByDescending(x => x.date_time).ToList();

                return Content(HttpStatusCode.OK, tableData);
            }
            catch (Exception e)
            {
                logger.Error("do not found ");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }





        [HttpPost]
        [Route("api/User/RegisterUser")]
        public IHttpActionResult RegisterUser([FromBody] UserDto obj)
        {
            try
            {
                string image = "";
                string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");
                //cheack uniqe email
                if (!user.checkUniqeMail(obj.email))
                {
                    return Content(HttpStatusCode.Conflict, "the email is allready exist");
                }

                //will upper first letter in first and last name
                obj.firstName = user.NameToUpper(obj.firstName);
                obj.lastName = user.NameToUpper(obj.lastName);

                //if weight exesist it is patient else doctor
                if (obj.weight != null)
                {
                    Nullable<int> Doctor_id = null;
                    if (obj.mailDoctor != null)
                    {//Todo send mail to doctor+alert
                        Doctor_id = user.checkDoctorMail(obj.mailDoctor);
                    }

                    image = images.CreateNewNameOrMakeItUniqe("profilePatient") + ".jpg";

                    if (images.ImageFileExist(image, rootPath) == null)
                        image = null;



                    DB.tblPatients.Add(new tblPatients()
                    {
                        email = obj.email,
                        firstname = obj.firstName,
                        lastname = obj.lastName,
                        birthdate = obj.BirthDate,
                        password = obj.password,
                        gender = obj.gender,
                        profileimage = image,
                        height = obj.height,
                        weight = obj.weight,
                        InsulinType_id = obj.InsulinType_id,
                        InsulinType_long_id = obj.InsulinType_long_id,
                        assistant_phone = obj.phoneNumber,
                        Doctor_id = Doctor_id,
                    });
                    //todo triger to group user type
                    DB.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri), obj);
                }
                else
                {
                    image = images.CreateNewNameOrMakeItUniqe("profileDoctor") + ".jpg";
                    if (images.ImageFileExist(image, rootPath) == null)
                        image = null;


                    DB.tblDoctor.Add(new tblDoctor()
                    {
                        email = obj.email,
                        firstname = obj.firstName,
                        lastname = obj.lastName,
                        birthdate = obj.BirthDate,
                        password = obj.password,
                        gender = obj.gender,
                        profileimage = image,
                    });
                    DB.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri), obj);
                }


            }
            //handel erorrs from DB like uniqe value, todo send messge back
            catch (DbUpdateException e)
            {
                logger.Fatal("DB Eror- " + e.InnerException.InnerException.Message);
                return Content(HttpStatusCode.BadRequest, e.InnerException.InnerException.Message);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("api/User/Prescription/addRequest")]
        public IHttpActionResult Prescription_addRequest([FromBody] tblPrescriptions obj)
        {
            try
            {
                DateTime d = (DateTime)obj.date_time;
                string requestDate = d.ToString("MMM dd yyyy").Substring(0, 11);
                int amount = DB.tblPrescriptions.Count(x => x.Patients_id == obj.Patients_id && x.date_time.ToString().Substring(0, 11) == requestDate);
                if (amount < 3)
                {
                    DB.tblPrescriptions.Add(obj);
                    DB.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri), "OK");
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Only 3 requests per day, please try again tomorrow.."));
                    //return Content(HttpStatusCode.Forbidden,"Only 3 requests per day, please try again tomorrow");
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPut]
        [Route("api/User/Prescription/{id}")]
        public IHttpActionResult Put(int id, [FromBody] tblPrescriptions obj)
        {
            try
            {
                tblPrescriptions prescription = DB.tblPrescriptions.SingleOrDefault(x => x.id == id);
                if (prescription != null)
                {
                    prescription.status = obj.status;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, new { id = prescription.id, status = prescription.status });
                }
                return Content(HttpStatusCode.NotFound, "id=" + id + "of prescription is not found");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpDelete]
        [Route("api/User/Prescription/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                tblPrescriptions prescription = DB.tblPrescriptions.SingleOrDefault(x => x.id == id);
                if (prescription != null)
                {
                    DB.tblPrescriptions.Remove(prescription);
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, prescription);
                }
                return Content(HttpStatusCode.NotFound, "id=" + id + "of prescription is not found");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }


        [HttpDelete]
        [Route("api/User/GetdataForTable/Delete/{time}")]
        public IHttpActionResult Delete(string time)
        {
            try
            {
                DateTime t = Convert.ToDateTime(time.ToString().Replace("!", ":"));
                tblPatientData p = DB.tblPatientData.SingleOrDefault(x => x.date_time == t);
                logger.Fatal(time);
                if (p != null)
                {
                    DB.tblPatientData.Remove(p);
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, p);
                }
                return Content(HttpStatusCode.NotFound, "time=" + time + "of patient data is not found");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }


        [HttpPost]
        [Route("api/User/InsertData")]
        public IHttpActionResult InsertData([FromBody] tblPatientData PatientDatadata)
        {
            try
            {
                DB.tblPatientData.Add(PatientDatadata);
                DB.SaveChanges();

                return Created(new Uri(Request.RequestUri.AbsoluteUri), PatientDatadata);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
