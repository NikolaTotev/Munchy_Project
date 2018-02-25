using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    public class RecipeDef
    {
        public string TypeOfFood { get; set; }
        public string USName { get; set; }
        public string BGName { get; set; }
        public string USDirections { get; set; }
        public string BGDirections { get; set; }
        public string TimeToCook { get; set; }
        public List<string> USIngredients { get; set; }
        public List<string> BGIngredients { get; set; }
        public List<float> Amounts { get; set; }
        public List<string> TimeTags { get; set; }
        public List<string> UserTags { get; set; }
        public List <string> Units { get; set; }
        public string ImageFile { get; set; }
        public int RecipeIndex { get; set; }
        public int Calories { get; set; }                
    }
}
