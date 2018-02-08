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

        List<CheckBox> SettingOptions;

        Dictionary<string, FoodDef> FoodList;
        Dictionary<string, RecipeDef> RecipeList;

        List<string> Ingredients = new List<string>();
        List<float> Amounts = new List<float>();
        List<string> Preferences = new List<string>();

        public List<string> CompatabilityMap = new List<string>
            {
                "isvegan",
                "isvegetarian",
                "isdiabetic",
                "eggs",
                "dairy",
                "fish",
                "nuts",
                "gluten",
                "soy"
            };

        public MainWindow()
        {
            InitializeComponent();
            FoodList = GetFoodList();
            RecipeList = GetRecipies();
            SaveRecipeList();
            SaveFoodList();
            SettingOptions = new List<CheckBox> { CB_IsVegan, CB_IsVegetarian, CB_IsDiabetic, CB_Eggs, CB_Dairy, CB_Fish, CB_Nuts, CB_Gluten, CB_Soy };
        }

        /// <summary>
        /// Returns a dictonary that is full of the food definitions in the JSON file.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FoodDef> GetFoodList()
        {
            Dictionary<string, FoodDef> ReturnedFoodList;
            // Throws Exception when the given file path doesn't exist or can't be accessed.
            if (!File.Exists(FoodData))
            {
                MessageBox.Show("ERROR F404 : FoodList file is corrupt or doesn't exist. Please visit == GITHUB LINK == for potential Fixes");
                ReturnedFoodList = new Dictionary<string, FoodDef>();
                return ReturnedFoodList;
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
                Dictionary<string, RecipeDef> ReturnedDataBase;
                MessageBox.Show("ERROR R404 : RecipeList file is corrupt or doesn't exist. Please visit == GITHUB LINK == for potential Fixes");
                ReturnedDataBase = new Dictionary<string, RecipeDef>();
                return ReturnedDataBase;
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
            Ingredients.Clear();
            Amounts.Clear();
            IngredientListbox.Items.Clear();
            AmountListbox.Items.Clear();
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
            if (!string.IsNullOrWhiteSpace(tb_InputFoodName.Text))
                NewFoodItem.Name = tb_InputFoodName.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(tb_CalorieInput.Text))
                NewFoodItem.Calories = int.Parse(tb_CalorieInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_ProteinInput.Text))
                NewFoodItem.Protein = int.Parse(tb_ProteinInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_FatInput.Text))
                NewFoodItem.Fat = int.Parse(tb_FatInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_CarbInput.Text))
                NewFoodItem.Carbs = int.Parse(tb_CarbInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_SugarInput.Text))
                NewFoodItem.Sugars = int.Parse(tb_SugarInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_SodiumInput.Text))
                NewFoodItem.Sodium = int.Parse(tb_SodiumInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_UOMInput.Text))
                NewFoodItem.UOM = tb_UOMInput.Text;

            if (!FoodList.ContainsKey(NewFoodItem.Name.ToLower()))
                FoodList.Add(NewFoodItem.Name.ToLower(), NewFoodItem);

            SaveFoodList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RecipeDef NewRecipeDef = new RecipeDef();

            if (Amounts != null && Amounts.Count > 0 && Amounts.Count == Ingredients.Count)
                NewRecipeDef.Amounts = Amounts;

            if (Ingredients != null && Ingredients.Count > 0 && Amounts.Count == Ingredients.Count)
                NewRecipeDef.Ingredients = Ingredients;

            if (!string.IsNullOrWhiteSpace(tb_RecipeNameInput.Text))
                NewRecipeDef.Name = tb_RecipeNameInput.Text.ToLower();


            if (!string.IsNullOrWhiteSpace(tb_ImageNameInput.Text))
                NewRecipeDef.ImageFile = tb_ImageNameInput.Text + ".jpg";

            if (!string.IsNullOrWhiteSpace(tb_DescriptionInput.Text))
                NewRecipeDef.Directions = tb_DescriptionInput.Text;

            if (int.TryParse(tb_RecipeCalorieInput.Text, out int n) && !string.IsNullOrWhiteSpace(tb_RecipeCalorieInput.Text))
                NewRecipeDef.Calories = int.Parse(tb_RecipeCalorieInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_TimeToCook.Text))
                NewRecipeDef.TimeToCook = tb_TimeToCook.Text;

            if (!RecipeList.ContainsKey(NewRecipeDef.Name.ToLower()))
            {
                RecipeList.Add(NewRecipeDef.Name.ToLower(), NewRecipeDef);
                L_WarningLabel_2.Text = "Recipe succesfully added!";
            }
            else
                L_WarningLabel_2.Text = "Recipe already exists in the recipe list";


            foreach (CheckBox element in SettingOptions)
            {
                if (element.IsChecked == true)
                {
                    if (!Preferences.Contains(CompatabilityMap[SettingOptions.IndexOf(element)]))
                    {
                       Preferences.Add(CompatabilityMap[SettingOptions.IndexOf(element)]);
                    }
                }              
            }

            if (Preferences != null)
            {
                NewRecipeDef.UserTags = Preferences;
            }
            SaveRecipeList();
            tb_TimeToCook.Clear();
            tb_RecipeNameInput.Clear();
            tb_RecipeCalorieInput.Clear();

        }

        private void Btn_AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_IngredientInput.Text))
            {
                string TagToAdd = tb_IngredientInput.Text;
                IngredientListbox.Items.Add(TagToAdd);
                Ingredients.Add(TagToAdd.ToLower());
            }
            else
            {
                L_WarningLabel_2.Text = "You can't cook with an ingredient you don't know!";
            }

            if (!string.IsNullOrWhiteSpace(tb_AmountInput.Text))
            {
                if (int.TryParse(tb_AmountInput.Text, out int n))
                {
                    string AmountToAdd = tb_AmountInput.Text;
                    AmountListbox.Items.Add(AmountToAdd);
                    Amounts.Add(float.Parse(AmountToAdd));
                }
                else
                {
                    L_WarningLabel_2.Text = "The amount must have a numerical value";
                }
            }
            else
            {
                L_WarningLabel_2.Text = "You need amounts for ingredients!";
            }
            tb_AmountInput.Clear();
            tb_IngredientInput.Clear();

        }
    }
}
