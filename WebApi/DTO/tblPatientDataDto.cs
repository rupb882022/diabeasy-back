using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class tblPatientDataDto
    {

        public DateTime date_time;
        public Nullable<int> blood_sugar_level;
        public string injectionType;
        public Nullable<double> value_of_ingection;
        public string injection_site;
        public Nullable<int> Patients_id ;
        public Nullable<double> totalCarbs;
        public List<AteDto> food=new List<AteDto>();
        public List<int> ExceptionalEvent;
        public bool reccomandtion=false;
        public double system_recommendations;
    }
}