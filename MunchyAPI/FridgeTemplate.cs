using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Nikola.Munchy.MunchyAPI
{
    public class FridgeTemplate
    {
        public Dictionary<string, FoodDef> UsersFoods { get; set; }
        public string SavedFilePath;

        public FridgeTemplate(string FilePathToUse)
        {
            SavedFilePath = FilePathToUse;
            // UsersFoods = new Dictionary<string, FoodDef>();
            UsersFoods = UsersFridge();
        }

        /// <summary>
        /// Adds a given item to the fridge.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        public void AddToFridge(FoodDef ItemToAdd)
        {
            UsersFoods.Add(ItemToAdd.Name, ItemToAdd);
            SaveFridge();
        }

        /// <summary>
        /// Removes a given item from the fridge.
        /// /// </summary>
        /// <param name="ItemToRemove"></param>
        public void RemoveFromFridge(FoodDef ItemToRemove)
        {
            UsersFoods.Remove(ItemToRemove.Name);
            SaveFridge();
        }

        /// <summary>
        /// Takes a string array, loops through the array checknig if the fridge dictionary contains such a key. 
        /// If it doesn't contain even one item it breaks the loop and returns false.
        /// </summary>
        /// <param name="foods"></param>
        /// <returns></returns>
        public bool FridgeConatains(string[] foods, float[] amounts)
        {

            for (int i = 0; i < foods.Length; i++)
            {
                // First the function checks if the dictonary contains the item.
                if (!UsersFoods.ContainsKey(foods[i]))
                {
                    return false;
                }

                // Then it gets the "FoodItem" that corresponds to the tag that was just checked.
                UsersFoods.TryGetValue(foods[i], out FoodDef FoodItem);

                // From the FoodItem it checks if there is the proper amount.
                if (FoodItem.Amount < amounts[i])
                {
                    return false;
                }
            }

            // Only if both requirements are met does the function return true.
            return true;
        }

        /// <summary>
        /// After using a certian product changes the Amount left, or if the AmountToChange is 0 removes the Item
        /// </summary>
        /// <param name="FoodItemToChange"></param>
        /// <param name="AmountToChange"></param>
        public void ModifyFoodItemAmount(string FoodItemToChange, float AmountToChange)
        {
            if (AmountToChange != 0)
                UsersFoods[FoodItemToChange].Amount = AmountToChange;

            if (AmountToChange == 0)
                UsersFoods.Remove(FoodItemToChange);

            SaveFridge();
        }

        /// <summary>
        ///  Reads the JSON file that stores the fridge information, deserializes it and assigns the "UsersFoods" to the deserialized dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FoodDef> UsersFridge()
        {
            if (!File.Exists(SavedFilePath))
            {
                throw new Exception(string.Format("File {0} does not exsit or can't be accessed."));
            }

            using (StreamReader file = File.OpenText(SavedFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, FoodDef> LoadedDictionary = (Dictionary<string, FoodDef>)serializer.Deserialize(file, typeof(Dictionary<string, FoodDef>));
                return LoadedDictionary;
            }
        }

        /// <summary>
        /// Save fridge function.It is called every time an element is added or removed. That way data loss is avoided.
        /// </summary>
        public void SaveFridge()
        {
            if (!File.Exists(SavedFilePath))
            {
                throw new Exception(string.Format("File {0} does not exsit or can't be accessed."));
            }

            using (StreamWriter file = File.CreateText(SavedFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, UsersFoods);
            }
        }
    }
}
