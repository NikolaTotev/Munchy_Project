using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nikola.Munchy.MunchyAPI;
using System.Collections.Generic;

namespace Nikola.Munchy.MunchyTester
{
    [TestClass]
    public class MunchyAPISerializationTest
    {
        //Tests serialization and deserialization of the USER via program manager. 
        [TestMethod]
        public void UserSerializationTest()
        {
            //Getting user applicaitondata folder.
            string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string programFolder = System.IO.Path.Combine(LocalAppDataPath, "Munchy");

            //Data File Locations
            string userFile = System.IO.Path.Combine(programFolder, "USER.json");

            string userFridgeFile = System.IO.Path.Combine(programFolder, "USER_FRIDGE.json");

            string foodDefFile = System.IO.Path.Combine(programFolder, "FoodData.json");
            string recipeDatabase = System.IO.Path.Combine(programFolder, "Recipes.json");

            string recipeSaveFile = System.IO.Path.Combine(programFolder, "RecipeSavesFile.json");
            string statSavePath = System.IO.Path.Combine(programFolder, "StatSavePath.json");
            string m_ShoppingListFile = System.IO.Path.Combine(programFolder, "ShoppingList.json");


            ProgramManager currentManager = new ProgramManager(userFile, userFridgeFile, recipeDatabase, foodDefFile, recipeSaveFile, statSavePath, m_ShoppingListFile);
            List<string> preferences = new List<string>
            {
                "isvegan",
                "isvegetarian",
                "isdiabetic",
                "eggs",
                "dairy",
                "fish",
                "nuts",
                "gluten",
                "soy"
            };

            currentManager.User.UserName = "Nikola";
            currentManager.User.Age = 17;
            currentManager.User.Weight = 89;
            currentManager.User.Sex = "male";
            currentManager.User.LanguagePref = "EN";
            currentManager.User.Preferences = preferences;
            currentManager.SaveUser();

            UserTemplate newUser = currentManager.GetUser();

            Assert.IsTrue(newUser.UserName == "Nikola");
            Assert.IsTrue(newUser.Age == 17);
            Assert.IsTrue(newUser.Weight == 89);
            Assert.IsTrue(newUser.Sex == "male");
            Assert.IsTrue(newUser.LanguagePref == "EN");
            Assert.IsTrue(newUser.Preferences.Count == preferences.Count);
        }

        //Testing Fridge serialization.
        [TestMethod]
        public void FridgeSerializationTest()
        {
            //Getting user applicaitondata folder.
            string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string programFolder = System.IO.Path.Combine(LocalAppDataPath, "Munchy");

            //Data File Locations
            string userFile = System.IO.Path.Combine(programFolder, "USER.json");

            string userFridgeFile = System.IO.Path.Combine(programFolder, "USER_FRIDGE.json");

            string foodDefFile = System.IO.Path.Combine(programFolder, "FoodData.json");
            string recipeDatabase = System.IO.Path.Combine(programFolder, "Recipes.json");

            string recipeSaveFile = System.IO.Path.Combine(programFolder, "RecipeSavesFile.json");
            string statSavePath = System.IO.Path.Combine(programFolder, "StatSavePath.json");
            string m_ShoppingListFile = System.IO.Path.Combine(programFolder, "ShoppingList.json");


            ProgramManager currentManager = new ProgramManager(userFile, userFridgeFile, recipeDatabase, foodDefFile, recipeSaveFile, statSavePath, m_ShoppingListFile);

            FoodDef milk = new FoodDef();
            milk.USName = "milk";
            milk.BGName = "мляко";
            milk.USUOM = "Grams";
            milk.BGUOM = "Грамове";
            milk.SuggestedAmounts = new List<float> { 10, 20, 30, 40 };
            milk.Protein = 10;
            milk.Sodium = 10;
            milk.Calories = 122;
            milk.Fat = 10;
            milk.Sugars = 10;
            milk.Carbs = 10;

            currentManager.UsersFridge.ClearFridge();
            Assert.IsTrue(currentManager.UsersFridge.USUsersFoods.Count == 0);
            Assert.IsTrue(currentManager.UsersFridge.BGUserFoods.Count == 0);

            currentManager.UsersFridge.AddToFridge(milk);
            Assert.IsTrue(currentManager.UsersFridge.USUsersFoods.Count == 1);
            Assert.IsTrue(currentManager.UsersFridge.USUsersFoods.Count == 1);


            Dictionary<string, FoodDef> UserFoods = currentManager.UsersFridge.UsersFridge();

            Assert.IsTrue(UserFoods.Count == currentManager.UsersFridge.USUsersFoods.Count);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].USName == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].USName);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].BGName == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].BGName);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].USUOM == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].USUOM);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].BGUOM == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].BGUOM);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].Protein == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].Protein);
            Assert.IsTrue(UserFoods[milk.USName.ToLower()].SuggestedAmounts[0] == currentManager.UsersFridge.USUsersFoods[milk.USName.ToLower()].SuggestedAmounts[0]);
        }
    }
}
