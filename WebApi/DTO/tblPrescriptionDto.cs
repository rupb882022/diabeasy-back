using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class tblPrescriptionDto
    {
        public int id;
        public Nullable<System.DateTime> date_time;
        public string subject;
        public string value;
        public Nullable<int> Patients_id;
        public Nullable<int> Doctor_id;
        public string status;

    }
}