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
            if (Unit != "ml")
            {
                float FoodDensity = foodManager.Foods[FoodName].FoodDensity;
                float VolumeToUse = StandardUnits[Unit];
                Mass = FoodAmount * (VolumeToUse * FoodDensity);
            }
            else if (Unit == "ml")
                Mass = FoodAmount * StandardUnits[Unit];

            return Mass;
        }
    }
}
