using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class IngredientsForRecipeDto
    {
        public int id;
        public string name;
        public string image;
        public double amount;
        public int unitId;
        public string unitName;
    }
}