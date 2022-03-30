using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diabeasy_back
{
    [MetadataType(typeof(RecipesMetaData))]
    public partial class Recipes
    {
       
    }

    public class RecipesMetaData
    {
        [Display(Name = "recipe name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "can not be an empty recipe name")]
        public string name;

        [Display(Name = "total Carbohydrates")]
        [Required]
        public Nullable<double> totalCarbohydrates;

        [Display(Name = "total sugars")]
        [Required]
        public Nullable<double> totalsugars;

        [Display(Name = "total Weigth In Grams")]
        [Required]
        public Nullable<double> totalWeigthInGrams;

        [Display(Name = "added by user id")]
        [Required]
        public Nullable<int> addByUserId;

    }
}
