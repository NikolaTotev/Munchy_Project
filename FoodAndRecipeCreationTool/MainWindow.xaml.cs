using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Nikola.Munchy.MunchyAPI;
using Newtonsoft.Json;
using System.IO;

namespace FoodAndRecipeCreationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string ProgramFolder = LocalAppDataPath + "\\Munchy";

        string FoodData = ProgramFolder + "\\FoodData.json";
        string RecipeDatabase = ProgramFolder + "\\Recipes.json";

        Dictionary<string, FoodDef> FoodList;
        Dictionary<string, RecipeDef> RecipeList;
        public MainWindow()
        {
            InitializeComponent();
            FoodList = GetFoodList();
            RecipeList = GetRecipies();
        }

        /// <summary>
        /// Returns a dictonary that is full of the food definitions in the JSON file.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FoodDef> GetFoodList()
        {
            // Throws Exception when the given file path doesn't exist or can't be accessed.
            if (!File.Exists(FoodData))
            {
                MessageBox.Show("ERROR F404 : FoodList file is corrupt or doesn't exist. Please visit == GITHUB LINK == for potential Fixes");
                return FoodList = new Dictionary<string, FoodDef>();
            }


            //Deserializes the JSON directly from file.
            using (StreamReader file = File.OpenText(FoodData))
            {
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, FoodDef> Foods = (Dictionary<string, FoodDef>)serializer.Deserialize(file, typeof(Dictionary<string, FoodDef>));
                return Foods;
            }
        }

        public Dictionary<string, RecipeDef> GetRecipies()
        {
            // Checks if given file exists.
            if (!File.Exists(RecipeDatabase))
            {
                MessageBox.Show("ERROR R404 : RecipeList file is corrupt or doesn't exist. Please visit == GITHUB LINK == for potential Fixes");
            }

            // Using stream reader a the JSON file is opened.
            using (StreamReader file = File.OpenText(RecipeDatabase))
            {
                // A new JsonSerializer is created that is used to deserialize the JSON file
                JsonSerializer serializer = new JsonSerializer();
                // A dictionary "Recipies" is created and its euqual to the serialized dicionary in the file.
                Dictionary<string, RecipeDef> Recipes = (Dictionary<string, RecipeDef>)serializer.Deserialize(file, typeof(Dictionary<string, RecipeDef>));
                return Recipes;
            }

        }

        public void SaveFoodList()
        {
            using (StreamWriter file = File.CreateText(FoodData))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, FoodList);
            }
        }

        public void SaveRecipeList()
        {
            using (StreamWriter file = File.CreateText(RecipeDatabase))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, RecipeList);
            }
        }


        private void Btn_OpenRecipeTool_Click(object sender, RoutedEventArgs e)
        {
            if (P_RecipeTool.Visibility == Visibility.Hidden)
            {
                P_RecipeTool.Visibility = Visibility.Visible;
                P_FoodTool.Visibility = Visibility.Hidden;
            }
            else
            {
                P_RecipeTool.Visibility = Visibility.Hidden;
            }
        }

        private void Btn_OpenFoodTool_Click(object sender, RoutedEventArgs e)
        {
            if (P_FoodTool.Visibility == Visibility.Hidden)
            {
                P_FoodTool.Visibility = Visibility.Visible;
                P_RecipeTool.Visibility = Visibility.Hidden;
            }
            else
            {
                P_FoodTool.Visibility = Visibility.Hidden;
            }
        }

        private void BTN_RecipeTool_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Btn_SaveFoodItem_Click(object sender, RoutedEventArgs e)
        {
            FoodDef NewFoodItem = new FoodDef();
            NewFoodItem.Name = tb_InputFoodName.Text;
            NewFoodItem.Calories = int.Parse(tb_CalorieInput.Text);
            NewFoodItem.Protein =  int.Parse(tb_ProteinInput.Text);
            NewFoodItem.Fat = int.Parse(tb_FatInput.Text);
            NewFoodItem.Carbs = int.Parse(tb_CarbInput.Text);
            NewFoodItem.Sugars = int.Parse(tb_SugarInput.Text);
            NewFoodItem.Sodium = int.Parse(tb_SodiumInput.Text);
            NewFoodItem.UOM = tb_UOMInput.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RecipeDef NewRecipeDef = new RecipeDef();

        }
    }
}
