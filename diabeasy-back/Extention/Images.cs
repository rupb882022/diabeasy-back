using System;
using System.Linq;
using System.IO;
using NLog;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace diabeasy_back
{
    public class Images
    {
        diabeasyDBContext DB = new diabeasyDBContext();
        static Logger logger = LogManager.GetCurrentClassLogger();
        public string CreateNewNameOrMakeItUniqe(string name)
        {

            try
            {
                switch (name)
                {
                    case "profileDoctor":
                        tblDoctor doctor = DB.tblDoctor.OrderByDescending(x => x.id).FirstOrDefault();
                        int newDoctorId = doctor.id + 2;
                        return name + newDoctorId.ToString();

                    case "profilePatient":
                        tblPatients patient = DB.tblPatients.OrderByDescending(x => x.id).FirstOrDefault();
                        int newPatientId = patient.id + 2;
                        return name + newPatientId.ToString();
                    case "Ingredient":
                        Ingredients Ingredient = DB.Ingredients.OrderByDescending(x => x.id).FirstOrDefault();
                        int newIngredientId = Ingredient.id + 2;
                        return name + newIngredientId.ToString();
                    case "recipe":
                        Recipes Recipe = DB.Recipes.OrderByDescending(x => x.id).FirstOrDefault();
                        int newRecipeId = Recipe.id + 2;
                        return name + newRecipeId.ToString();
                    default:
                        saveImageNameInDB(name);
                        return name;
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                return null;
            }


        }
        public string ImageFileExist(string Name, string rootPath)
        {

            string[] names = Directory.GetFiles(rootPath);

            foreach (var fileName in names)
            {
                if (Path.GetFileNameWithoutExtension(fileName).IndexOf(Path.GetFileNameWithoutExtension(Name)) != -1)
                {

                    return fileName;
                }
            }

            return null;
        }
        void saveImageNameInDB(string name)
        {
            try
            {

                int id = int.Parse(string.Concat(name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse()));
                string pattern = @"\d+$";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                string result = rgx.Replace(name, replacement);

                switch (result)
                {
                    case "profileDoctor":
                       
                        tblDoctor D = DB.tblDoctor.Where(x => x.id == id).SingleOrDefault();
                        if (D != null)
                        {
                            D.profileimage = name + ".jpg";
                        }
                        break;

                    case "profilePatient":
                        tblPatients p = DB.tblPatients.Where(x => x.id == id).SingleOrDefault();
                        if (p != null)
                        {
                            p.profileimage = name + ".jpg";
                        }
                        break;
                    case "Ingredient":
                        Ingredients i = DB.Ingredients.Where(x => x.id == id).SingleOrDefault();
                        if (i != null)
                        {
                            i.image = name + ".jpg";
                        }
                        break;
                    case "recipe":
                        Recipes r = DB.Recipes.Where(x => x.id == id).SingleOrDefault();
                        if (r != null)
                        {
                            r.image = name + ".jpg";
                        }
                        break;
                    default:
                        break;
                }
                DB.SaveChanges();
             
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
            }
        }
    }
}
