using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Nikola.Munchy.MunchyAPI
{
    public class ProgramManager
    {
        string UserFile;
        string FoodItemsFile;
        string RecipiesFile;
        string UserFridgeFile;

        public List<UserTemplate> Users { get; set; }

        public ProgramManager(string UserFileSave,string UserFridgeFileSave)
        {
            UserFile = UserFileSave;
            //FoodItemsFile = FoodItemsFileSave;
           // RecipiesFile = RecipiesFileSave;
            UserFridgeFile = UserFridgeFileSave;
            Users = GetUsers();
        }

        public List<UserTemplate> GetUsers()
        {
         List<UserTemplate> RetrievedUsers = new List<UserTemplate>();
            if (!File.Exists(UserFile))
            {
                throw new Exception(string.Format("Create New User"));
                //return RetrievedUsers;
            }
            else
            {
                using (StreamReader file = File.OpenText(UserFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    RetrievedUsers = (List<UserTemplate>)serializer.Deserialize(file, typeof(List<UserTemplate>));
                    return RetrievedUsers;
                }
            }
        }

        public void InitUsers()
        {
           
        }

        public void InitFridges()
        {

        }

        public void CreateUser(string Name, string Sex, int Age)
        {
            Users = new List<UserTemplate>();
            FridgeTemplate newFridge = new FridgeTemplate(UserFridgeFile);
            UserTemplate newUser = new UserTemplate(newFridge);
            newUser.UserName = Name;
            newUser.Sex = Sex;
            Users.Add(newUser);
        }

    }
}
