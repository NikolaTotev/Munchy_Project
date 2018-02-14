﻿using System;
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
        public int RecipieIndex = 0;

        // Recipes that are compatable with the user's preferences.
        public List<string> CompatableRecipes = new List<string>();
        // All compatable recipies that have the time tag "breakfast".
        public List<string> Breakfast = new List<string>();
        // All compatable recipies that have the time tag "lunch".
        public List<string> Lunch = new List<string>();
        // All compatable recipies that have the time tag "dinner".
        public List<string> Dinner = new List<string>();
        // All compatable recipes that the user has the ingredients for.
        public List<string> RecipesWithFridgeFoods = new List<string>();

        public Dictionary<string, RecipeDef> Recipies;
        public Dictionary<string, FoodDef> FridgeItems;

        List<RecipeDef> RecipiesToShow;

        ProgramManager CurrentManager;

        public RecipeManager(string DatabaseToUse, ProgramManager Manager)
        {
            RecipeDatabaseFile = DatabaseToUse;
            CurrentManager = Manager;
            UserIndex = Manager.User.CompatabilityIndex;
            Recipies = GetRecipies();
            FridgeItems = Manager.UsersFridge.USUsersFoods;
            SortRecipes();
        }

        /// <summary>
        /// Returns a dictionary based on a specified JSON file. 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, RecipeDef> GetRecipies()
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
        /// Sorts recipies using Bit Comparison. Sorts recipes based on: Appropriate based on user settings, 
        /// if they are good for breakfast, lunch, dinner, and if the user has all the ingredients for the recipes.
        /// </summary>
        public void SortRecipes()
        {
            UserIndex = CurrentManager.User.CompatabilityIndex;
            foreach (KeyValuePair<string, RecipeDef> item in Recipies)
            {
                foreach (string tag in item.Value.UserTags)
                {
                    RecipieIndex += (int)Math.Pow(2, (CurrentManager.CompatabilityMap.IndexOf(tag)));
                }

                if (UserIndex != 0)
                {
                    if ((UserIndex & RecipieIndex) == UserIndex)
                    {
                        CompatableRecipes.Add(item.Key);

                        if (item.Value.TimeTags.Contains("breakfast"))
                        {
                            Breakfast.Add(item.Key);
                        }

                        if (item.Value.TimeTags.Contains("lunch"))
                        {
                            Lunch.Add(item.Key);
                        }

                        if (item.Value.TimeTags.Contains("dinner"))
                        {
                            Dinner.Add(item.Key);
                        }

                        foreach (string food in item.Value.USIngredients)
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
                else
                {
                    if (item.Value.TimeTags.Contains("breakfast"))
                    {
                        Breakfast.Add(item.Key);
                    }

                    if (item.Value.TimeTags.Contains("lunch"))
                    {
                        Lunch.Add(item.Key);
                    }

                    if (item.Value.TimeTags.Contains("dinner"))
                    {
                        Dinner.Add(item.Key);
                    }

                    foreach (string food in item.Value.USIngredients)
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

                RecipieIndex = 0; 

            }
        }
    }
}
