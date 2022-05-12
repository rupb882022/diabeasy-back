using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diabeasy_back
{
    [MetadataType(typeof(tblForumMetaData))]
    public partial class tblForum
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["diabeasyDB"].ConnectionString);
       public static int writtenBy(tblForum obj)
        {
            try
            {
                if (obj.Patients_id != null)
                {
                    return  int.Parse(obj.Patients_id.ToString());
                }
                else
                {
                    return  int.Parse(obj.Doctor_id.ToString());
                }
            }
            catch (Exception ex)
            {

                logger.Fatal(ex.Message);
                return 0;
            }
        }
    }
    public class tblForumMetaData
    {

        [MinLength(3, ErrorMessage = "must be 3 charts or more")]
        [Display(Name = "forum value")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "value cannot be empty")]
        public string value;

        [MinLength(3, ErrorMessage = "must be 3 charts or more")]
        [Display(Name = "forum subject")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "subject cannot be empty")]
        public string subject;


    }
}
