using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class tblRecipesDto
    {
        public int id;
        public string name;
        public string image;
        public double totalCarbohydrates;
        public double totalsugars;
        public double totalWeigthInGrams;
        public string cookingMethod;
        public string addByUserId;
        public bool favorit = false;
        public List<tblCategoryDto> category = new List<tblCategoryDto>();
        public List <IngredientsForRecipeDto> Ingrediants= new List<IngredientsForRecipeDto>();
        public List<tblUnitOfMeasureDto> UnitOfMeasure = new List<tblUnitOfMeasureDto>();
    }
}