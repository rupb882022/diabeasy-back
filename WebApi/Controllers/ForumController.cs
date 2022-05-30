using diabeasy_back;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.DTO;
using System.Data;
using NLog;
using System.Configuration;


namespace WebApi.Controllers
{
    //[RoutePrefix("api/forun")]
    public class ForumController : ApiController
    {

        diabeasyDBContext DB = new diabeasyDBContext();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
        static Logger logger = LogManager.GetCurrentClassLogger();
        User user=new User();
        [HttpGet]
        [Route("api/Forum")]
        public IHttpActionResult GetAllCommentsDetails()
        {
            try
            {
                string sqlQuery = @"select F.id,date_time,subject,value,
                                            CASE
                                            WHEN P.firstname is not null
                                            THEN P.firstname + ' ' + P.lastname
                                            ELSE D.firstname + ' ' + D.lastname
                                            END as userName,
                                         CASE
                                            WHEN P.id is not null
                                            THEN P.id
                                            ELSE D.id
                                            END as userId,
	                                         CASE
                                            WHEN P.profileimage is not null
                                            THEN P.profileimage
                                            ELSE D.profileimage
                                            END as profileimage,
                                         CASE
                                            WHEN F.Id_Continue_comment is  null
                                            THEN 0
                                            ELSE Id_Continue_comment
                                            END as  Id_Continue_comment 
                        from tblForum F left join tblPatients P on F.Patients_id = P.id
                        left join tblDoctor D on F.Doctor_id = D.id
                        order by subject,Id_Continue_comment";
                SqlDataAdapter adpter = new SqlDataAdapter(sqlQuery, con);
                DataSet ds = new DataSet();
                adpter.Fill(ds, "tblForum");
                DataTable dt = ds.Tables["tblForum"];
                tblForumDto[] allComents = new tblForumDto[dt.Rows.Count];

                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    tblForumDto obj = new tblForumDto()
                    {
                        id = dt.Rows[index]["id"].ToString(),
                        date_time = dt.Rows[index]["date_time"].ToString(),
                        subject = dt.Rows[index]["subject"].ToString(),
                        value = dt.Rows[index]["value"].ToString(),
                        userName = dt.Rows[index]["userName"].ToString(),
                        userId = dt.Rows[index]["userId"].ToString(),
                        profileimage = dt.Rows[index]["profileimage"].ToString(),
                        Id_Continue_comment = dt.Rows[index]["Id_Continue_comment"].ToString()
                    };

                    allComents[index] = obj;
                }

                return Content(HttpStatusCode.OK, allComents);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpGet]
        [Route("api/Forum/GetAllsubjects")]
        public IHttpActionResult GetAllsubjects()
        {
            try
            {
                List<string> subjects = DB.tblForum.Select(x => x.subject).Distinct().ToList();
                return Content(HttpStatusCode.OK, subjects);
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpPost]
        [Route("api/Forum/addComment")]
        public IHttpActionResult Post([FromBody] tblForum obj)
        {
            try
            {
                int writtenBy = 0;
                alert newAlert;
                writtenBy = tblForum.writtenBy(obj);


                //if (obj.Id_Continue_comment != null)
                //{
                //    tblForum tf = DB.tblForum.Where(x => x.id == obj.Id_Continue_comment).SingleOrDefault();
                //    newAlert = new alert()
                //    {
                //        active = true,
                //        getting_user_id = tblForum.writtenBy(tf),
                //        sendding_user_id = writtenBy,
                //        content = "forum-comment",
                //        date_time = obj.date_time
                //    };
                //    DB.alert.Add(newAlert);
                //}
                //else
                //{
                List<tblForum> TB = DB.tblForum.Where(x => x.subject == obj.subject).OrderByDescending(z => z.Patients_id).ToList();
                List<int> Ids = new List<int>();
                for (int i = 0; i < TB.Count; i++)
                {
                    int id = tblForum.writtenBy(TB[i]);
                    if (writtenBy != id && (i == 0 || id != tblForum.writtenBy(TB[i - 1]))) {
                        newAlert = new alert()
                        {
                            active = true,
                            getting_user_id = id,
                            sendding_user_id = writtenBy,
                            content = obj.Id_Continue_comment != null ? "forum-comment" : "forum-subject",
                            date_time = obj.date_time
                        };
                        DB.alert.Add(newAlert);
                        Ids.Add(id);
                    }
                }
                DB.tblForum.Add(obj);
                DB.SaveChanges();
                for (int i = 0; i < Ids.Count; i++)
                {
                    user.PushNotificationNow(Ids[i], "A new comment was added on a subject that you were part of");
                }
                return Created(new Uri(Request.RequestUri.AbsoluteUri), "created");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpDelete]
        [Route("api/Forum/Delete/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                tblForum comment = DB.tblForum.SingleOrDefault(x => x.id == id);
                if (comment != null)
                {
                    List<tblForum> Continue_comments = DB.tblForum.Where(x => x.Id_Continue_comment == id).ToList();
                    if (Continue_comments != null)
                    {
                        foreach (tblForum item in Continue_comments)
                        {
                            DB.tblForum.Remove(item);
                        }

                    }
                    DB.tblForum.Remove(comment);
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, comment);
                }
                return Content(HttpStatusCode.NotFound, "id=" + id + "of comment is not found");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }
        [HttpPut]
        [Route("api/Forum/{id}")]
        public IHttpActionResult Put(int id, [FromBody] tblForum obj)
        {
            try
            {
                tblForum comment = DB.tblForum.SingleOrDefault(x => x.id == id);
                if (comment != null)
                {
                    comment.value = obj.value;
                    comment.subject = obj.subject;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, new { id = comment.id, value = comment.value, subject = comment.subject });
                }
                return Content(HttpStatusCode.NotFound, "id=" + id + "of comment is not found");
            }
            catch (Exception e)
            {
                logger.Fatal(e.Message);
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

//        [HttpGet]
//        [Route("api/Forum/push")]
//        public IHttpActionResult Get()
//{
//            List<int> Ids = new List<int>()
//            {
//                1,3,5
//            };
//                            for (int i = 0; i<Ids.Count; i++)
//                {
//                        dynamic pnd = new 
//                        {
//                            id = Ids[i],
//                            title = "Diabeasy App",
//                            body = "New Comment was added",
//                            badge = 0,
//                            ttl = 3
//                        };

//        user.SendPushNotification(Ids[i], "Diabeasy App", "New Comment was added");
//                }
//            return Content(HttpStatusCode.OK, Ids);

//        }
    }
}
