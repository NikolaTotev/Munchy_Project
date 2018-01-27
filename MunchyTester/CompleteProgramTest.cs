using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikola.Munchy.MunchyAPI;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
namespace Nikola.Munchy.MunchyTester
{
    [TestClass]
    public class CompleteProgramTest
    {
        [TestMethod]
        public void ComplteTest()
        {
            string UserFile = @"d:\Desktop\USER5.json";
            string DefaultUserFile = @"d:\Desktop\DEFAULT_USER.json";

            string UserFridgeFile = @"d:\Desktop\USER_F.json";
            string DefaultFridgeFile = @"d:\Desktop\DEFAULT_FRIDGE.json";

            string FoodDefFile = @"d:\Desktop\FoodData.json";
            string RecipeDatabase = @"d:\Desktop\Recipes.json";

            string RecipeSaveFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\RecipeSavesFile";
            string DefaultSaver = @"  C:\Users\Nikola\AppData\Roaming\Munchy\DefaultSaverFile";


            List<string> TestPreferences = new List<string> { "IsVegetarian" };
            List<string> FoodsToAdd = new List<string> { "butter", "chicken",  "carrot", "onion", "pepper", "salt", "tomato", "rice noodle", "egg", "cheese",  "oil", "lentil", "squash" };
            
            ProgramManager TestManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, DefaultSaver);

            Assert.IsTrue(TestManager.User.UserName == null);
            Assert.IsTrue(TestManager.User.UserFridge.UsersFoods.Count == 1);
            Assert.IsTrue(TestManager.User.UserFridge.UsersFoods.ContainsKey("milk"));
            Assert.IsTrue(TestManager.User.CompatabilityIndex == 0);
            Assert.IsTrue(TestManager.RecipieManag.Breakfast.Count == 2);
            Assert.IsTrue(TestManager.RecipieManag.Lunch.Count == 7);
            Assert.IsTrue(TestManager.RecipieManag.Dinner.Count == 7);
            Assert.IsTrue(TestManager.RecipieManag.CompatableRecipes.Count == 0);
            Assert.IsTrue(TestManager.RecipieManag.Recipies.Count == 7);
            Assert.IsTrue(TestManager.RecipieManag.FridgeItems.Count == TestManager.User.UserFridge.UsersFoods.Count);
            Assert.IsTrue(TestManager.RecipieManag.RecipeDatabaseFile == RecipeDatabase);
            Assert.IsTrue(TestManager.RecipieManag.UserIndex == TestManager.User.CompatabilityIndex);
            Assert.IsTrue(TestManager.FoodManag.Foods.Count != 0);

          

            
            foreach (string tag in TestPreferences)
            {
                TestManager.User.Preferences.Add(tag);
            }

           

            foreach (KeyValuePair<string, FoodDef> element in TestManager.FoodManag.Foods)
            {
                foreach (string food in FoodsToAdd)
                {
                    if (element.Key == food)
                    {
                        TestManager.User.UserFridge.AddToFridge(element.Value);
                    }                    
                }
                
            }

            TestManager.SaveUser();

          
            TestManager.User.CalculateIndex();
            TestManager.SaveUser();
            Assert.IsTrue(File.Exists(UserFile));
            Assert.IsFalse(TestManager.User.CompatabilityIndex == 0);

            Assert.IsTrue(TestManager.User.UserFridge.UsersFoods.Count == FoodsToAdd.Count + 1);

            TestManager.RecipieManag.SortRecipes();

            Assert.IsTrue(TestManager.RecipieManag.CompatableRecipes.Count != 0);
            Assert.IsTrue(TestManager.RecipieManag.Breakfast.Count != 0);
            Assert.IsTrue(TestManager.RecipieManag.Lunch.Count != 0);
            Assert.IsTrue(TestManager.RecipieManag.Dinner.Count != 0);      
        }

        [TestMethod]
        public void ComplateTestForWhenUserAlreayExists()
        {
            string UserFile = @"d:\Desktop\USER5.json";
            string DefaultUserFile = @"d:\Desktop\DEFAULT_USER.json";

            string UserFridgeFile = @"d:\Desktop\USER_F.json";
            string DefaultFridgeFile = @"d:\Desktop\DEFAULT_FRIDGE.json";

            string FoodDefFile = @"d:\Desktop\FoodData.json";
            string RecipeDatabase = @"d:\Desktop\Recipes.json";

            string RecipeSaveFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\RecipeSavesFile";
            string DefaultSaver = @"  C:\Users\Nikola\AppData\Roaming\Munchy\DefaultSaverFile";


            List<string> TestPreferences = new List<string> { "IsVegetarian" };
            List<string> FoodsToAdd = new List<string> { "butter", "chicken", "carrot", "onion", "pepper", "salt", "tomato", "rice noodle", "egg", "cheese", "oil", "lentil", "squash" };

            ProgramManager TestManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, DefaultSaver);

            Assert.IsTrue(TestManager.User.UserName == "Nikola");
            Assert.IsTrue(TestManager.User.UserFridge.UsersFoods.Count == FoodsToAdd.Count + 1);
            Assert.IsTrue(TestManager.User.UserFridge.UsersFoods.ContainsKey("egg"));
            Assert.IsTrue(TestManager.User.CompatabilityIndex == 2);
            Assert.IsTrue(TestManager.RecipieManag.Breakfast.Count == 2);
            Assert.IsTrue(TestManager.RecipieManag.Lunch.Count == 4);
            Assert.IsTrue(TestManager.RecipieManag.Dinner.Count == 4);
            Assert.IsTrue(TestManager.RecipieManag.CompatableRecipes.Count == 4);
            Assert.IsTrue(TestManager.RecipieManag.Recipies.Count == 7);
            Assert.IsTrue(TestManager.RecipieManag.FridgeItems.Count == TestManager.User.UserFridge.UsersFoods.Count);
            Assert.IsTrue(TestManager.RecipieManag.RecipeDatabaseFile == RecipeDatabase);
            Assert.IsTrue(TestManager.RecipieManag.UserIndex == TestManager.User.CompatabilityIndex);
            Assert.IsTrue(TestManager.FoodManag.Foods.Count != 0);
        }


        [TestMethod]
        public void RecipeSaverTest()
        {
            string UserFile = @"d:\Desktop\USER5.json";
            string DefaultUserFile = @"d:\Desktop\DEFAULT_USER.json";

            string UserFridgeFile = @"d:\Desktop\USER_F.json";
            string DefaultFridgeFile = @"d:\Desktop\DEFAULT_FRIDGE.json";

            string FoodDefFile = @"d:\Desktop\FoodData.json";
            string RecipeDatabase = @"d:\Desktop\Recipes.json";

            string RecipeSaveFile = @"C:\Users\Nikola\AppData\Roaming\Munchy\RecipeSavesFile.json";
            string DefaultSaver = @"C:\Users\Nikola\AppData\Roaming\Munchy\DefaultSaverFile.json";


            List<string> TestPreferences = new List<string> { "IsVegetarian" };
            List<string> FoodsToAdd = new List<string> { "butter", "chicken", "carrot", "onion", "pepper", "salt", "tomato", "rice noodle", "egg", "cheese", "oil", "lentil", "squash" };

            ProgramManager TestManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, DefaultSaver);

           
            Assert.IsTrue(TestManager.UserRecipeSaves.SavedRecipes.Count ==0);
            Assert.IsTrue(TestManager.UserRecipeSaves.CookedToday.Count == 0);
            Assert.IsTrue(TestManager.UserRecipeSaves.CookedRecipes.Count ==0);
            Assert.IsTrue(TestManager.UserRecipeSaves.RecentlyViewed.Count == 0);


        }


    }
}
