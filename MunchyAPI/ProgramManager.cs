using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Nikola.Munchy.MunchyAPI
{
    public class ProgramManager
    {
        string UserFile;
        string DefaultUserFile;
        string FoodItemsFile;
        string RecipiesFile;
        string UserFridgeFile;
        string DefaultFridgeFile;
        public FridgeTemplate UsersFridge;
        public UserTemplate User { get; set; }

        public ProgramManager(string UserFileSave, string UserFridgeFileSave, string DefaultFridge, string DefaultUser)
        {
            UserFile = UserFileSave;
            DefaultUserFile = DefaultUser;
            //FoodItemsFile = FoodItemsFileSave;
            // RecipiesFile = RecipiesFileSave;
            UserFridgeFile = UserFridgeFileSave;
            DefaultFridgeFile = DefaultFridge;
            User = new UserTemplate();
            User = GetUser();
            InitFridge(User);
        }

        /// <summary>
        /// If the "UserFile" exists, the "User" instance gets assigned the returned "UserTemplate". If the file does not exist
        /// i.e. there has not been a user created yet, the User gets assinged to Null. 
        /// </summary>
        /// <returns></returns>
        public UserTemplate GetUser()
        {
            UserTemplate RetrievedUsers;
            if (!File.Exists(UserFile))
            {
                using (StreamReader file = File.OpenText(DefaultUserFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    RetrievedUsers = (UserTemplate)serializer.Deserialize(file, typeof(UserTemplate));
                    return RetrievedUsers;
                }
            }

            using (StreamReader file = File.OpenText(UserFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                RetrievedUsers = (UserTemplate)serializer.Deserialize(file, typeof(UserTemplate));
                return RetrievedUsers;
            }

        }


        /// <summary>
        /// Creates a new FridgeTemplate instance and assigns it to the UserTemplate instance frige. 
        /// The "FridgeTemplate" class handles its own serialization/deserialization. 
        /// </summary>
        /// <param name="UserToUse"></param>
        public void InitFridge(UserTemplate UserToUse)
        {
            UsersFridge = new FridgeTemplate(UserFridgeFile, DefaultFridgeFile);
            UserToUse.UserFridge = UsersFridge;
        }

        /// <summary>
        /// Function is used to create brand new user. This function should be called by the UI part of the project when it detects 
        /// that the "User" that the "ProgramManager" has is equal to "null". This function takes in user input in the form of the 
        /// given arguments it takes.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Sex"></param>
        /// <param name="Age"></param>
        public void CreateUser(string Name, string Sex, int Age)
        {
            UserTemplate newUser = new UserTemplate();
            newUser.UserName = Name;
            newUser.Sex = Sex;
            InitFridge(newUser);
            User = newUser;
        }

    }
}
