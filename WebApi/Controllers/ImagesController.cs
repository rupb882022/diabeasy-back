﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using diabeasy_back;

namespace WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class ImagesController : ApiController
    {
     diabeasyDBContext DB = new diabeasyDBContext();
        //just for trying
        [HttpGet]
        public IEnumerable<string> Get()
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


        [Route("api/uploadpicture")]
        public Task<HttpResponseMessage> Post()
        {
            string outputForNir = "start---";
            List<string> savedFilePath = new List<string>();
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //Where to put the picture on server  ...MapPath("~/TargetDir")
            string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");
            var provider = new MultipartFileStreamProvider(rootPath);
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData item in provider.FileData)
                    {
                        try
                        {
                            outputForNir += " ---here";
                            string name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                            outputForNir += " ---here2=" + name;

                            //need the guid because in react native in order to refresh an inamge it has to have a new name
 //                          string newFileName = Path.GetFileNameWithoutExtension(name) + "_" + CreateDateTimeWithValidChars() + Path.GetExtension(name);
                            string newFileName = CreateNewNameOrMakeItUniqe(Path.GetFileNameWithoutExtension(name)) + Path.GetExtension(name);

                            //string newFileName = Path.GetFileNameWithoutExtension(name) + "_" + Guid.NewGuid() + Path.GetExtension(name);
                            //string newFileName = name + "" + Guid.NewGuid();
                            outputForNir += " ---here3" + newFileName;

                            //delete all files begining with the same name
                            string[] names = Directory.GetFiles(rootPath);
                            foreach (var fileName in names)
                            {
                                if (Path.GetFileNameWithoutExtension(fileName).IndexOf(Path.GetFileNameWithoutExtension(name)) != -1)
                                {
                                    File.Delete(fileName);
                                }
                            }

                            //File.Move(item.LocalFileName, Path.Combine(rootPath, newFileName));
                            File.Copy(item.LocalFileName, Path.Combine(rootPath, newFileName), true);
                            File.Delete(item.LocalFileName);
                            outputForNir += " ---here4";

                            Uri baseuri = new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.PathAndQuery, string.Empty));
                            outputForNir += " ---here5";
                            string fileRelativePath = "~/uploadFiles/" + newFileName;
                            outputForNir += " ---here6 imageName=" + fileRelativePath;
                            Uri fileFullPath = new Uri(baseuri, VirtualPathUtility.ToAbsolute(fileRelativePath));
                            outputForNir += " ---here7" + fileFullPath.ToString();
                            savedFilePath.Add(fileFullPath.ToString());
                        }
                        catch (Exception ex)
                        {
                            outputForNir += " ---excption=" + ex.Message;
                            return Request.CreateResponse(HttpStatusCode.BadRequest, outputForNir);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.Created, "nirchen " + savedFilePath[0] + "!" + provider.FileData.Count + "!" + outputForNir + ":)");
                });
            return task;
        }

        //private string CreateDateTimeWithValidChars()
        //{
        //    return DateTime.Now.ToString().Replace('/', '_').Replace(':', '-').Replace(' ', '_');
        //}

        private string CreateNewNameOrMakeItUniqe(string name)
        {
            string start = name;//Path.GetFileNameWithoutExtension(name);
                                // string end = Path.GetExtension(name);
            try
            {
                switch (name)
                {
                    case "profileDoctor":
                        tblDoctor doctor = DB.tblDoctor.OrderByDescending(x => x.id).FirstOrDefault();
                        int newDoctorId = doctor.id + 2;
                        return start + newDoctorId.ToString();

                    case "profilePatient":
                        tblPatients patient = DB.tblPatients.OrderByDescending(x => x.id).FirstOrDefault();
                        int newPatientId = patient.id + 2;
                        return start + newPatientId.ToString();

                    // case for ingredients and recipes.

                    default:
                        return start;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }


        }
    }


}

