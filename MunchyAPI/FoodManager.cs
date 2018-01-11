using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Nikola.Munchy.MunchyAPI
{
    public class FoodManager
    {
        string DefinitionFilePath;
        public Dictionary<string, FoodDef> Foods;

        /// <summary>
        /// When Application is started and a food manager instance is created, the file path where the data is saved is passed into the constructor of FoodManager
        /// </summary>
        /// <param name="defPath"></param>
        public FoodManager(string defPath)
        {
            DefinitionFilePath = defPath;
            Foods = GetFoodDefinitions();
        }

        /// <summary>
        /// Returns a dictonary that is full of the food definitions in the JSON file.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FoodDef> GetFoodDefinitions()
        {
            // Throws Exception when the given file path doesn't exist or can't be accessed.
            if (!File.Exists(DefinitionFilePath))
            {
                throw new Exception(string.Format("The given file path {0} does not exist or can't be accessed", DefinitionFilePath));
            }

            //Deserializes the JSON directly from file.
            using (StreamReader file = File.OpenText(DefinitionFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, FoodDef> Foods = (Dictionary<string, FoodDef>)serializer.Deserialize(file, typeof(Dictionary<string, FoodDef>));
                return Foods;
            }
        }
    }
}
