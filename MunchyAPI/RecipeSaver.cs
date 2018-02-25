using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Nikola.Munchy.MunchyAPI
{
    public class RecipeSaver
    {
        public List<string> USSavedRecipes;
        public List<string> USCookedRecipes;
        public List<string> USRecentlyViewed;
        public List<string> USCookedToday;

        public List<string> BGSavedRecipes;
        public List<string> BGCookedRecipes;
        public List<string> BGRecentlyViewed;
        public List<string> BGCookedToday;

        [NonSerialized]
        public string SaveLocation;

        public RecipeSaver(string SavePath)
        {
            SaveLocation = SavePath;
            USSavedRecipes = new List<string>();
            USCookedRecipes = new List<string>();
            USRecentlyViewed = new List<string>();
            USCookedToday = new List<string>();

            BGSavedRecipes = new List<string>();
            BGCookedRecipes = new List<string>();
            BGRecentlyViewed = new List<string>();
            BGCookedToday = new List<string>();
        }

        //Handles saving the recipe saver. Files saved to JSON. 
        public void SaveRecipeSaver()
        {
            using (StreamWriter file = File.CreateText(SaveLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }
    }


}
