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
        public string addByUserId;
        public List<tblCategoryDto> category=new List<tblCategoryDto>();
        public List<tblUnitOfMeasureDto> UnitOfMeasure=new List<tblUnitOfMeasureDto>();
    }
}