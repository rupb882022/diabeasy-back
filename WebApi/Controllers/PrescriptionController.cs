﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using diabeasy_back;


namespace WebApi.Controllers
{
    public class PrescriptionController : ApiController
    {
        diabeasyDBContext DB = new diabeasyDBContext();

        // GET: api/Prescription
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("api/Prescription/{id}")]
        // GET: api/Prescription/5
        public IHttpActionResult Get(int id)
        {
            var allPrescriptions = DB.tblPrescriptions.Where(x => x.Patients_id == id).OrderByDescending(x=>x.date_time).Select(x => new {x.id, x.date_time, x.subject, x.value }).ToList();
            return Content(HttpStatusCode.OK, allPrescriptions);
        }

        // POST: api/Prescription
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Prescription/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Prescription/5
        public void Delete(int id)
        {
        }
    }
}