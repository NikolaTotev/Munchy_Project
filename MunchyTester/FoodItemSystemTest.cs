using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Nikola.Munchy.MunchyAPI;

namespace Nikola.Munchy.MunchyTester
{
    [TestClass]
    public class FoodItemSystemTest
    {
        [TestMethod]
        public void FoodManagerTest()
        {
            // Creates an example list of 2 Food definitions. All the properties are filled in.
            List<FoodDef> Data = new List<FoodDef>
            {
                new FoodDef { Name = "milk", Calories = 350, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Liters", ImageFileName = "/resources/images/milk.png" },
                new FoodDef { Name = "butter", Calories = 500, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" }
            };

            // Creates an example dictionary that will be filled with the elements from the list.
            Dictionary<string, FoodDef> foods = new Dictionary<string, FoodDef>();

            // A for each loop is used to ensure correct tagging of each object. Tagging is done via the FoodDef "Name" property.
            foreach (var item in Data)
            {
                foods.Add(item.Name, item);
            }

            // Test location of JSON data file.
            string TestFile = @"/Resources/JSON_test.json";

            // Using the stream writer a file is created at the "TestFile" location. A JsonSerializer is created and it serializes the "foods" 
            // dictionary to the file that was created.
            using (StreamWriter file = File.CreateText(TestFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, foods);
            }

            // Checks if the file that was just created has actually been created. If not it sends a test failed message.
            Assert.IsTrue(File.Exists(TestFile), "Test file {0} should exsit", TestFile);

            // Creates a test food manager and passes to the constructor the "TestFile" location where the test JSON file is located.
            FoodManager TestManager = new FoodManager(TestFile);

            // A new IDictionary is created and it is assined the value of the Dictionary that the TestManager function "GetFoodDefinitions" returns.
            IDictionary<string, FoodDef> testDictionary = TestManager.GetFoodDefinitions();

            // Tests if the "testDictionary" is null or not. For the test to pass it must not be null.
            Assert.IsNotNull(testDictionary, "Food def's should be retrieved.");

            // Tests for equal counts between created dictonary and the dictionary that has been returned.
            Assert.AreEqual(foods.Count, testDictionary.Count, "Items in both dictonaries should be equal");

            // Checks wether or not the returned dictionary contains given keys.
            Assert.IsTrue(testDictionary.ContainsKey(Data[0].Name), "{0} should exist in the returned dictonary of definitions", Data[0].Name);
            Assert.IsTrue(testDictionary.ContainsKey(Data[1].Name), "{0} should exist in the returned dictonary of definitions", Data[1].Name);


        }

        [TestMethod]
        public void RecipeManagerTest()
        {
            List<RecipeDef> Recipies = new List<RecipeDef>
            {
                new RecipeDef {Name = "Chicken Dinner", Description = "A dinner made of chicken", Ingredients =  new List<string> {"chicken, milk, butter" }, ImageFile  = "/resources/images/r_ChickenDiner" },
                new RecipeDef {Name = "Cake", Description = "Velvet Cake", Ingredients = new List<string> {"chocolate, milk, butter" }, ImageFile  = "/resources/images/r_VelvetCake" }
            };

            Dictionary<string, RecipeDef> RecipieDic = new Dictionary<string, RecipeDef>();

            foreach (var item in Recipies)
            {
                RecipieDic.Add(item.Name, item);
            }

            string testPath = @"d:\Desktop\Recipies.json";

            using (StreamWriter file = File.CreateText(testPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, RecipieDic);
            }

            Assert.IsTrue(File.Exists(testPath), "Test file at {0} should exist and contain 2 test  Recipies", testPath);

            RecipeManager testRecManager = new RecipeManager(testPath);

            IDictionary<string, RecipeDef> testDiction = testRecManager.GetRecipies();

            Assert.IsNotNull(testDiction, "Recipies should be retrieved");
            Assert.AreEqual(RecipieDic.Count, testDiction.Count, "Number of elements in testDiction should be equal to the RecipieDic");
            Assert.IsTrue(testDiction.ContainsKey(Recipies[0].Name), "{0} should exist in the returned dictionary", Recipies[0].Name);
            Assert.IsTrue(testDiction.ContainsKey(Recipies[1].Name), "{0} should exist in the returned dictionary", Recipies[1].Name);
        }

        [TestMethod]
        public void FridgeTest()
        {

            // Creates an example list of 2 Food definitions. All the properties are filled in.
            List<FoodDef> Data = new List<FoodDef>
            {
                new FoodDef { Name = "milk", Calories = 350, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Liters", ImageFileName = "/resources/images/milk.png" },
                new FoodDef { Name = "butter", Calories = 500, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" }
            };
                       
            // Creates an example dictionary that will be filled with the elements from the list.
            Dictionary<string, FoodDef> foods = new Dictionary<string, FoodDef>();

            // A for each loop is used to ensure correct tagging of each object. Tagging is done via the FoodDef "Name" property.
            foreach (var item in Data)
            {
                foods.Add(item.Name, item);
            }

            List<FoodDef> DefaultData = new List<FoodDef>
            {
                new FoodDef { Name = "milk", Calories = 350, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Liters", ImageFileName = "/resources/images/milk.png" },
                new FoodDef { Name = "butter", Calories = 211, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" },
                new FoodDef { Name = "cheese", Calories = 330, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" },
                new FoodDef { Name = "bread", Calories = 12, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" }
            };
            Dictionary<string, FoodDef> DefaultFoods = new Dictionary<string, FoodDef>();

            foreach (var item in DefaultData)
            {
                DefaultFoods.Add(item.Name, item);
            }

            // Test location of JSON data file.
            string TestFile = @"d:\Desktop\JSON_FridgeTest.json";
            string DefaultFridge = @"d:\Desktop\DEFAULT_FRIDGE.json";

            // Using the stream writer a file is created at the "TestFile" location. A JsonSerializer is created and it serializes the "foods" 
            // dictionary to the file that was created.
            using (StreamWriter file = File.CreateText(TestFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, foods);
            }

            using (StreamWriter file = File.CreateText(DefaultFridge))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, DefaultFoods);
            }

            // Checks if the file that was just created has actually been created. If not it sends a test failed message.
            Assert.IsTrue(File.Exists(TestFile), "Test file {0} should exsit", TestFile);

            // Creates a test fridge that will be used to test fridge functionality
            FridgeTemplate testFridge = new FridgeTemplate(TestFile, DefaultFridge);

            // On creation the fridge dictionary should be filled, this checks if the fridge dictionary has indeed been filled.
            Assert.IsTrue(testFridge.UsersFoods != null, "UsersFoods must not be empty");

            // Checks if the dictionary has been correctly filled by checking if the dictionary contains the key "Data[0].Name".
            Assert.IsTrue(testFridge.UsersFoods.ContainsKey(Data[0].Name), "The first Item in the fridge must be the same as the first item in the test list");

            // Creates a new list that will be used to add a new FoodDef to the fridge. This is used to test the Add, Remove and Serach methods.
            List<FoodDef> NewFood = new List<FoodDef>
            {
                new FoodDef  { Name = "chocolate", Calories = 311, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Grams", ImageFileName = "/resources/images/chocolate.png" }
            };

            // Testing the "AddToFridge" function.
            testFridge.AddToFridge(NewFood[0]);

            // Testing wether or not the item has been added and if it has is it the correct item.
            Assert.IsTrue(testFridge.UsersFoods.Count == 3, "UsersFoods count should be with one more than the previous count");
            Assert.IsTrue(testFridge.UsersFoods.ContainsKey(NewFood[0].Name), "UsersFoods should contain the new item");

            // Testing the "RemoveFromFridge" function.
            testFridge.RemoveFromFridge(NewFood[0]);

            // Testing if the item has been removed
            Assert.IsTrue(testFridge.UsersFoods.Count == 2, "UsersFoods count should be with one more than the previous count");
            // Confirms that the correct item has been removed.
            Assert.IsTrue(!testFridge.UsersFoods.ContainsKey(NewFood[0].Name), "The new item should have been removed from UsersFoods");

            // This list is created to check the "search" function of the Fridge class. 
            List<FoodDef> NewFoods = new List<FoodDef>
            {
                new FoodDef  { Name = "chocolate", Calories = 311, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Grams",Amount = 200, ImageFileName = "/resources/images/chocolate.png" },
                new FoodDef  { Name = "orange", Calories = 311, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Number",Amount = 5, ImageFileName = "/resources/images/orange.png" },
                new FoodDef  { Name = "rice", Calories = 311, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Grams",Amount = 500, ImageFileName = "/resources/images/rice.png" }
            };

            // Adding the new list of items to the fridge.
            foreach (var FoodItem in NewFoods)
            {
                testFridge.AddToFridge(FoodItem);
            }

            // Creating the arrays that are needed for the search function.
            string[] TagsToSearchFor = new string[] { NewFoods[0].Name, NewFoods[1].Name, "peach" };
            float[] AmountsToCheck = new float[] { NewFoods[0].Amount, NewFoods[1].Amount, 200 };

            // A boolean is created to accept the "FridgeContains" result. "FridgeContains" is a boolean.
            bool HasIngredients = testFridge.FridgeConatains(TagsToSearchFor, AmountsToCheck);

            // In this case the amounts to search for are set to a value that will retun false. This checks if it will indeed be false.
            Assert.IsTrue(HasIngredients == false, "The fridge should not have all ingredients because there is a test value that is made incorrect");

            // Specifies which item's amount has to be modified and by how much
            testFridge.ModifyFoodItemAmount("rice", 100);

            testFridge.UsersFoods.TryGetValue("rice", out FoodDef Product);
            Assert.IsTrue(Product.Amount == 100, "The amount of rice should be 100");

            // Sets the modified amount to 0 which means the item has to be removed.
            testFridge.ModifyFoodItemAmount("rice", 0);
            // Tests if the item has indeed been removed.
            Assert.IsTrue(!testFridge.UsersFoods.ContainsKey("rice"), "Rice should be remove, as the modified amount was set to 0");
        }

        [TestMethod]
        public void DefaultFridgeTest()
        {

            List<FoodDef> DefaultData = new List<FoodDef>
            {
                new FoodDef { Name = "milk", Calories = 350, Carbs = 20, Protein = 50, Sugars = 100, Fat = 20, Sodium = 0, UOM = "Liters", ImageFileName = "/resources/images/milk.png" },
                new FoodDef { Name = "butter", Calories = 211, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" },
                new FoodDef { Name = "cheese", Calories = 330, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" },
                new FoodDef { Name = "bread", Calories = 12, Carbs = 10, Protein = 20, Sugars = 0, Fat = 250, Sodium = 12, UOM = "Grams", ImageFileName = "/resources/images/butter.png" }
            };
            Dictionary<string, FoodDef> DefaultFoods = new Dictionary<string, FoodDef>();

            foreach (var item in DefaultData)
            {
                DefaultFoods.Add(item.Name, item);
            }

            // Test location of JSON data file.
            string TestFile = @"d:\Desktop\JSON_FridgeTest.json";
            string DefaultFridge = @"d:\Desktop\DEFAULT_FRIDGE.json";

        

            using (StreamWriter file = File.CreateText(DefaultFridge))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, DefaultFoods);
            }

            // Checks if the file that was just created has actually been created. If not it sends a test failed message.
            Assert.IsTrue(File.Exists(DefaultFridge), "Test file {0} should exsit", DefaultFridge);

            FridgeTemplate testFridge = new FridgeTemplate(TestFile, DefaultFridge);

            //Assert.IsTrue(testFridge.UsersFoods.ContainsKey(DefaultData[3].Name), "DefaultData list should have been used");
            Assert.IsTrue(!testFridge.UsersFoods.ContainsKey("bread"), "DefaultData list should have been used");

        }
    }
}
