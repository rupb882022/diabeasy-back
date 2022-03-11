using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class UserDto
    {
        public Nullable<int> id;
        public string firstName;
        public string lastName;
        public string email;
        public string gender;
        public DateTime BirthDate;
        public string password;
        public Nullable<int> height;
        public Nullable<int> weight;
        public Nullable<int> InsulinType_id;
        public Nullable<int> InsulinType_long_id;
        public string mailDoctor;
        public Nullable <int> phoneNumber;

    }
}
