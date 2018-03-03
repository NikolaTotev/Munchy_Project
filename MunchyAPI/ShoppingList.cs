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
        public List<string> USFoodsToBuy = new List<string>();
        public List<string> BGFoodsToBuy = new List<string>();

        public void AddToShoppingList(string USFoodToAdd, string BGFoodToAdd)
        {
            USFoodsToBuy.Add(USFoodToAdd);
            BGFoodsToBuy.Add(BGFoodToAdd);
        }

        public void RemoveFromShoppingList(int positionToRemoveAt)
        {
            USFoodsToBuy.RemoveAt(positionToRemoveAt);
            BGFoodsToBuy.RemoveAt(positionToRemoveAt);
        }

        public void ClearList()
        {
            USFoodsToBuy.Clear();
        }

       
    }
}
