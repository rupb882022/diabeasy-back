using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diabeasy_back
{
    [MetadataType(typeof(tblForumMetaData))]
    public partial class tblForum
    {
  
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
