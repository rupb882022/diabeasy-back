using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class GrapsDto
    {
        public int month;
        public Nullable<int> averge=null;
        public string upTo240;
        public string upTo180;
        public string upTo75;
        public string upTo60;
        public string upTo0;

        public string H0;
        public string H8;
        public string H14;
        public string H20;
    }
}