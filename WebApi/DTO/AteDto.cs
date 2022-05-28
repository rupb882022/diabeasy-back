using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class AteDto
    {
        public string unitName;
        public Nullable<int> UnitOfMeasure_id;
        public Nullable<double> amount;
        public Nullable<int> foodId;
        public string FoodName;
        public string image;
        public Nullable<int> count;
    }
}