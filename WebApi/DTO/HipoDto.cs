using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class HipoDto
    {
        public DateTime date_time;
        public int blood_sugar_level;
        public int blood_sugar_level_2H;
        public double totalCarbs;
        public int food_id;
        public int UnitOfMeasure_id;
        public string UnitName;
        public string FoodName;
        public double amount;
        public string image;
        public int count;
    }
}