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

        public int UserIndex;
        public int RecipieIndex;
        

        public List<string> CompatableRecipes;
        public List<string> Breakfast;
        public List<string> Lunch;
        public List<string> Dinner;
        public List<string> RecipesWithFridgeFoods;

        IDictionary<string, RecipeDef> Recipies;
        Dictionary<string, FoodDef> FridgeItems;

        public List<float> FoodAmounts;
        public List<string> Ingredients;

        List<RecipeDef> RecipiesToShow;

        ProgramManager CurrentManager;

        public RecipeManager(string DatabaseToUse, ProgramManager Manager)
        {
            RecipeDatabaseFile = DatabaseToUse;
            CurrentManager = Manager;
            UserIndex = Manager.User.CompatabilityIndex;
            Recipies = GetRecipies();
            FridgeItems = Manager.UsersFridge.UsersFoods;
            SortRecipes();
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


        /// <summary>
        /// Sorts recipies using Binary Comparison. Sorts recipes based on: Appropriate based on user settings, 
        /// if they are good for breakfast, lunch, dinner, and if the user has all the ingredients for the recipes.
        /// </summary>
        public void SortRecipes()
        {
            foreach (KeyValuePair<string, RecipeDef> item in Recipies)
            {
                foreach (string tag in item.Value.UserTags)
                {
                    RecipieIndex += 2 ^ CurrentManager.CompatabilityMap.IndexOf(tag);
                }

                if ((UserIndex & RecipieIndex) == UserIndex)
                {
                    CompatableRecipes.Add(item.Key);

                    if (item.Value.TimeTags.Contains("breakfast"))
                    {
                        Breakfast.Add(item.Key);
                    }

                    if (item.Value.TimeTags.Contains("lunch"))
                    {
                        Breakfast.Add(item.Key);
                    }

                    if (item.Value.TimeTags.Contains("dinner"))
                    {
                        Breakfast.Add(item.Key);
                    }

                    foreach (string food in item.Value.Ingredients)
                    {
                        if (FridgeItems.ContainsKey(food))
                        {
                            HasAllIngredients = true;
                        }
                        else
                        {
                            HasAllIngredients = false;
                            break;
                        }
                    }

                    if (HasAllIngredients == true)
                    {
                        RecipesWithFridgeFoods.Add(item.Key);
                    }
                }
            }
        }
    }
}
