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
        string FoodItemsFile;
        string RecipiesFile;
        string UserFridgeFile;
        string RecipeSaverSaveFile;
        string StatisticsSaveFile;

        public FridgeTemplate UsersFridge;
        public RecipeManager RecipieManag;
        public FoodManager FoodManag;
        public UserTemplate User;
        public RecipeSaver UserRecipeSaves;
        public StatisticsManager StatManager;

        public List<string> CompatabilityMap = new List<string>
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

        public ProgramManager(string UserFileSave, string UserFridgeFileSave, string RecipieDatabase, string FoodItemsDatabase, string RecipeSaveFile, string StatisticsSavePath)
        {
            UserFile = UserFileSave;

            UserFridgeFile = UserFridgeFileSave;

            RecipeSaverSaveFile = RecipeSaveFile;
            StatisticsSaveFile = StatisticsSavePath;

            User = new UserTemplate(this);
            User = GetUser();
            User.CurrentManager = this;
            InitFridge(User);

            FoodItemsFile = FoodItemsDatabase;
            RecipiesFile = RecipieDatabase;

            FoodManag = new FoodManager(FoodItemsFile);

            UserRecipeSaves = new RecipeSaver(RecipeSaverSaveFile);

            UserRecipeSaves = GetRecipeSaver();
            UserRecipeSaves.SaveLocation = RecipeSaverSaveFile;
            UserRecipeSaves.SaveRecipeSaver();

            StatManager = new StatisticsManager(StatisticsSaveFile)
            {
                SaveLocation = StatisticsSaveFile
            };
            StatManager.SaveStatistics();

            RecipieManag = new RecipeManager(RecipiesFile, this);
        }

        /// <summary>
        /// Gets saved recipe saver. If the save file doesn't exsit, it uses the default save file. 
        /// If that doesnt exsit it creates a save file and contiunes work as normal.
        /// </summary>
        /// <returns></returns>
        public RecipeSaver GetRecipeSaver()
        {
            RecipeSaver RetrivedSaver;
            if (!File.Exists(RecipeSaverSaveFile))
            {
                UserRecipeSaves.SaveRecipeSaver();
            }

            using (StreamReader file = File.OpenText(RecipeSaverSaveFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                RetrivedSaver = (RecipeSaver)serializer.Deserialize(file, typeof(RecipeSaver));
                return RetrivedSaver;
            }
        }

        private StatisticsManager GetStatisticManager()
        {
            StatisticsManager RetrivedManager;

            if (!File.Exists(StatisticsSaveFile))
            {
                StatManager.SaveStatistics();
            }

            using (StreamReader file = File.OpenText(StatisticsSaveFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                RetrivedManager = (StatisticsManager)serializer.Deserialize(file, typeof(StatisticsManager));
                return RetrivedManager;
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
                SaveUser();
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
            UsersFridge = new FridgeTemplate(UserFridgeFile);
            UserToUse.UserFridge = UsersFridge;
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
