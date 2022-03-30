using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class tblFavoritesDto
    {
        public Nullable<int> Rcipe_id;
        public Nullable<int> Ingredient_id;
        public int user_id;

    }
}