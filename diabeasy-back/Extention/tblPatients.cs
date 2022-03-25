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
        [Display(Name = "first name patient")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "first name cannot be empty")]
        public string firstname;

        [Display(Name = "last name patient")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "last name cannot be empty")]
        public string lastname;

        [Display(Name = "height")]
        [Range(100,250,ErrorMessage ="please enter a your height in CM between 100-250")]
        public Nullable<double> height;

        [Display(Name = "weight")]
        [Range(10, 250, ErrorMessage = "please enter a your weight in KG")]
        public Nullable<double> weight;

        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "must be more then 6 charts")]
       // [StringLength(20)]
        [MinLength(6,ErrorMessage = "must be more then 6 charts")]
        public string password;

        [Display(Name = "Email address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "must fill email adress")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email;

        [Required(AllowEmptyStrings = true)]
        [Phone(ErrorMessage = "not a valid phone number")]
        public Nullable<int> assistant_phone;

        [Display(Name = "birthdate patient")]
        public Nullable<System.DateTime> birthdate;


    }

}
