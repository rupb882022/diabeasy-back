using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace diabeasy_back
{
    public class Images
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public string CreateNewNameOrMakeItUniqe(string name)
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
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }


        }
    }
}
