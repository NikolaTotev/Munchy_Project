using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    public static class UnitConverter
    {
        private static Dictionary<string, float> StandardUnits = new Dictionary<string, float> { { "Cup", 236 }, { "Tbsp", 15 }, { "Tsp", 15 } };

        public static float GetMass(string FoodName, float FoodAmount, string Unit, FoodManager foodManager)
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
                Mass = FoodAmount * StandardUnits[Unit];

            if (Unit == "Count")
                Mass = FoodAmount;

            return Mass;
        }
    }
}
