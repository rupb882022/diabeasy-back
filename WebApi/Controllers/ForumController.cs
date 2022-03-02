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

namespace WebApi.Controllers
{
    //[RoutePrefix("api/forun")]
    public class ForumController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        SqlConnection con = new SqlConnection("Data Source=media.ruppin.ac.il;Initial Catalog=bgroup88_test2;Persist Security Info=True;User ID=bgroup88;Password=bgroup88_06613;MultipleActiveResultSets=True;Application Name=EntityFramework");

        public IHttpActionResult Get(string type)
        {
            try
            {
                switch (type)
                {
                    case "all":
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
                        object[] allComents = new object[dt.Rows.Count];

                        object obj;
                        for (int index = 0; index < dt.Rows.Count; index++)
                        {
                            obj = new { id = dt.Rows[index]["id"].ToString(), date_time = dt.Rows[index]["date_time"].ToString(), subject = dt.Rows[index]["subject"].ToString(), value = dt.Rows[index]["value"].ToString(), userName = dt.Rows[index]["userName"].ToString(), userId = dt.Rows[index]["userId"].ToString(), profileimage = dt.Rows[index]["profileimage"].ToString(), Id_Continue_comment = dt.Rows[index]["Id_Continue_comment"].ToString() };
                            allComents[index] = obj;
                        }

                        return Content(HttpStatusCode.OK, allComents);

                    case "subject":
                        List<string> subjects = DB.tblForum.Select(x => x.subject).Distinct().ToList();
                        return Content(HttpStatusCode.OK, subjects);
                    default:
                        return Content(HttpStatusCode.NotFound, type+" is not exist"); 
                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }


        }

        public IHttpActionResult Post(string type,[FromBody] tblForum obj)
        {
            try
            {
                switch (type)
                {
                    case "add_comment":
                        DB.tblForum.Add(obj);
                        DB.SaveChanges();
                        return Created(new Uri(Request.RequestUri.AbsoluteUri + obj.id), obj);
                    default:
                        return Content(HttpStatusCode.NotFound, type + " is not exist");

                }
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

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
                return Content(HttpStatusCode.NotFound,"id="+ id + "of comment is not found");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        public IHttpActionResult Put(int id,[FromBody] tblForum obj )
        {
            try
            {
                tblForum comment = DB.tblForum.SingleOrDefault(x => x.id == id);
                if (comment != null)
                {
                    comment.value = obj.value;
                    comment.subject = obj.subject;
                    DB.SaveChanges();
                    return Content(HttpStatusCode.OK, comment);
                }
                return Content(HttpStatusCode.NotFound, "id=" + id + "of comment is not found");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
