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

        [NonSerialized]
        public Dictionary<string, bool> Preferences;

        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }
        public bool IsDiabetic { get; set; }
        public bool HasAlergies { get; set; }
        public bool A_Nuts { get; set; }
        public bool A_Milk { get; set; }
        public bool A_Fish { get; set; }
        public bool A_Eggs { get; set; }
        public bool A_Soy { get; set; }

        public string UserName { get; set; }
        public int CalorieTracker { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public int Age { get; set; }



        public UserTemplate()
        {
            Preferences = new Dictionary<string, bool> { { "IsVegan", IsVegan },
                { "IsVegetarian", IsVegetarian },
                { "IsDiabetic", IsDiabetic },
                { "HasAlergies", HasAlergies },
                { "A_Nuts", A_Nuts },
                { "A_Milk", A_Milk },
                { "A_Fish", A_Fish },
                { "A_Eggs", A_Eggs },
                { "A_Soy", A_Soy }
            };
        }

    }






}

