using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
namespace Nikola.Munchy.MunchyAPI
{
    public class UserTemplate
    {      
        [NonSerialized]
        public FridgeTemplate UserFridge;

        [NonSerialized]
        public ShoppingList UserShoppingList;

        [NonSerialized]
        public ProgramManager CurrentManager;

        public List<string> Preferences = new List<string>();
        public string UserName { get; set; }
        public int CalorieTracker { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public int CompatabilityIndex { get; set; }
        public string LanguagePref { get; set; }

        public UserTemplate(ProgramManager Manager)
        {
            CurrentManager = Manager;
            CompatabilityIndex = 0;
            CalculateIndex();
        }

        //Calculates compatability index based one the compatabilitymap in the ProgramManager. For more information on how it works visit the github wiki.
        public void CalculateIndex()
        {
            CompatabilityIndex = 0;
            foreach (string tag in Preferences)
            {
                CompatabilityIndex += (int)Math.Pow(2, (CurrentManager.CompatabilityMap.IndexOf(tag)));
            }
        }
    }
}

