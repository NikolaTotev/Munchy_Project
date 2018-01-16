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
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public List<float> Amounts { get; set; }
        public List<string> TimeTags { get; set; }
        public List<string> UserTags { get; set; }
        public string ImageFile { get; set; }
        public int RecipeIndex { get; set; }

       
    }
}
