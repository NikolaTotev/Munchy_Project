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
        static string ProgramFolder = System.IO.Path.Combine(LocalAppDataPath, "Munchy");

        string FoodData = System.IO.Path.Combine(ProgramFolder, "FoodData.json");
        string RecipeDatabase = System.IO.Path.Combine(ProgramFolder, "Recipes.json");

        List<CheckBox> SettingOptions;
        List<RadioButton> Units;

        Dictionary<string, FoodDef> FoodList;
        Dictionary<string, RecipeDef> RecipeList;

        List<string> USIngredients = new List<string>();
        List<string> BGIngredients = new List<string>();
        List<float> Amounts = new List<float>();

        List<string> UnitsToAdd = new List<string>();

        List<float> SuggestedAmounts = new List<float>();

        List<string> Preferences = new List<string>();

        // === DO NOT EDIT OR CHANGE ===
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
            Units = new List<RadioButton> {RB_Cup, RB_Tbsp, RB_Tsp, RB_count, RB_Ml, RB_grams};
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

        //Handles saving the food list.
        public void SaveFoodList()
        {
            using (StreamWriter file = File.CreateText(FoodData))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, FoodList);
            }
        }

        //Handles saving the recipe list.
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

        //Opens recipe tool.
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

        //Opens foodtool.
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

        //Handles saving the newly created FooDef
        private void Btn_SaveFoodItem_Click(object sender, RoutedEventArgs e)
        {
            FoodDef NewFoodItem = new FoodDef();
            if (!string.IsNullOrWhiteSpace(US_FoodName.Text))
            {
                NewFoodItem.USName = US_FoodName.Text.ToLower();
            }

            if (!string.IsNullOrWhiteSpace(BG_FoodName.Text))
            {
                NewFoodItem.BGName = BG_FoodName.Text.ToLower();
            }

            if (float.TryParse(tb_CalorieInput.Text, out float parsedValue))
            {
                NewFoodItem.Calories = float.Parse(tb_CalorieInput.Text);
            }

            if (float.TryParse(tb_ProteinInput.Text, out float parsedValue1))
            {
                NewFoodItem.Protein = float.Parse(tb_ProteinInput.Text);
            }

            if (float.TryParse(tb_FatInput.Text, out float parsedValue2))
            {
                NewFoodItem.Fat = float.Parse(tb_FatInput.Text);
            }

            if (float.TryParse(tb_CarbInput.Text, out float parsedValue3))
            {
                NewFoodItem.Carbs = float.Parse(tb_CarbInput.Text);
            }


            if (float.TryParse(tb_SugarInput.Text, out float parsedValue4))
            {
                NewFoodItem.Sugars = float.Parse(tb_SugarInput.Text);
            }

            if (float.TryParse(tb_SodiumInput.Text, out float parsedValue5))
            {
                NewFoodItem.Sodium = float.Parse(tb_SodiumInput.Text);
            }

            if (float.TryParse(Tb_DesityInput.Text, out float parsedValue6))
            {
                NewFoodItem.FoodDensity = float.Parse(Tb_DesityInput.Text);
            }

            if (!string.IsNullOrWhiteSpace(US_UOMInput.Text))
            {
                NewFoodItem.USUOM = US_UOMInput.Text;
            }


            if (!string.IsNullOrWhiteSpace(BG_UOMInput.Text))
            {
                NewFoodItem.BGUOM = BG_UOMInput.Text;
            }

            if (SuggestedAmounts != null && SuggestedAmounts.Count > 0)
            {
                NewFoodItem.SuggestedAmounts = SuggestedAmounts;
            }

            if (NewFoodItem.USName != null)
            {
                FoodList.Add(NewFoodItem.USName.ToLower(), NewFoodItem);
            }

            SaveFoodList();
            FoodList = GetFoodList();

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
            SuggestedAmounts = new List<float>();
            BG_FoodName.Clear();
            Lb_SuggestedAmounts.Items.Clear();
        }

        //Handles adding suggested food amounts for creating FoodDefs.
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

        //Handles saving the newly created Recipe
        private void BTN_SaveRecipe(object sender, RoutedEventArgs e)
        {
            RecipeDef NewRecipeDef = new RecipeDef();
            NewRecipeDef.TimeTags = new List<string>();
            NewRecipeDef.UserTags = new List<string>();

            if (!string.IsNullOrWhiteSpace(US_NameInput.Text) && !string.IsNullOrWhiteSpace(BG_NameInput.Text))
            {
                NewRecipeDef.USName = US_NameInput.Text.ToString();
                NewRecipeDef.BGName = BG_NameInput.Text.ToString();
            }

            if (!string.IsNullOrWhiteSpace(US_Directions.Text) && !string.IsNullOrWhiteSpace(BG_Directions.Text))
            {
                NewRecipeDef.USDirections = US_Directions.Text.ToString();
                NewRecipeDef.BGDirections = BG_Directions.Text.ToString();
            }

            if (!string.IsNullOrWhiteSpace(TB_ImageNameInput.Text))
            {
                NewRecipeDef.ImageFile = TB_ImageNameInput.Text.ToString() + ".jpg";
            }

            if (CB_Breakfast.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add(RecipeManager.BreakfastTag);
            }

            if (CB_Lunch.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add(RecipeManager.LunchTag);
            }

            if (CB_Dinner.IsChecked == true)
            {
                NewRecipeDef.TimeTags.Add(RecipeManager.DinnerTag);
            }

            if (int.TryParse(TB_RecipeCalorieInput.Text, out int parsedValue1))
            {
                NewRecipeDef.Calories = int.Parse(TB_RecipeCalorieInput.Text);
            }


            if (!string.IsNullOrWhiteSpace(TB_TimeToCook.Text))
            {
                NewRecipeDef.TimeToCook = TB_TimeToCook.Text.ToString();
            }

            foreach (CheckBox element in SettingOptions)
            {
                if (element.IsChecked == true)
                {
                    NewRecipeDef.UserTags.Add(CompatabilityMap[SettingOptions.IndexOf(element)]);
                }
            }

            if (USIngredients.Count > 0 && BGIngredients.Count > 0 && Amounts.Count > 0 && UnitsToAdd.Count == Amounts.Count && Amounts.Count == USIngredients.Count && Amounts.Count == BGIngredients.Count)
            {
                NewRecipeDef.USIngredients = USIngredients;
                NewRecipeDef.BGIngredients = BGIngredients;
                NewRecipeDef.Units = UnitsToAdd;
                NewRecipeDef.Amounts = Amounts;
            }

            if (NewRecipeDef.USIngredients.Count == NewRecipeDef.Amounts.Count && NewRecipeDef.BGIngredients.Count == NewRecipeDef.Amounts.Count && NewRecipeDef.Amounts.Count == NewRecipeDef.Units.Count)
            {
                if (!RecipeList.ContainsKey(NewRecipeDef.USName.ToLower()))
                {
                    RecipeList.Add(NewRecipeDef.USName.ToLower(), NewRecipeDef);
                    SaveRecipeList();
                    RecipeList = GetRecipies();

                    if (RecipeList.ContainsKey(NewRecipeDef.USName.ToLower()))
                    {
                        NewRecipeDef = new RecipeDef();

                        USIngredients = new List<string>();
                        BGIngredients = new List<string>();
                        Amounts = new List<float>();
                        UnitsToAdd = new List<string>();
                        US_FoodName.Clear();
                        BG_FoodName.Clear();

                        foreach (CheckBox element in SettingOptions)
                        {
                            element.IsChecked = false;
                        }

                        foreach (CheckBox element in SettingOptions)
                        {
                            element.IsChecked = false;
                        }

                        US_IngredientList.Items.Clear();
                        BG_IngredientList.Items.Clear();
                        AmountListbox.Items.Clear();
                        TB_ImageNameInput.Clear();
                        TB_RecipeCalorieInput.Clear();
                        TB_TimeToCook.Clear();
                    }
                }
            }
        }

        //Handles adding ingredients, amounts and units to the corresponding lists.
        private void Btn_AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            bool UnitSelected = false;

            foreach (RadioButton element in Units)
            {
                if (element.IsChecked == true)
                {
                    UnitSelected = true;
                    break;
                }
                else
                {
                    UnitSelected = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(US_IngredientInput.Text) && !string.IsNullOrWhiteSpace(BG_IngredientInput.Text) && !string.IsNullOrWhiteSpace(TB_AmountInput.Text) && UnitSelected == true)
            {
                if (!USIngredients.Contains(US_IngredientInput.ToString().ToLower()) && !BGIngredients.Contains(BG_IngredientInput.ToString().ToLower()))
                {
                    USIngredients.Add(US_IngredientInput.Text.ToString().ToLower());
                    BGIngredients.Add(BG_IngredientInput.Text.ToString().ToLower());
                    Amounts.Add(float.Parse(TB_AmountInput.Text));


                    US_IngredientList.Items.Add(US_IngredientInput.Text.ToLower());
                    BG_IngredientList.Items.Add(BG_IngredientInput.Text.ToLower());
                    AmountListbox.Items.Add(float.Parse(TB_AmountInput.Text));

                    foreach (RadioButton element in Units)
                    {
                        if (element.IsChecked == true)
                        {
                            UnitsToAdd.Add(element.Content.ToString());
                        }
                    }

                    US_IngredientInput.Clear();
                    BG_IngredientInput.Clear();
                    Lb_SuggestFoodsToAdd.Visibility = Visibility.Hidden;
                    TB_AmountInput.Clear();
                }
            }

        }

        //Functions called when textbox test is changed.
        private void USIngrTextChanged(object sender, TextChangedEventArgs e)
        {
            Lb_SuggestFoodsToAdd.Visibility = Visibility.Visible;
            SearchForFoodsToAdd();
        }

        //Handles searching for food items to suggest.
        private void SearchForFoodsToAdd()
        {
            // Checks if the search box is null or not.
            if (!string.IsNullOrWhiteSpace(US_IngredientInput.Text))
            {
                string searchedWord = US_IngredientInput.Text;
                string ToLower = searchedWord.ToLower();

                foreach (KeyValuePair<string, FoodDef> element in FoodList)
                {
                    if (element.Key.StartsWith(ToLower) && !Lb_SuggestFoodsToAdd.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString()))
                    {
                        Lb_SuggestFoodsToAdd.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString());
                    }
                }
            }
            else
            {
                Lb_SuggestFoodsToAdd.Items.Clear();
            }
        }

        //Adds names of clicked name of food item in the suggested food items to add list.d
        private void Lb_SuggestedFoodsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lb_SuggestFoodsToAdd.SelectedItem != null)
            {
                string foodItemToAdd = Lb_SuggestFoodsToAdd.SelectedItem.ToString().ToLower();
                FoodDef foodToAdd = FoodList[foodItemToAdd];
                US_IngredientInput.Text = Lb_SuggestFoodsToAdd.SelectedItem.ToString().ToLower();
                BG_IngredientInput.Text = foodToAdd.BGName.ToLower();
            }
        }

        //Removes listbox when the amount input gets focus.
        private void TBAmountInputGotFocus(object sender, RoutedEventArgs e)
        {
            Lb_SuggestFoodsToAdd.Visibility = Visibility.Hidden;
        }
    }
}
