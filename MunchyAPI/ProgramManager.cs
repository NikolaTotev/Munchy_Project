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
        public string UserFile;
        string DefaultUserFile;
        string FoodItemsFile;
        string RecipiesFile;
        string UserFridgeFile;
        string DefaultFridgeFile;
        string RecipeSaverSaveFile;
        string DefaultSaverSaveFile;

        public FridgeTemplate UsersFridge;
        public RecipeManager RecipieManag;
        public FoodManager FoodManag;
        public UserTemplate User { get; set; }
        public RecipeSaver UserRecipeSaves;

        public List<string> CompatabilityMap = new List<string>
            {
                "IsVegan",
                "IsVegetarian",
                "IsDiabetic",
                "Eggs",
                "Dairy",
                "Fish",
                "Nuts",
                "Gluten",
                "Soy"
            };

        public ProgramManager(string UserFileSave, string UserFridgeFileSave, string DefaultFridge, string DefaultUser, string RecipieDatabase, string FoodItemsDatabase, string RecipeSaveFile, string DefaultSaverFile)
        {
            FoodItemsFile = FoodItemsDatabase;
            RecipiesFile = RecipieDatabase;

            FoodManag = new FoodManager(FoodItemsFile);

            UserFile = UserFileSave;
            DefaultUserFile = DefaultUser;

            UserFridgeFile = UserFridgeFileSave;
            DefaultFridgeFile = DefaultFridge;

            RecipeSaverSaveFile = RecipeSaveFile;
            DefaultSaverSaveFile = DefaultSaverFile;

            User = new UserTemplate(this);
            User = GetUser();
            User.CurrentManager = this;
            InitFridge(User);

            UserRecipeSaves = new RecipeSaver(RecipeSaverSaveFile);  
            UserRecipeSaves = GetRecipeSaver();
            UserRecipeSaves.SaveLocation = RecipeSaverSaveFile;
            UserRecipeSaves.SaveRecipeSaver();
            
            RecipieManag = new RecipeManager(RecipiesFile, this);

        }

        public RecipeSaver GetRecipeSaver()
        {
            RecipeSaver RetrivedSaver;
            if (!File.Exists(RecipeSaverSaveFile))
            {
                if (File.Exists(DefaultSaverSaveFile))
                {
                    using (StreamReader file = File.OpenText(DefaultSaverSaveFile))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        RetrivedSaver = (RecipeSaver)serializer.Deserialize(file, typeof(RecipeSaver));
                        return RetrivedSaver;
                    }
                }
                else
                {
                    UserRecipeSaves.SaveRecipeSaver();
                    UserRecipeSaves.SaveLocation = RecipeSaverSaveFile;
                }
            }

            using (StreamReader file = File.OpenText(RecipeSaverSaveFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                RetrivedSaver = (RecipeSaver)serializer.Deserialize(file, typeof(RecipeSaver));
                return RetrivedSaver;
            }
        }

        /// <summary>
        /// If the "UserFile" exists, the "User" instance gets assigned the returned "UserTemplate". If the file does not exist
        /// i.e. there has not been a user created yet, the User gets assinged to Null. 
        /// </summary>
        /// <returns></returns>
        public UserTemplate GetUser()
        {
            UserTemplate RetrievedUser;
            if (!File.Exists(UserFile))
            {
                using (StreamReader file = File.OpenText(DefaultUserFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    RetrievedUser = (UserTemplate)serializer.Deserialize(file, typeof(UserTemplate));
                    return RetrievedUser;
                }
            }

            using (StreamReader file = File.OpenText(UserFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                RetrievedUser = (UserTemplate)serializer.Deserialize(file, typeof(UserTemplate));
                return RetrievedUser;
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
        public void CreateUser(string UserName, string UserSex, int UserAge)
        {
            UserTemplate newUser = new UserTemplate(this)
            {
                UserName = UserName,
                Sex = UserSex,
                Age = UserAge
            };

            InitFridge(newUser);
            User = newUser;
        }

        public void SaveUser()
        {
            using (StreamWriter file = File.CreateText(UserFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, User);
            }
        }
    }
}
