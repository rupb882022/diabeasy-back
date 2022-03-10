using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class UserDto
    {
        public int id;
        public string firstName;
        public string lastName;
        public string email;
        public string gender;
        public DateTime BirthDate;
        public string password;
        public Nullable<int> height;
        public Nullable<int> weight;
        public int InsulinType_id;
        public int InsulinType_long_id;
        public string mailDoctor;
        //public string spot;
        public Nullable <int> phoneNumber;
        public string image;
    }
}
