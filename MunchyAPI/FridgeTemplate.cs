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

        //Stores foods with the key being the bulgarian name.
        public Dictionary<string, FoodDef> BGUserFoods { get; set; }

        //Stores foods with the key being the English name. This is the main list.
        public Dictionary<string, FoodDef> USUsersFoods { get; set; }

        public string SavedFilePath;
        public string DefaultPath;

        public FridgeTemplate(string FilePathToUse)
        {
            SavedFilePath = FilePathToUse;
            USUsersFoods = UsersFridge();
            SaveFridge();

            BGUserFoods = new Dictionary<string, FoodDef>();
            RefreshBGList();
        }

        //Handles updating the BGUserFoods dictionary
        public void RefreshBGList()
        {
            BGUserFoods.Clear();
            foreach (KeyValuePair<string, FoodDef> element in USUsersFoods)
            {
                BGUserFoods.Add(element.Value.BGName, element.Value);
            }
        }

        /// <summary>
        /// Adds a given item to the fridge.
        /// </summary>
        /// <param name="ItemToAdd"></param>
        public void AddToFridge(FoodDef ItemToAdd)
        {
            USUsersFoods.Add(ItemToAdd.USName, ItemToAdd);
            RefreshBGList(); RefreshBGList();
            SaveFridge();
        }

        /// <summary>
        /// Removes a given item from the fridge.
        /// /// </summary>
        /// <param name="ItemToRemove"></param>
        public void RemoveFromFridge(string ItemToRemove)
        {
            USUsersFoods.Remove(ItemToRemove);
            RefreshBGList();
            SaveFridge();
        }

        /// <summary>
        /// Takes a string array, loops through the array checknig if the fridge dictionary contains such a key. 
        /// If it doesn't contain even one item it breaks the loop and returns false.
        /// </summary>
        /// <param name="foods"></param>
        /// <returns></returns>
        public bool FridgeConatains(List<string> FoodItemsToChange, List<float> AmountsToChange, List<string> Units, FoodManager foodManager)
        {
            if(FoodItemsToChange != null)
            {
                for (int i = 0; i < FoodItemsToChange.Count; i++)
                {
                    float AmountToRemove = UnitConverter.GetAmountToRemove(FoodItemsToChange[i], AmountsToChange[i], Units[i], foodManager);

                    if (USUsersFoods.ContainsKey(FoodItemsToChange[i]))
                    {
                        foreach (KeyValuePair<string, FoodDef> element in USUsersFoods)
                        {
                            if (element.Value.USName == FoodItemsToChange[i] && element.Value.Amount - AmountToRemove < 0)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }            
            // Only if requirements are met does the function return true.
            return true;
        }

        /// <summary>
        /// After using a certian product changes the Amount left, or if the AmountToChange is 0 removes the Item
        /// </summary>
        /// <param name="FoodItemToChange"></param>
        /// <param name="AmountToChange"></param>
        public void ModifyFoodItemAmount(List<string> FoodItemsToChange, List<float> AmountsToChange, List<string>Units, FoodManager foodManager)
        {
            for (int i = 0; i < FoodItemsToChange.Count; i++)
            {
                float AmountToRemove = UnitConverter.GetAmountToRemove(FoodItemsToChange[i], AmountsToChange[i], Units[i], foodManager);

                foreach (KeyValuePair<string, FoodDef> element in USUsersFoods)
                {
                    if (element.Value.USName.ToLower() == FoodItemsToChange[i].ToLower() && element.Value.Amount - AmountToRemove >= 0)
                    {
                        element.Value.Amount -= AmountToRemove;
                    }                                                       
                }
            }

            for (int i = 0; i < USUsersFoods.Count; i++)
            {
                if(USUsersFoods.ElementAt(i).Value.Amount == 0)
                {
                    USUsersFoods.Remove(USUsersFoods.ElementAt(i).Key);
                }
            }
            
            SaveFridge();
        }

        /// <summary>
        ///  Creates a dictionary conatining "FoodDefs" based on a JSON file. If the file does not exist, a default preset is used.
        ///  When the default preset is used it means that this is the first time a fridge is created (meaning its the first time the program is run)
        ///  /// </summary>
        /// <returns></returns>
        public Dictionary<string, FoodDef> UsersFridge()
        {
            if (!File.Exists(SavedFilePath))
            {
                Dictionary<string, FoodDef> LoadedDictionary = new Dictionary<string, FoodDef>();
                SaveFridge();
                return LoadedDictionary;
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
            using (StreamWriter file = File.CreateText(SavedFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, USUsersFoods);
            }
        }

        /// <summary>
        /// Removes all items from the fridge.
        /// </summary>
        public void ClearFridge()
        {
            USUsersFoods.Clear();
            RefreshBGList();
        }
    }
}
