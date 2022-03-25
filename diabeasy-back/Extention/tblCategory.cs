using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{
    [MetadataType(typeof(tblCategoryMetaData))]

    public partial class tblCategory
    {
    }
    public class tblCategoryMetaData
    {
        [Display(Name = "category name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "can not be an empty category name name")]
        public string name;

    }
}
