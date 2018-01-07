using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace Nikola.Munchy.MunchyAPI

{
    public class RecipeManager
    {
        public string RecipeDatabaseFile;

        public RecipeManager(string DatabaseToUse)
        {
            RecipeDatabaseFile = DatabaseToUse;
        }

        /// <summary>
        /// Returns a dictionary based on a specified JSON file. 
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, RecipeDef> GetRecipies()
        {
            // Checks if given file exists.
            if (!File.Exists(RecipeDatabaseFile))
            {
                // If file does not exist an exeption is thrown.
                throw new Exception(string.Format("Given database file {0} does not exist or cant be accessed", RecipeDatabaseFile));
            }

            // Using stream reader a the JSON file is opened.
            using (StreamReader file = File.OpenText(RecipeDatabaseFile))
            {
                // A new JsonSerializer is created that is used to deserialize the JSON file
                JsonSerializer serializer = new JsonSerializer();
                // A dictionary "Recipies" is created and its euqual to the serialized dicionary in the file.
                Dictionary<string, RecipeDef> Recipes = (Dictionary<string, RecipeDef>)serializer.Deserialize(file, typeof(Dictionary<string, RecipeDef>));
                return Recipes;
            }

        }

    }
}
