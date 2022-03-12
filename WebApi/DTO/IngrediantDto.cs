using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class IngrediantDto
    {
        public int id;
        public string name;
        public string image;
        public List<string> category=new List<string>();
        public List<tblUnitOfMeasureDto> UnitOfMeasure=new List<tblUnitOfMeasureDto>();
    }
}