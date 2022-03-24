using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{

    [MetadataType(typeof(tblPatientsMeteData))]
    public partial class tblPatients
    {

    }
    public class tblPatientsMeteData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "first name cannot be empty")]
        public string firstname;
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "last name cannot be empty")]
        public string lastname;

        [Range(100,250,ErrorMessage ="please enter a your height in CM between 100-250")]
        public Nullable<double> height;
      
        [Range(10, 250, ErrorMessage = "please enter a your weight in KG")]
        public Nullable<double> weight;
    }

}
