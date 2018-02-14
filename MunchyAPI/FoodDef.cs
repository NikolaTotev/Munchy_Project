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
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Fat { get; set; }
        public int Carbs { get; set; }
        public int Sugars { get; set; }
        public int Sodium { get; set; }
        public float Amount { get; set; }
        public string UOM { get; set; }
        public string ImageFileName { get; set; }        
        public List<float> SuggestedAmounts { get; set; }
    }
}
