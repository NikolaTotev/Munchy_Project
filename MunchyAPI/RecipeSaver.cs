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
        public List<string> SavedRecipes;
        public List<string> CookedRecipes;
        public List<string> RecentlyViewed;
        public List<string> CookedToday;

        [NonSerialized]
        public string SaveLocation;
        //string DefaultSaver = @"C:\Users\Nikola\AppData\Roaming\Munchy\DefaultSaverFile.json";


        public RecipeSaver(string SavePath)
        {
            SaveLocation = SavePath;
            SavedRecipes = new List<string>();
            CookedRecipes = new List<string>();
            RecentlyViewed = new List<string>();
            CookedToday = new List<string>();
        }

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
