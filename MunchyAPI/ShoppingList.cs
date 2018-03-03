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
        public List<string> FoodsToBuy = new List<string>();    

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

       
    }
}
