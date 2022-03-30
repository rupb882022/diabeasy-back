using System;
using System.Linq;
using System.IO;
using NLog;
using System.Web;

namespace diabeasy_back
{
    public class Images
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public string CreateNewNameOrMakeItUniqe(string name)
        {
            string start = name;//Path.GetFileNameWithoutExtension(name);
                                // string end = Path.GetExtension(name);
            try
            {
                switch (name)
                {
                    case "profileDoctor":
                        tblDoctor doctor = DB.tblDoctor.OrderByDescending(x => x.id).FirstOrDefault();
                        int newDoctorId = doctor.id + 2;
                        return start + newDoctorId.ToString();

                    case "profilePatient":
                        tblPatients patient = DB.tblPatients.OrderByDescending(x => x.id).FirstOrDefault();
                        int newPatientId = patient.id + 2;
                        return start + newPatientId.ToString();
                    case "Ingredient":
                        Ingredients Ingredient = DB.Ingredients.OrderByDescending(x => x.id).FirstOrDefault();
                        int newIngredientId = Ingredient.id + 2;
                        return start + newIngredientId.ToString();
                    case "recipe":
                        Recipes Recipe = DB.Recipes.OrderByDescending(x => x.id).FirstOrDefault();
                        int newRecipeId = Recipe.id + 2;
                        return start + newRecipeId.ToString();

                    default:
                        return start;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }


        }
        public bool ImageFileExist(string Name)
        {
            string rootPath = HttpContext.Current.Server.MapPath("~/uploadFiles");
            string[] names = Directory.GetFiles(rootPath);
            logger.Fatal("1");

            foreach (var fileName in names)
            {
                if (Path.GetFileNameWithoutExtension(fileName).IndexOf(Path.GetFileNameWithoutExtension(Name)) != -1)
                {
                    logger.Fatal("2");

                    return true;
                }
            }
            logger.Fatal("3");

            return false;

        }
    }
}
