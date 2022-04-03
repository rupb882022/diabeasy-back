using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class tblPatientDataDto
    {

        public DateTime date_time;
        public int? blood_sugar_level;
        public string injectionType;
        public double? value_of_ingection;
        public string injection_site;
        public int? Patients_id ;
        public double? totalCarbs;
    }
}