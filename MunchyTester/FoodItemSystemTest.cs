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
            int UserIndex = 0;
            int RecipeIndex1 = 0;
            int RecpieIndex2 = 0;
            List<string> CompatabilityMap = new List<string>
            {
                "IsVegan",
                "IsVegetarian",
                "IsDiabetic",
                "HasAlergies",
                "Eggs",
                "Dairy",
                "Fish",
                "Nuts",
                "Soy"
            };

            List<string> UserPreferences = new List<string>
            {
                "IsVegetarian"
               
            };

            List<string> R1 = new List<string>
            {
                "IsVegetarian",
                "IsDiabetic"
            };

            List<string> R2 = new List<string>
            {
                "HasAlergies",
                "Dairy",
            };

            UserTemplate testUser = new UserTemplate
            {
                Preferences = UserPreferences
            };


            foreach (string tag in testUser.Preferences)
            {
                UserIndex += 2 ^ CompatabilityMap.IndexOf(tag);
            }

            RecipeDef testDef1 = new RecipeDef();
            RecipeDef testDef2 = new RecipeDef();

            testDef1.UserTags = R1;
            testDef2.UserTags = R2;

            foreach (string tag in testDef1.UserTags)
            {
                RecipeIndex1 += 2 ^ CompatabilityMap.IndexOf(tag);
            }

            foreach (string tag in testDef2.UserTags)
            {
                RecpieIndex2 += 2 ^ CompatabilityMap.IndexOf(tag);
            }

            bool GoodRecipie;

            GoodRecipie = ((UserIndex & RecipeIndex1) == UserIndex);
            
            Assert.IsTrue(GoodRecipie);

            GoodRecipie = ((UserIndex & RecpieIndex2) == UserIndex);
            
            Assert.IsFalse(GoodRecipie);

        }


    }
}
