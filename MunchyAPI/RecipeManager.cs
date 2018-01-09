using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace Nikola.Munchy.MunchyAPI

{
    public class RecipeManager
    {
        public string RecipeDatabaseFile;
        bool HasAllIngredients;
        public Dictionary<string, bool> UserPreferences;
        public List<bool> RecipieTags;
        public List<float> FoodAmounts;
        public List<string> Ingredients;

        int CurrentPos = 0;

        IDictionary<string, RecipeDef> Recipies;

        List<RecipeDef> RecipiesToShow;
        Dictionary<string, RecipeDef> RecipiesToSort;

        ProgramManager CurrentManager;

        public RecipeManager(string DatabaseToUse, ProgramManager Manager)
        {
            RecipeDatabaseFile = DatabaseToUse;
            CurrentManager = Manager;
            UserPreferences = Manager.User.Preferences;
        }

        /// <summary>
        /// Returns a dictionary based on a specified JSON file. 
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, RecipeDef> GetRecipies()
        {
            // Checks if given file exists.
            if (!File.Exists(RecipeDatabaseFile))
            {
                // If file does not exist an exeption is thrown.
                throw new Exception(string.Format("Given database file {0} does not exist or cant be accessed", RecipeDatabaseFile));
            }

            // Using stream reader a the JSON file is opened.
            using (StreamReader file = File.OpenText(RecipeDatabaseFile))
            {
                // A new JsonSerializer is created that is used to deserialize the JSON file
                JsonSerializer serializer = new JsonSerializer();
                // A dictionary "Recipies" is created and its euqual to the serialized dicionary in the file.
                Dictionary<string, RecipeDef> Recipes = (Dictionary<string, RecipeDef>)serializer.Deserialize(file, typeof(Dictionary<string, RecipeDef>));
                return Recipes;
            }

        }

        public void LoadRecipies()
        {
            Recipies = GetRecipies();
            if (DateTime.Now.Hour < 11 && DateTime.Now.Hour >= 7)
            {
                foreach (KeyValuePair<string, RecipeDef> Recipie in Recipies)
                {
                    if (Recipie.Value.TimeTags.Contains("breakfast"))
                    {
                        RecipiesToSort.Add(Recipie.Key, Recipie.Value);
                    }
                }
            }

            if (DateTime.Now.Hour < 5 && DateTime.Now.Hour >= 11)
            {
                Recipies = GetRecipies();
                if (DateTime.Now.Hour < 11 && DateTime.Now.Hour >= 7)
                {
                    foreach (KeyValuePair<string, RecipeDef> Recipie in Recipies)
                    {
                        if (Recipie.Value.TimeTags.Contains("lunch"))
                        {
                            RecipiesToSort.Add(Recipie.Key, Recipie.Value);
                        }
                    }
                }
            }

            if (DateTime.Now.Hour < 22 && DateTime.Now.Hour <= 5)
            {
                Recipies = GetRecipies();
                if (DateTime.Now.Hour < 11 && DateTime.Now.Hour >= 7)
                {
                    foreach (KeyValuePair<string, RecipeDef> Recipie in Recipies)
                    {
                        if (Recipie.Value.TimeTags.Contains("dinner"))
                        {
                            RecipiesToSort.Add(Recipie.Key, Recipie.Value);
                        }
                    }
                }
            }
        }

        public void SortRecipies()
        {
            foreach (KeyValuePair<string, RecipeDef> element in Recipies)
            {
                if (element.Value.UserTags == UserPreferences)
                {
                    RecipiesToShow.Add(element.Value);
                }
            }
        }

        public RecipeDef GetRecipie()
        {
            RecipeDef CurrentRecipie;
            if (CurrentPos <= RecipiesToShow.Count || CurrentPos <= 15)
            {
                CurrentRecipie = RecipiesToShow[CurrentPos];
                CurrentPos++;
                return CurrentRecipie;
            }
            else
            {
                throw new Exception(string.Format("Out of recipies"));
            }
        }
        

    }
}
