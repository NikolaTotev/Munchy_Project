using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    public static class UnitConverter
    {
        //Dictionary of standard values that are used in the program.
        private static Dictionary<string, float> StandardUnits = new Dictionary<string, float> { { "Cup", 236 }, { "Tbsp", 15 }, { "Tsp", 15 } };

        //Gets the amount needed to be removed based on what unit is being used and the density of the given food being modified. Returns a float.
        public static float GetAmountToRemove(string FoodName, float FoodAmount, string Unit, FoodManager foodManager)
        {
            float Mass = 0;
            float FoodDensity = 0;
            float VolumeToUse = 0;

            if (Unit != "ml" && Unit != "Count")
            {
                if (foodManager.Foods.ContainsKey(FoodName))
                    FoodDensity = foodManager.Foods[FoodName].FoodDensity;

                if (StandardUnits.ContainsKey(Unit))
                     VolumeToUse = StandardUnits[Unit];

                Mass = FoodAmount * (VolumeToUse * FoodDensity);
            }

            if (Unit == "ml")
            {
                Mass = FoodAmount * StandardUnits[Unit];
            }
            
            if (Unit == "Count")
            {
                Mass = FoodAmount;
            }         

            if (Unit == "g")
            {
                Mass = FoodAmount;
            }
            return Mass;
        }
    }
}
