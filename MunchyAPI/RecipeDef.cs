using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    public class RecipeDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Ingredients { get; set; }
        public string ImageFile { get; set; }
    }
}
