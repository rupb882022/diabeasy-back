using diabeasy_back;
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
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class UserController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        User user = new User();
        Images images = new Images();
        Food food = new Food();
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
        [Route("api/User/editPersonalInfo/{id}")]
        public IHttpActionResult GetPersonalInfo(int id)
        {
            try
            {
                tblPatients Pat = DB.tblPatients.SingleOrDefault(x => x.id == id);
                if (Pat != null)
                {
                    var DocMail = Pat.Doctor_id;
                    var d = DB.tblDoctor.Where(x => x.id == DocMail).Select(x => x.email);
                    //  tblDoctor d = DB.tblDoctor.Where(x=>x.id ==id).Select(x=>x.email);
                    var P = DB.tblPatients.Where(x => x.id == id).Select(x => new { id = x.id, firstname = x.firstname, lastname = x.lastname, gender = x.gender, birthdate = x.birthdate, weight = x.weight, height = x.height, InsulinType_id = x.InsulinType_id, InsulinType_long_id = x.InsulinType_long_id, docEmail = d });
                    return Content(HttpStatusCode.OK, P);
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, " not exist");
                }
            }
            catch (Exception e)
            {
                logger.Error("GET Personal info to edit " + e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/alert/{id}")]
        public IHttpActionResult getAlert(int id)
        {
            try
            {
                string query = @"select *,case when DATEDIFF(HOUR,date_time,GETDATE())<24 then DATEDIFF(HOUR,date_time,GETDATE())
                                    when DATEDIFF(HOUR,date_time,GETDATE())<(24*7) then convert (int,DATEDIFF(HOUR,date_time,GETDATE()))/24
                                    when DATEDIFF(HOUR,date_time,GETDATE())<(24*7*4) then convert (int,DATEDIFF(HOUR,date_time,GETDATE()))/24/7
                                    else convert (int,DATEDIFF(HOUR,date_time,GETDATE()))/24/30 end 'daysLeft',

                                   case when DATEDIFF(HOUR,date_time,GETDATE())<24 then 'Hours'
                                   when DATEDIFF(HOUR,date_time,GETDATE())<(24*7) then 'Days'
                                   when DATEDIFF(HOUR,date_time,GETDATE())<(24*7*4) then 'Weeks'
                                    else 'Mounts' end 'daysLeftName'
                                from alert a inner join (
                                select id,profileimage,firstname+' '+lastname as 'name'
                                from tblPatients
                                union
                                select id,profileimage,firstname+' '+lastname as 'name'
                                from tblDoctor
                                )as users on a.sendding_user_id=users.id
                                where getting_user_id=@id
                                order by active desc, date_time desc";

                SqlDataAdapter adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@id", id);
                DataSet ds = new DataSet();
                adpter.Fill(ds, "alerts");
                DataTable dt = ds.Tables["alerts"];
                return Content(HttpStatusCode.OK, dt);
            }
            catch (Exception e)
            {

                logger.Error("getAlerte");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpGet]
        [Route("api/User/readAlert/{id}")]
        public IHttpActionResult readAlert(int id)
        {
            try
            {
                alert a = DB.alert.Where(x => x.id == id).SingleOrDefault();

                if (a != null)
                {
                    a.active = false;
                    DB.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri), a.sendding_user_id);
                }
                else
                {
                    throw new Exception("cannot find alert id");
                }


            }
            catch (Exception e)
            {

                logger.Error("delete alert " + e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/assistant_phone/{id}")]
        public IHttpActionResult assistant_phone(int id)
        {
            string assisant_phone = "";

            try
            {
                tblPatients patient = DB.tblPatients.Where(x => x.id == id).SingleOrDefault();
                if (patient == null || patient.assistant_phone == null)
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

        [HttpPost]
        [Route("api/User/assistant_phone/{id}/{number}")]
        public IHttpActionResult AddNewEmergancyPhone(int id, string number)
        {
            // string assisant_phone = "";

            try
            {
                tblPatients patient = DB.tblPatients.Where(x => x.id == id).SingleOrDefault();
                if (patient != null)
                {
                    patient.assistant_phone = int.Parse(number);
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, number);
                }
                return Content(HttpStatusCode.NotFound, "not found");

            }
            catch (Exception e)
            {
                logger.Error("do not found assistant_phone");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }



        }
        [HttpPost]
        [Route("api/User/HistoryLog")]
        public IHttpActionResult insertHistoryLog([FromBody] HistoryLogDto obj)
        {

            try
            {
                tblHistorylog H = DB.tblHistorylog.Where(x => x.Patients_id == obj.Patients_id && x.Historylog_key == obj.Historylog_key).SingleOrDefault();
                if (H != null)
                {
                    H.date_time = DateTime.Now;
                    H.Historylog_value = obj.Historylog_value;
                }
                else
                {
                    H = new tblHistorylog()
                    {
                        date_time = DateTime.Now,
                        Historylog_key = obj.Historylog_key,
                        Historylog_value = obj.Historylog_value,
                        Patients_id = obj.Patients_id,
                    };
                    DB.tblHistorylog.Add(H);
                }
                DB.SaveChanges();
                return Content(HttpStatusCode.Created, "created");

            }
            catch (Exception e)
            {
                logger.Error(e.Message + " \n" + e.InnerException);
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

        [HttpPut]
        [Route("api/User/setNewpassword")]
        public IHttpActionResult setNewpassword([FromBody] UserDto User)
        {
            try
            {
                string email = User.email.Replace("=", ".");

                tblPatients Patients = DB.tblPatients.Where(x => x.email == email).SingleOrDefault();
                if (Patients != null)
                {
                    Patients.password = User.password;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, "save");
                }
                else
                {
                    tblDoctor doctor = DB.tblDoctor.Where(x => x.email == email).SingleOrDefault();
                    if (doctor != null)
                    {
                        doctor.password = User.password;
                        DB.SaveChanges();
                        return Content(HttpStatusCode.OK, "save");
                    }
                    throw new Exception("do not found user- worng email");
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
        [Route("api/User/GetInjectionRecommend/{id}/{blood_sugar_level}/{injectionType}")]
        public IHttpActionResult GetInjectionRecommend(int id, int blood_sugar_level, string injectionType)
        {
            try
            {

                dynamic res = user.GetInjectionRecommend(id, blood_sugar_level, injectionType);


                if (res == null)
                {
                    throw new Exception("no query resulte");
                }

                return Content(HttpStatusCode.OK, res);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/GetLastBloodTest/{id}")]
        public IHttpActionResult GetLastBloodTest(int id)
        {
            try
            {

            tblPatientData  pd= DB.tblPatientData.Where(x => x.Patients_id== id).OrderByDescending(z=>z.date_time).FirstOrDefault();

                if (pd == null)
                {
                    throw new Exception("cannot find Patients_id the id is:" + id);
                }
              
                return Content(HttpStatusCode.OK, pd.date_time);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }


        [HttpGet]
        [Route("api/User/GetdataForGraphs/{id}")]
        public IHttpActionResult GetDataForGraphs(int id)
        {
            try
            {
                string query = @" select month,year,upTo240,upTo180,upTo75,upTo60,upTo0,averge,
 (case when CONVERT(int,C0008)>0 then CONVERT(int,B0008)/CONVERT(int,C0008)else 0 end ) as 'H00:00-08:00',
 (case when CONVERT(int,C0814)>0 then CONVERT(int,B0814)/CONVERT(int,C0814)else 0 end)as 'H08:00-14:00',
  (case when CONVERT(int,C1420)>0 then CONVERT(int,B1420)/CONVERT(int,C1420)else 0 end)as 'H14:00-20:00',
 (case when CONVERT(int,C2000)>0 then CONVERT(int,B2000)/CONVERT(int,C2000)else 0 end)as 'H20:00-00:00'
						        from(select MONTH(date_time) as 'month',YEAR(date_time)as 'year',AVG(blood_sugar_level) as 'averge',
								SUM(CASE WHEN blood_sugar_level >240 THEN 1 ELSE 0 end) as 'upTo240',
								SUM( CASE WHEN blood_sugar_level <240 and blood_sugar_level>180 THEN 1 ELSE 0 end) as 'upTo180',
								SUM( CASE WHEN blood_sugar_level <180 and blood_sugar_level>75 THEN 1 ELSE 0 end) as 'upTo75',
								SUM( CASE WHEN blood_sugar_level <75 and blood_sugar_level>60 THEN 1 ELSE 0 end) as 'upTo60',
								SUM( CASE WHEN blood_sugar_level <60  THEN 1 ELSE 0 end) as 'upTo0',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'00:00:00:000') AND CONVERT(time,'8:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B0008',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'00:00:00:000') AND CONVERT(time,'8:00:00:000')
								THEN 1 ELSE 0 end) as 'C0008',
								sum(CASE WHEN cast(date_time as time) between CONVERT(time,'8:00:00:000') AND CONVERT(time,'14:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B0814',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'8:00:00:000') AND CONVERT(time,'14:00:00:000')
								THEN 1 ELSE 0 end) as 'C0814',
									sum(CASE WHEN cast(date_time as time) between CONVERT(time,'14:00:00:000') AND CONVERT(time,'20:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B1420',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'14:00:00:000') AND CONVERT(time,'20:00:00:000')
								THEN 1 ELSE 0 end) as 'C1420',
								sum(CASE WHEN cast(date_time as time) between CONVERT(time,'20:00:00:000') AND CONVERT(time,'23:59:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B2000',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'20:00:00:000') AND CONVERT(time,'23:59:00:000')
								THEN 1 ELSE 0 end) as 'C2000'
								 from tblPatientData
								 where Patients_id=@id
								 group by MONTH(date_time),YEAR(date_time)
								 union
								 select '30' as 'month',YEAR(getDate())as 'year', AVG(blood_sugar_level),
								 SUM(CASE WHEN blood_sugar_level >240 THEN 1 ELSE 0 end) as 'upTo240',
								SUM( CASE WHEN blood_sugar_level <240 and blood_sugar_level>180 THEN 1 ELSE 0 end) as 'upTo180',
								SUM( CASE WHEN blood_sugar_level <180 and blood_sugar_level>75 THEN 1 ELSE 0 end) as 'upTo75',
								SUM( CASE WHEN blood_sugar_level <75 and blood_sugar_level>60 THEN 1 ELSE 0 end) as 'upTo60',
								SUM( CASE WHEN blood_sugar_level <60  THEN 1 ELSE 0 end) as 'upTo0',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'00:00:00:000') AND CONVERT(time,'8:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B0008',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'00:00:00:000') AND CONVERT(time,'8:00:00:000')
								THEN 1 ELSE 0 end) as 'C0008',
								sum(CASE WHEN cast(date_time as time) between CONVERT(time,'8:00:00:000') AND CONVERT(time,'14:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B0814',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'8:00:00:000') AND CONVERT(time,'14:00:00:000')
								THEN 1 ELSE 0 end) as 'C0814',
									sum(CASE WHEN cast(date_time as time) between CONVERT(time,'14:00:00:000') AND CONVERT(time,'20:00:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B1420',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'14:00:00:000') AND CONVERT(time,'20:00:00:000')
								THEN 1 ELSE 0 end) as 'C1420',
								sum(CASE WHEN cast(date_time as time) between CONVERT(time,'20:00:00:000') AND CONVERT(time,'23:59:00:000')
								THEN blood_sugar_level ELSE 0 end) as 'B2000',
								sum(CASE WHEN cast(date_time as time) between  CONVERT(time,'20:00:00:000') AND CONVERT(time,'23:59:00:000')
								THEN 1 ELSE 0 end) as 'C2000'
								from tblPatientData
							   where Patients_id=@id and DATEDIFF(day,date_time,GETDATE())between 0 and 30)as data
								order by 'year','month' ";

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
                    if (dt.Rows[i]["averge"].GetType() == 0.GetType())
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

                            H0 = dt.Rows[i]["H00:00-08:00"].ToString(),
                            H14 = dt.Rows[i]["H14:00-20:00"].ToString(),
                            H8 = dt.Rows[i]["H08:00-14:00"].ToString(),
                            H20 = dt.Rows[i]["H20:00-00:00"].ToString()
                        });
                    }
                }
                query = @"select(46.7 + AVG(blood_sugar_level)) / 28.7 as a1c
                            from tblPatientData
                        where Patients_id = @id and DATEDIFF(day, date_time, GETDATE())< 90";
                adpter = new SqlDataAdapter(query, con);
                adpter.SelectCommand.Parameters.AddWithValue("@id", id);

                adpter.Fill(ds, "a1c");
                dt = ds.Tables["a1c"];
                var res = new { data = JSONresult, a1c = dt.Rows[0]["a1c"] };
                return Content(HttpStatusCode.OK, res);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/User/GetdataForTable/{id}/{fromDate}/{toDate}")]
        public IHttpActionResult GetdataForTable(int id, DateTime fromDate, DateTime toDate)
        {

            try
            {
                DateTime d = toDate;
                DateTime ToDateEnd = d.AddDays(1);
                var tableData = DB.tblPatientData.Where(x => x.Patients_id == id && x.date_time > fromDate && x.date_time < ToDateEnd).Select(x => new tblPatientDataDto() { date_time = x.date_time, blood_sugar_level = x.blood_sugar_level, value_of_ingection = (double)x.value_of_ingection, totalCarbs = (double)x.totalCarbs, injection_site = x.injection_site }).OrderByDescending(x => x.date_time).ToList();

                return Content(HttpStatusCode.OK, tableData);
            }
            catch (Exception e)
            {
                logger.Error("do not found ");
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }


        [HttpGet]
        [Route("api/User/Get_all_ExceptionalEvent")]
        public IHttpActionResult Get_all_ExceptionalEvent()
        {

            try
            {
                List<tblExceptionalEventDto> E = DB.tblExceptionalEvent.Select(x => new tblExceptionalEventDto() { id = x.id, name = x.name, date = x.date_time }).ToList();
                return Content(HttpStatusCode.OK, E);
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
                if (amount <= 3)
                {

                    int patientID = Convert.ToInt32(obj.Patients_id);
                    tblPatients p = DB.tblPatients.SingleOrDefault(x => x.id == patientID);
                    int docID = Convert.ToInt32(p.Doctor_id);
                    obj.Doctor_id = docID;
                    alert alert = new alert()
                    {
                        active = true,
                        getting_user_id = docID,
                        sendding_user_id = patientID,
                        content = "addPrescription",
                        date_time = (DateTime)obj.date_time
                    };
                    DB.alert.Add(alert);
                    DB.tblPrescriptions.Add(obj);
                    DB.SaveChanges();

                    user.PushNotificationNow(docID, $"You got a new prescription request from {p.firstname + " " + p.lastname}");

                    return Created(new Uri(Request.RequestUri.AbsoluteUri), "  --OKKK");
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
        [HttpPost]
        [Route("api/User/addToken/{id}")]
        public IHttpActionResult user_addToken(int id, [FromBody] tblPatients obj)
        {
            try
            {
                tblPatients p = DB.tblPatients.SingleOrDefault(x => x.id == id);
                if (p != null)
                {
                    //  DB.tblPrescriptions.Add(obj);
                    p.pushtoken = obj.pushtoken;
                    DB.SaveChanges();
                    return Created(new Uri(Request.RequestUri.AbsoluteUri), "OK");
                }
                else
                {
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Not found"));
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
                    alert alert = new alert()
                    {
                        active = true,
                        getting_user_id = obj.Patients_id,
                        sendding_user_id = obj.Doctor_id,
                        content = "statusPrescription",
                        date_time = (DateTime)obj.date_time
                    };
                    DB.alert.Add(alert);
                    DB.SaveChanges();
                    int patientID = Convert.ToInt32(prescription.Patients_id);
                    user.PushNotificationNow(patientID, $"New update for your prescription request from {Convert.ToDateTime(prescription.date_time).ToString("MMM-dd-yyyy").Substring(0, 11)}");
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


        [HttpPut]
        [Route("api/User/updateTableRow")]
        public IHttpActionResult Put([FromBody] tblPatientData obj)
        {
            try
            {
                // DateTime t = Convert.ToDateTime(obj.date_time.ToString().Replace("!", ":"));
                DateTime t = Convert.ToDateTime(obj.date_time);
                tblPatientData data = DB.tblPatientData.SingleOrDefault(x => x.date_time == t);
                logger.Fatal(data);

                if (data != null)
                {
                    data.Patients_id = obj.Patients_id;
                    data.date_time = obj.date_time;
                    data.blood_sugar_level = obj.blood_sugar_level;
                    data.injectionType = obj.injectionType;
                    data.value_of_ingection = obj.value_of_ingection;
                    data.totalCarbs = obj.totalCarbs;
                    data.injection_site = obj.injection_site;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, data.date_time);
                }
                return Content(HttpStatusCode.NotFound, "time=" + data.date_time + "of patient data is not found");
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
        [Route("api/User/deleteTableRow/{time}/{userId}")]
        public IHttpActionResult Delete(string time,int userId)
        {
            try
            {
                DateTime t = Convert.ToDateTime(time.ToString().Replace("!", ":"));
                tblPatientData p = DB.tblPatientData.SingleOrDefault(x => x.date_time == t && x.Patients_id == userId);
                if (p != null)
                {
                   List <tblEventOf> E= DB.tblEventOf.Where(x => x.date_time == t && x.Patients_id == userId).ToList();
                    if (E != null && E.Count > 0)
                    {
                        foreach (tblEventOf e in E)
                        {
                            DB.tblEventOf.Remove(e);
                        }
                    }
                    List <tblATE_Ingredients> AI= DB.tblATE_Ingredients.Where(x => x.date_time == t&&x.Patients_id== userId).ToList();
                    if (AI != null && AI.Count > 0)
                    {
                        foreach(tblATE_Ingredients ai in AI)
                        {
                            DB.tblATE_Ingredients.Remove(ai);
                        }  
                    }
                    List<tblATE_Recipes> AR = DB.tblATE_Recipes.Where(x => x.date_time == t && x.Patients_id == userId).ToList();
                    if (AI != null && AI.Count > 0)
                    {
                        foreach (tblATE_Recipes ar in AR)
                        {
                            DB.tblATE_Recipes.Remove(ar);
                        }
                    }
                    DB.SaveChanges();
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
        public IHttpActionResult InsertData([FromBody] tblPatientDataDto PatientDatadata)
        {
            try
            {
                //case that user add food details
                if (PatientDatadata.food.Count > 0)
                {
                    for (int i = 0; i < PatientDatadata.food.Count; i++)
                    {
                        if ((int)PatientDatadata.food[i].foodId % 2 == 0)
                        {
                            DB.tblATE_Recipes.Add(new tblATE_Recipes()
                            {
                                Patients_id = (int)PatientDatadata.Patients_id,
                                date_time = PatientDatadata.date_time,
                                Recipe_id = (int)PatientDatadata.food[i].foodId,
                                amount = PatientDatadata.food[i].amount,
                                UnitOfMeasure_id = food.getUnitID(PatientDatadata.food[i].unitName)
                            });
                        }
                        else
                        {
                            DB.tblATE_Ingredients.Add(new tblATE_Ingredients()
                            {
                                Patients_id = (int)PatientDatadata.Patients_id,
                                date_time = PatientDatadata.date_time,
                                Ingredient_id = (int)PatientDatadata.food[i].foodId,
                                amount = PatientDatadata.food[i].amount,
                                UnitOfMeasure_id = food.getUnitID(PatientDatadata.food[i].unitName)
                            });
                        }
                    }
                }

                tblPatientData p = new tblPatientData()
                {
                    date_time = PatientDatadata.date_time,
                    blood_sugar_level = PatientDatadata.blood_sugar_level,
                    totalCarbs = PatientDatadata.totalCarbs,
                    injectionType = PatientDatadata.injectionType,
                    injection_site = PatientDatadata.injection_site,
                    value_of_ingection = PatientDatadata.value_of_ingection,
                    Patients_id = (int)PatientDatadata.Patients_id,
                    system_recommendations = PatientDatadata.system_recommendations
                };
                if (PatientDatadata.ExceptionalEvent != null && PatientDatadata.ExceptionalEvent.Count > 0)
                {

                    for (int i = 0; i < PatientDatadata.ExceptionalEvent.Count; i++)
                    {
                        DB.tblEventOf.Add(new tblEventOf()
                        {
                            Patients_id = (int)PatientDatadata.Patients_id,
                            date_time = PatientDatadata.date_time,
                            ExceptionalEvent_id = (int)PatientDatadata.ExceptionalEvent[i],
                            Email = "1"
                        });
                    }
                }

                DB.tblPatientData.Add(p);
                DB.SaveChanges();
                //is good food
                //if(PatientDatadata.blood_sugar_level>75 && PatientDatadata.blood_sugar_level <= 155)
                //{
                //   tblPatientData pd= DB.tblPatientData.Where(x => x.Patients_id == (int)PatientDatadata.Patients_id).OrderByDescending(z => z.date_time).FirstOrDefault();
                //    if (pd != null&&pd.value_of_ingection!=null&& pd.value_of_ingection>0)
                //    {
                //        double diifhour = (pd.date_time - PatientDatadata.date_time).TotalHours;
                //        if(diifhour>=2&& diifhour <= 4)
                //        {

                //        }
                //    }
                //}

                return Created(new Uri(Request.RequestUri.AbsoluteUri), PatientDatadata);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.InnerException);
            }
        }



        //[HttpPost]
        //[Route("api/User/test")]
        //public IHttpActionResult test([FromBody] tblPatientDataDto PatientDatadata)
        //{
        //    try
        //    {
        //        user.MLRecommend((int)PatientDatadata.blood_sugar_level, (double)PatientDatadata.totalCarbs, DateTime.Now);


        //        return Created(new Uri(Request.RequestUri.AbsoluteUri), PatientDatadata);
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Fatal(e.Message);
        //        return Content(HttpStatusCode.BadRequest, e.InnerException);
        //    }

        //}




    }
}
