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
            UserTemplate testUser = new UserTemplate();
            testUser.UserName = "Nikola";
            testUser.Age = 17;
            testUser.Sex = "Male";

            string TestFile = @"d:\Desktop\JSON_FridgeTest.json";
            string DefaultFridge = @"d:\Desktop\DEFAULT_FRIDGE.json";
            string UserFile = @"d:\Desktop\USER_TEST.json";
            string DefaultUserFile = @"d:\Desktop\USER_404T.json";

            using (StreamWriter file = File.CreateText(UserFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, testUser);
            }
            Assert.IsTrue(File.Exists(UserFile), "User File should have been cteated");

            ProgramManager testManager = new ProgramManager(UserFile, TestFile, DefaultFridge, DefaultUserFile);
            Assert.IsTrue(testManager.User != null, "The instance of the user in testmanager shouldn't be null");
            Assert.IsTrue(testManager.User.UserName == testUser.UserName, "The name of the User instance in testManager should be the same as the TestUser");


        }

        // TestMethod2 Tests when the ProgramManager is given files that don't exsist(in this case TestFile and UserFile). 
        // If the functions are working correctly the 
        [TestMethod]
        public void TestMethod2()
        {
            string TestFile = @"d:\Desktop\JON_FridgeTest.json";
            string DefaultFridge = @"d:\Desktop\DEFAULT_FRIDGE.json";
            string UserFile = @"d:\Desktop\USR_404T.json";
            string DefaultUserFile = @"d:\Desktop\USER_404T.json";


            ProgramManager testManager = new ProgramManager(UserFile, TestFile, DefaultFridge, DefaultUserFile);
            Assert.IsTrue(testManager.User.UserName == null, "The instance of the user in testmanager should be set to default user as the user file does not exsit");
            Assert.IsTrue(testManager.User.UserFridge.UsersFoods.ContainsKey("bread"), "UserFrige should be set to default fridge");
            //Assert.IsTrue(testManager.User.UserName == testUser.UserName, "The name of the User instance in testManager should be the same as the TestUser");

            if (testManager.User.UserName == null)
            {
                testManager.CreateUser("Niky", "Male", 17);
            }

            Assert.IsTrue(testManager.User.UserName == "Niky", "The instance of the user in testmanager should be set to default user as the user file does not exsit");
        }
    }
}
