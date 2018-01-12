using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nikola.Munchy.MunchyAPI;
using System.IO;

namespace Nikola.Munchy.MunchyAPI
{
    public class ShoppingList
    {
        string SaveLocation;
        public List<string> FoodsToBuy = new List<string>();


        public ShoppingList(string ShoppingListSaveLocation)
        {
            SaveLocation = ShoppingListSaveLocation;
        }

        public void AddToShoppingList(string FoodToAdd)
        {
            FoodsToBuy.Add(FoodToAdd);
        }

        public void RemoveFromShoppingList(string FoodToRemove)
        {
            FoodsToBuy.Add(FoodToRemove);
        }

        public void ClearList()
        {
            FoodsToBuy.Clear();
        }

        public void SaveShoppingList()
        {
            using (StreamWriter file = File.CreateText(SaveLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }
    }
}
