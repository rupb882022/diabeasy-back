using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace diabeasy_back
{
    public class Food
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public Nullable<double> calc100Grams(int unid_id,int weightInGrams, double value)
        {
            try
            {
                tblUnitOfMeasure U = DB.tblUnitOfMeasure.Where(x => x.id == unid_id).SingleOrDefault();
                if (U.name == "grams")
                    return null;
                double calcValue = (value / weightInGrams) * 100;
                return calcValue;
                              }
            catch (Exception ex)
            {
                logger.Fatal("do not found unit id in DB" + ex.Message);
                return null;
            }
        }
        public int getUnitID(string name)
        {
            try
            {
            tblUnitOfMeasure U = DB.tblUnitOfMeasure.Where(x => x.name == name).SingleOrDefault();
                if (U == null)
                    throw new NullReferenceException("invalid unit id");

                return U.id;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return int.MinValue;
            }
        }
    }
}
