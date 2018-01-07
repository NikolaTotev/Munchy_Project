using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Nikola.Munchy.MunchyAPI;
using System.IO;
using System.Collections.Generic;
namespace Nikola.Munchy.MunchyTester
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string UserFridge = @"d:\Desktop\JSON_FridgeTest.json";
            string TestUsersFile = @"d:\Desktop\USERS.json";
            string Recipies;
            string FoodItems;
            List<UserTemplate> testUsers = new List<UserTemplate>
            {
               new UserTemplate() {UserName = "Nikola", Age = 17, Sex = "Male"}
            };

            using (StreamWriter file = File.CreateText(TestUsersFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, testUsers);
            }




            ProgramManager testManager = new ProgramManager(TestUsersFile, UserFridge);
            Assert.IsTrue(testManager.Users.Count == 1, "The Test manager user list must have a user in it");
        }
    }
}
