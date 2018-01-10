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
        string UserSaveFile;
        string UserFridgeSave;

        // Add CookBook (CookBook class has to be created)
        // Add FBT (Food Balance Tracker)
        // Add BirthDay

        [NonSerialized]
        public FridgeTemplate UserFridge;

        public ProgramManager CurrentManager;

        public List<string> Preferences;


        public string UserName { get; set; }
        public int CalorieTracker { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }
        public int CompatabilityIndex { get; set; }



        public UserTemplate()
        {           
            Preferences = new List<string>();
        }
              
    }






}

