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
        List<RadioButton> Units;

        Dictionary<string, FoodDef> FoodList;
        Dictionary<string, RecipeDef> RecipeList;

        List<string> USIngredients = new List<string>();
        List<string> BGIngredients = new List<string>();
        List<string> UnitsToAdd = new List<string>();

        List<float> SuggestedAmounts = new List<float>();

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
            Units = new List<RadioButton> { RB_Cup, RB_Tbsp, RB_Tsp };
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
            USIngredients.Clear();
            Amounts.Clear();
            BG_IngredientList.Items.Clear();
            US_IngredientList.Items.Clear();
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
            if (!string.IsNullOrWhiteSpace(US_FoodName.Text))
                NewFoodItem.USName = US_FoodName.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(BG_FoodName.Text))
                NewFoodItem.BGName = BG_FoodName.Text.ToLower();

            if (float.TryParse(tb_CalorieInput.Text, out float parsedValue))
            {
                if (!string.IsNullOrWhiteSpace(tb_CalorieInput.Text))
                    NewFoodItem.Calories = float.Parse(tb_CalorieInput.Text);
            }

            if (float.TryParse(tb_ProteinInput.Text, out float parsedValue1))
            {
                if (!string.IsNullOrWhiteSpace(tb_ProteinInput.Text))
                    NewFoodItem.Protein = float.Parse(tb_ProteinInput.Text);
            }

            if (float.TryParse(tb_FatInput.Text, out float parsedValue2))
            {
                if (!string.IsNullOrWhiteSpace(tb_FatInput.Text))
                    NewFoodItem.Fat = float.Parse(tb_FatInput.Text);
            }

            if (float.TryParse(tb_CarbInput.Text, out float parsedValue3))
            {
                if (!string.IsNullOrWhiteSpace(tb_CarbInput.Text))
                    NewFoodItem.Carbs = float.Parse(tb_CarbInput.Text);
            }


            if (float.TryParse(tb_SugarInput.Text, out float parsedValue4))
            {
                if (!string.IsNullOrWhiteSpace(tb_SugarInput.Text))
                    NewFoodItem.Sugars = float.Parse(tb_SugarInput.Text);
            }

            if (float.TryParse(tb_SodiumInput.Text, out float parsedValue5))
            {
                if (!string.IsNullOrWhiteSpace(tb_SodiumInput.Text))
                    NewFoodItem.Sodium = float.Parse(tb_SodiumInput.Text);
            }

            if (!string.IsNullOrWhiteSpace(US_UOMInput.Text))
                NewFoodItem.USUOM = US_UOMInput.Text;

            if (!string.IsNullOrWhiteSpace(BG_UOMInput.Text))
                NewFoodItem.BGUOM = BG_UOMInput.Text;

            if (SuggestedAmounts != null && SuggestedAmounts.Count > 0)
                NewFoodItem.SuggestedAmounts = SuggestedAmounts;


            if (NewFoodItem.USName != null)
            {
                if (!FoodList.ContainsKey(NewFoodItem.USName.ToLower()))
                    FoodList.Add(NewFoodItem.USName.ToLower(), NewFoodItem);
            }
            SaveFoodList();

            BG_UOMInput.Clear();
            US_UOMInput.Clear();
            tb_CalorieInput.Clear();
            tb_CarbInput.Clear();
            tb_FatInput.Clear();
            tb_ProteinInput.Clear();
            tb_SodiumInput.Clear();
            tb_SugarInput.Clear();
            US_FoodName.Clear();
            Lb_SuggestedAmounts.Items.Clear();
            SuggestedAmounts.Clear();
            BG_FoodName.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RecipeDef NewRecipeDef = new RecipeDef();
            NewRecipeDef.TimeTags = new List<string>();
            NewRecipeDef.Units = new List<string>();

            if (Amounts != null && Amounts.Count > 0 && Amounts.Count == USIngredients.Count)
                NewRecipeDef.Amounts = Amounts;

            if (USIngredients != null && USIngredients.Count > 0 && Amounts.Count == USIngredients.Count)
                NewRecipeDef.USIngredients = USIngredients;

            if (BGIngredients != null && BGIngredients.Count > 0 && Amounts.Count == BGIngredients.Count)
                NewRecipeDef.BGIngredients = BGIngredients;

            if (!string.IsNullOrWhiteSpace(BG_NameInput.Text))
                NewRecipeDef.BGName = BG_NameInput.Text.ToLower();

            if (!string.IsNullOrWhiteSpace(US_NameInput.Text))
                NewRecipeDef.USName = US_NameInput.Text.ToLower();


            if (!string.IsNullOrWhiteSpace(tb_ImageNameInput.Text))
                NewRecipeDef.ImageFile = tb_ImageNameInput.Text + ".jpg";

            if (!string.IsNullOrWhiteSpace(US_Directions.Text))
                NewRecipeDef.USDirections = US_Directions.Text;

            if (!string.IsNullOrWhiteSpace(BG_Directions.Text))
                NewRecipeDef.BGDirections = BG_Directions.Text;

            if (int.TryParse(tb_RecipeCalorieInput.Text, out int n) && !string.IsNullOrWhiteSpace(tb_RecipeCalorieInput.Text))
                NewRecipeDef.Calories = int.Parse(tb_RecipeCalorieInput.Text);

            if (!string.IsNullOrWhiteSpace(tb_TimeToCook.Text))
                NewRecipeDef.TimeToCook = tb_TimeToCook.Text;

            

            if (UnitsToAdd.Count > 0)
                NewRecipeDef.Units = UnitsToAdd;

            foreach (CheckBox element in SettingOptions)
            {
                if (element.IsChecked == true)
                {
                    if (!Preferences.Contains(CompatabilityMap[SettingOptions.IndexOf(element)]))
                    {
                        Preferences.Add(CompatabilityMap[SettingOptions.IndexOf(element)]);
                    }
                    element.IsChecked = false;
                }
            }

            if (CB_Breakfast.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add("breakfast");
            }

            if (CB_Lunch.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add("lunch");
            }

            if (CB_Dinner.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add("dinner");
            }

            if (Preferences != null)
            {
                NewRecipeDef.UserTags = Preferences;
            }

            if (NewRecipeDef.USName != null)
            {
                if (!RecipeList.ContainsKey(NewRecipeDef.USName.ToLower()))
                {
                    RecipeList.Add(NewRecipeDef.USName.ToLower(), NewRecipeDef);
                    L_WarningLabel_2.Text = "Recipe succesfully added!";
                }
                else
                    L_WarningLabel_2.Text = "Recipe already exists in the recipe list";

            }

            SaveRecipeList();
            tb_TimeToCook.Clear();
            US_NameInput.Clear();
            BG_NameInput.Clear();
            tb_RecipeCalorieInput.Clear();

        }

        private void Btn_AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton element in Units)
            {
                if (element.IsChecked == true)
                {
                    UnitsToAdd.Add(element.Content.ToString());
                }
                element.IsChecked = false;
            }

            if (!string.IsNullOrWhiteSpace(US_IngredientInput.Text) && !string.IsNullOrWhiteSpace(BG_IngredientInput.Text))
            {
                string USIngrToAdd = US_IngredientInput.Text;
                string BGIngrToAdd = BG_IngredientInput.Text;
                US_IngredientList.Items.Add(USIngrToAdd);
                BG_IngredientList.Items.Add(BGIngrToAdd);
                USIngredients.Add(USIngrToAdd.ToLower());
                BGIngredients.Add(BGIngrToAdd.ToLower());
            }
            else
            {
                L_WarningLabel_2.Text = "You can't cook with an ingredient you don't know!";
            }

            if (!string.IsNullOrWhiteSpace(tb_AmountInput.Text))
            {
                if (float.TryParse(tb_AmountInput.Text, out float n))
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
            US_IngredientInput.Clear();
            BG_IngredientInput.Clear();

        }

        private void BTN_AddSuggestedAmount(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SuggestAmountInput.Text, out int n) && SuggestedAmounts.Count < 4)
            {
                float AmountToAdd = float.Parse(SuggestAmountInput.Text);
                SuggestedAmounts.Add(AmountToAdd);
                Lb_SuggestedAmounts.Items.Add(AmountToAdd);
            }
            else
            {
                L_WarningLabel_2.Text = "The amount must have a numerical value";
            }
            SuggestAmountInput.Clear();
        }
    }
}
