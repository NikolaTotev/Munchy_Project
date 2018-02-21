using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    public class FoodDef
    {
        public string USName { get; set; }
        public string BGName { get; set; }
        public float Calories { get; set; }
        public float Protein { get; set; }
        public float Fat { get; set; }
        public float Carbs { get; set; }
        public float Sugars { get; set; }
        public float Sodium { get; set; }
        public float Amount { get; set; }
        public string USUOM { get; set; }
        public string BGUOM { get; set; }
        public float FoodDensity { get; set; }
        public string IngrAmount { get; set; }
        public string ImageFileName { get; set; }        
        public List<float> SuggestedAmounts { get; set; }
    }
}
