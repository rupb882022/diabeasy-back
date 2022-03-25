using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{
    [MetadataType(typeof(IngredientsMetaData))]
    public partial class Ingredients
    {
    }

    public class IngredientsMetaData
    {
        [Display(Name = "ingredient name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "can not be an empty ingredient name")]
        public string name;

        [Display(Name = "added by user id")]
        [Required]
        public Nullable<int> addByUserId;
    }
}
