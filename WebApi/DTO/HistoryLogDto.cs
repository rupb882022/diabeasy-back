using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.DTO
{
    public class HistoryLogDto
    {
        public int id;
        public DateTime date_time=new DateTime();
        public string Historylog_key;
        public string Historylog_value;
         public int Patients_id;
    }
}