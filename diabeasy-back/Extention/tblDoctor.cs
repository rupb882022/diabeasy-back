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
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string email;

        [Display(Name = "first name doctor")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "first name cannot be empty")]      
        [MinLength(2, ErrorMessage = "must be 2 charts or more")]
        public string firstname;

        [Display(Name = "last name doctor")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "last name cannot be empty")]
        [MinLength(2, ErrorMessage = "must be 2 charts or more")]
        public string lastname;


        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "must be 8 charts or more")]
        [MinLength(8, ErrorMessage = "must be 8 charts or more")]
        public string password;
        

    }
}
