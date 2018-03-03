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
        string ShoppingListSaveFile;

        public FridgeTemplate UsersFridge;
        public RecipeManager RecipieManag;
        public FoodManager FoodManag;
        public UserTemplate User;
        public RecipeSaver UserRecipeSaves;
        public StatisticsManager StatManager;
        public ShoppingList UserShoppingList;

        //Compatability map is used for determining which recipes are compatable with the users preferences.
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

        public ProgramManager(string UserFileSave, string UserFridgeFileSave, string RecipieDatabase, string FoodItemsDatabase, string RecipeSaveFile, string StatisticsSavePath, string ShoppingListSave)
        {
            UserFile = UserFileSave;

            UserFridgeFile = UserFridgeFileSave;

            RecipeSaverSaveFile = RecipeSaveFile;
            StatisticsSaveFile = StatisticsSavePath;
            ShoppingListSaveFile = ShoppingListSave;

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
            UserShoppingList = new ShoppingList();
            UserShoppingList = GetShoppingList();
            User.UserShoppingList = UserShoppingList;
        }

        //Deserializes the recipe saver. Recipe saver is used to save recently viewed recipe, saved recipes and cooked recipes.
        public ShoppingList GetShoppingList()
        {
            ShoppingList ShoppingListToReturn;
            if (!File.Exists(ShoppingListSaveFile))
            {
                SaveShoppingList();
            }

            using (StreamReader file = File.OpenText(ShoppingListSaveFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                ShoppingListToReturn = (ShoppingList)serializer.Deserialize(file, typeof(ShoppingList));
                return ShoppingListToReturn;
            }
        }

        //Handles saving the shopping list.
        public void SaveShoppingList()
        {
            using (StreamWriter file = File.CreateText(ShoppingListSaveFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, UserShoppingList);
            }

            User.UserShoppingList = GetShoppingList();
        }

        //Deserializes the recipe saver. Recipe saver is used to save recently viewed recipe, saved recipes and cooked recipes.
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

        //Deserializes the statistics manager.
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

        //Gets the user by deserializing the user save file.
        public UserTemplate GetUser()
        {
            UserTemplate RetrievedUser;
            if (!File.Exists(UserFile))
            {
                SaveUser();
                User.Weight = 50;
                User.Age = 20;
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

        //Handles saving the user.
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
