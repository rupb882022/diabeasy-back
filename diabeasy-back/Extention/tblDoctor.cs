using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{
    [MetadataType(typeof(tblDoctorMeteData))]

    public partial class tblDoctor
    {

    }
    public class tblDoctorMeteData
    {
        [Display(Name = "Email address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "must fill email adress")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email;

        [Display(Name = "first name doctor")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "first name cannot be empty")]    
        public string firstname;

        [Display(Name = "last name doctor")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "last name cannot be empty")]
        public string lastname;

        [Display(Name = "birthdate doctor")]
        public Nullable<System.DateTime> birthdate;

        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "must be more then 6 charts")]
        [MinLength(6, ErrorMessage = "must be more then 6 charts")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string password;

        [Display(Name = "gender")]
        public string gender;
        

    }
}
