using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{
    [MetadataType(typeof(tblPrescriptionsMetaData))]

    public partial class tblPrescriptions
    {
    }
    public class tblPrescriptionsMetaData
    {
       

        [MinLength(3, ErrorMessage = "must be more then 3 charts")]
        [Display(Name = "Prescription subject")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "can not be an empty request subject")]
        public string subject;

        [MinLength(3, ErrorMessage = "must be more then 3 charts")]
        [Display(Name = "Prescription value")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "can not be an empty request value")]
        public string value;

        [Display(Name = "status")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "empty status")]
        public string status;

    }
}
