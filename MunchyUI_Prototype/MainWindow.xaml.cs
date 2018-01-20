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
namespace MunchyUI_Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Save File Locations
        string UserFile = @"  C:\Users\Nikola\AppData\Roaming\MunchyUSER5.json";
        string DefaultUserFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\DEFAULT_USER.json";

        string UserFridgeFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\USER_F.json";
        string DefaultFridgeFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\DEFAULT_FRIDGE.json";

        string FoodDefFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\FoodData.json";
        string RecipeDatabase = @"  C:\Users\Nikola\AppData\Roaming\Munchy\Recipes.json";

        string RecipeSaveFile = @"  C:\Users\Nikola\AppData\Roaming\Munchy\RecipeSavesFile.json";
        string DefaultSaver = @"  C:\Users\Nikola\AppData\Roaming\Munchy\DefaultSaverFile.json";


        //Variables for the summary of the fridge.
        int CalorieSum = 0;
        int ProteinSum = 0;
        int FatSum = 0;
        int CarbSum = 0;
        int SugarSum = 0;
        int SodiumSum = 0;

        // A list of checkboxes that are used for saving the users settings and preferences
        List<CheckBox> SettingOptions;
        List<FoodDef> RecipeIngredientList;

        TextBlock[] SummaryTextBlocks;
        int[] SummaryValues;

        // This list is used for populating the Ingredients ListView in the UI.

        RecipeDef SuggestedRecipe = new RecipeDef();
        ProgramManager CurrentManager;

        // RecipeImage handles the recipe image on the full recipe view aswell as the recipe view on the main menu.
        ImageBrush RecipeImage = new ImageBrush();

        // ================= UI LOGIC =================
        #region UI Logic

        #region Initialization functions

        public MainWindow()
        {
            InitializeComponent();
            SummaryTextBlocks = new TextBlock[] { tB_CalorieSummary, tB_ProteinSummary, tB_FatSummary, tB_CarbsSummary, tB_SugarSumary, tB_SodiumSummary };
            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, DefaultSaver);
            SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };
            RecipeIngredientList = new List<FoodDef>();
            InitialFridgeUISetup();
            PopulateFridgeSummary();
            SuggestRecipe();
        }

        // Handles initial Setup of the fridge UI. Called only on program start.
        private void InitialFridgeUISetup()
        {
            if (CurrentManager.User.UserFridge.UsersFoods.Count > 0)
            {
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.UsersFoods)
                {
                    lb_FoodList.Items.Add(element.Key);

                    if (lb_Fridge.Items.Count < 10)
                        lb_Fridge.Items.Add(element.Key);
                }
            }
        }

        #endregion


        #region Recipe related functions

        // Opens or closes the full recipe view.
        private void ShowOrCloseFullRecipeView()
        {
            if (p_FullRecipeView.Visibility == Visibility.Hidden)
            {
                RecipeImage.ImageSource = new BitmapImage(new Uri(SuggestedRecipe.ImageFile, UriKind.Relative));
                img_RecipeImage.Fill = RecipeImage;
                p_FullRecipeView.Visibility = Visibility.Visible;
            }
            else
            {
                p_FullRecipeView.Visibility = Visibility.Hidden;
            }
        }

        // Adds recipe information to the full recipe view. 
        // Function is called when the recipe view is opened, or when a new recipe is suggested.
        private void AddRecipeIngredientsToListView()
        {
            foreach (string ingredient in SuggestedRecipe.Ingredients)
            {
                RecipeIngredientList.Add(new FoodDef() { Name = CurrentManager.FoodManag.Foods[ingredient].Name, Amount = SuggestedRecipe.Amounts[SuggestedRecipe.Ingredients.IndexOf(ingredient)] });
            }

            lv_Ingredients.ItemsSource = RecipeIngredientList;
            tB_Directions.Text = SuggestedRecipe.Directions.ToString();
            tB_RecipeTitle.Text = SuggestedRecipe.Name.ToString();
            tB_TimeToCook.Text = SuggestedRecipe.TimeToCook.ToString();
        }


        // Suggests a recipe based on the time of day. Recipe sorting is handled in the back end.
        private void SuggestRecipe()
        {
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 11)
            {
                if (CurrentManager.RecipieManag.Breakfast.Count > 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Breakfast[0]];

                }
            }

            if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17)
            {
                if (CurrentManager.RecipieManag.Lunch.Count > 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Lunch[0]];

                }
            }

            if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 24)
            {
                if (CurrentManager.RecipieManag.Dinner.Count > 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Dinner[0]];
                }
            }
            tB_RecipeName.Text = SuggestedRecipe.Name;
            AddRecipeIngredientsToListView();
        }

        #endregion


        #region Fridge related functions

        #region UI Show/Hide functions
        private void ShowOrCloseFridge()
        {
            if (p_UserFoods.Visibility == Visibility.Hidden)
            {
                p_UserFoods.Visibility = Visibility.Visible;
            }
            else
            {
                p_UserFoods.Visibility = Visibility.Hidden;
            }
        }

        // Function that handles opening and closing the search/add panel.
        private void OpenCloseFoodSearch()
        {
            if (p_AddFoodItemPanel.Visibility == Visibility.Hidden)
                p_AddFoodItemPanel.Visibility = Visibility.Visible;
            else
                p_AddFoodItemPanel.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Adding/Removing food items

        // Called when the text in the textbox for searching for foods is changed
        private void FoodSearchTextChanged()
        {
            // Checks if the search box is null or not.
            if (!string.IsNullOrWhiteSpace(tb_Search.Text))
            {
                //Makes sure that text is not the keyword "Search"
                if (tb_Search.Text != "Search")
                {
                    l_SearchInfo.Text = "Click on an item to add it";
                    string searchedWord = tb_Search.Text;
                    string ToLower = searchedWord.ToLower();

                    foreach (KeyValuePair<string, FoodDef> element in CurrentManager.FoodManag.Foods)
                    {
                        if (element.Key.StartsWith(ToLower.Substring(0)) && !lB_SuggestedFoods.Items.Contains(element.Key))
                        {
                            lB_SuggestedFoods.Items.Add(element.Key);
                        }
                    }
                }
            }
            else
            {
                lB_SuggestedFoods.Items.Clear();
                l_SearchInfo.Text = "Type to search for an item";
            }
        }

        // Function is called once the user clicks on an item in the listbox of suggested foods. The item is added into the user fridge UI
        // the FoodDef is added into the user's fridge and the users fridge is saved.
        private void AddClickedFoodItem()
        {
            if (lB_SuggestedFoods.SelectedItem != null)
            {
                if (!CurrentManager.User.UserFridge.UsersFoods.ContainsKey((lB_SuggestedFoods.SelectedItem.ToString())))
                {
                    CurrentManager.User.UserFridge.AddToFridge((CurrentManager.FoodManag.Foods[lB_SuggestedFoods.SelectedItem.ToString()]));
                    lb_FoodList.Items.Add((CurrentManager.FoodManag.Foods[lB_SuggestedFoods.SelectedItem.ToString()].Name));
                    CurrentManager.UsersFridge.SaveFridge();
                    l_SearchInfo.Text = "Item added!";
                }
                else
                {
                    l_SearchInfo.Text = "Don't worry! You already have this.";
                }
                PopulateFridgeSummary();
            }
        }

        private void RemoveItem()
        {
            if (lb_FoodList.SelectedItem != null)
            {
                CurrentManager.User.UserFridge.RemoveFromFridge(lb_FoodList.SelectedItem.ToString());
                lb_FoodList.Items.Remove(lb_FoodList.SelectedIndex);
                lb_FoodList.Items.Clear();
                CurrentManager.User.UserFridge.SaveFridge();

                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.UsersFoods)
                {
                    lb_FoodList.Items.Add(element.Key);
                }
                PopulateFridgeSummary();
            }
        }
        #endregion

        #region Fridge infromation management
        // Adds all the elements in the users fridge to the listbox in the UI. Function is called on program start.
        private void PopulateFridgeSummary()
        {
            CalorieSum = 0;
            ProteinSum = 0;
            FatSum = 0;
            CarbSum = 0;
            SugarSum = 0;
            SodiumSum = 0;
            if (CurrentManager.User.UserFridge.UsersFoods.Count > 0)
            {
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.UsersFoods)
                {
                    CalorieSum += element.Value.Calories;
                    ProteinSum += element.Value.Protein;
                    FatSum += element.Value.Fat;
                    CarbSum += element.Value.Carbs;
                    SugarSum += element.Value.Sugars;
                    SodiumSum += element.Value.Sodium;
                }
                SummaryValues = new int[] { CalorieSum, ProteinSum, FatSum, CarbSum, SugarSum, SodiumSum };
                for (int i = 0; i < SummaryTextBlocks.Length; i++)
                {
                    SummaryTextBlocks[i].Text = SummaryValues[i].ToString();
                }
            }
            else
            {
                SummaryValues = new int[] { CalorieSum, ProteinSum, FatSum, CarbSum, SugarSum, SodiumSum };
                for (int i = 0; i < SummaryTextBlocks.Length; i++)
                {
                    SummaryTextBlocks[i].Text = SummaryValues[i].ToString();
                }
            }
        }

        // When a food item is selected in the listbox. The panel on the right shows the foods nutritional information.
        private void GetAndShowFoodInfo()
        {
            if (lb_FoodList.SelectedItem != null)
            {
                FoodDef SelectedItem = CurrentManager.FoodManag.Foods[lb_FoodList.SelectedItem.ToString()];
                tb_FoodName.Text = SelectedItem.Name.First().ToString().ToUpper() + SelectedItem.Name.Substring(1);
                tb_FoodItemCalorie.Text = SelectedItem.Calories.ToString();
                tB_FoodProtein.Text = SelectedItem.Protein.ToString();
                tB_FoodFat.Text = SelectedItem.Fat.ToString();
                tB_FoodCarbs.Text = SelectedItem.Carbs.ToString();
                tB_FoodSugar.Text = SelectedItem.Sugars.ToString();
                tB_FoodSodium.Text = SelectedItem.Sodium.ToString();
            }
            else
            {
                tb_FoodItemCalorie.Text = "0";
                tB_FoodProtein.Text = "0";
                tB_FoodFat.Text = "0";
                tB_FoodCarbs.Text = "0";
                tB_FoodSugar.Text = "0";
                tB_FoodSodium.Text = "0";
            }
        }
        #endregion

        #endregion

        #region User settings functions

        private void ShowSettingsPanel()
        {
            tb_NameInput.Text = CurrentManager.User.UserName;
            tb_AgeInput.Text = CurrentManager.User.Age.ToString();
            tb_WeightInput.Text = CurrentManager.User.Weight.ToString();

            foreach (CheckBox element in SettingOptions)
            {
                if (CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]))
                {
                    element.IsChecked = true;
                }
                else
                {
                    element.IsChecked = false;
                }
            }

            if (p_Settings.Visibility == Visibility.Hidden)
            {
                p_Settings.Visibility = Visibility.Visible;
            }
            else
            {
                p_Settings.Visibility = Visibility.Hidden;
            }
        }

        private void SaveUserSettings()
        {
            int parsedValue;
            if (!string.IsNullOrWhiteSpace(tb_NameInput.Text) && !string.IsNullOrWhiteSpace(tb_AgeInput.Text) && !string.IsNullOrWhiteSpace(tb_WeightInput.Text))
            {
                CurrentManager.User.UserName = tb_NameInput.Text;

                if (int.TryParse(tb_AgeInput.Text, out parsedValue))
                    CurrentManager.User.Age = int.Parse(tb_AgeInput.Text);
                else MessageBox.Show("Age is a number value!");

                if (int.TryParse(tb_WeightInput.Text, out parsedValue))
                    CurrentManager.User.Weight = int.Parse(tb_WeightInput.Text);
                else MessageBox.Show("Weight is a number value!");
            }

            if (rb_Male.IsChecked == true)
            {
                CurrentManager.User.Sex = "male";
            }

            if (rb_Female.IsChecked == true)
            {
                CurrentManager.User.Sex = "female";
            }

            if (rb_Other.IsChecked == true)
            {
                CurrentManager.User.Sex = "other";
            }

            foreach (CheckBox element in SettingOptions)
            {
                if (element.IsChecked == true)
                {
                    if (!CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]))
                    {
                        CurrentManager.User.Preferences.Add(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]);
                    }
                }

                if (element.IsChecked == false)
                {
                    if (CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]))
                    {
                        CurrentManager.User.Preferences.Remove(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]);
                    }
                }
            }

            CurrentManager.SaveUser();
            CurrentManager.User.CalculateIndex();
            CurrentManager.SaveUser();
            tB_UserName.Text = CurrentManager.User.UserName;
        }
        #endregion

        #endregion


        // ================= UI EVENTS =================
        #region UI Event functions

        #region Panel Show/Close events

        /// <summary>
        /// Shows/ Hides full recipe view and populates the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ShowRecipe_Click(object sender, RoutedEventArgs e)
        {
            ShowOrCloseFullRecipeView();
        }


        /// <summary>
        /// Opens full fridge view for the fridge. Function called by the "Show Fridge" button and the close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Showfridge_Click(object sender, RoutedEventArgs e)
        {
            ShowOrCloseFridge();
        }


        /// <summary>
        /// Opens user settings panel and populates the current active settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSettings(object sender, MouseButtonEventArgs e)
        {
            ShowSettingsPanel();
        }

        /// <summary>
        /// Button for opening the food search/add panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFoodSearch(object sender, RoutedEventArgs e)
        {
            OpenCloseFoodSearch();
        }

        /// <summary>
        /// Button for closing the food search/add panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CloseClick(object sender, RoutedEventArgs e)
        {
            OpenCloseFoodSearch();
        }
        #endregion

        #region Button Events

        #region Fridge related
        private void ShowFoodInfo(object sender, SelectionChangedEventArgs e)
        {
            GetAndShowFoodInfo();
        }

        /// <summary>
        /// Removes an item from the fridge UI and the user fridge.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem();
        }

        #endregion

        #region Recipe related
        /// <summary>
        /// Calls SuggestRecipe function on button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SuggestRecipe_Click(object sender, RoutedEventArgs e)
        {
            SuggestRecipe();
        }

        /// <summary>
        /// Shows all recipes saved by the user. Function is called on button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SeeAllSavedRecipes_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region User related
        /// <summary>
        /// Saves users settings based on the input from the settings panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveUserSettings();
        }
        #endregion

        #endregion

        #region Textbox and listbox events

        /// <summary>
        /// Once the textbox has lost focus the defualt text "Search" appears;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoodSearchLostFocus(object sender, RoutedEventArgs e)
        {
            tb_Search.Text = "Search";
        }

        /// <summary>
        /// Clears the textbox once the user has focused it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchFoodClearTextBox(object sender, RoutedEventArgs e)
        {
            tb_Search.Text = null;
        }

        private void AddClickedItem(object sender, SelectionChangedEventArgs e)
        {
            AddClickedFoodItem();
        }

        /// <summary>
        /// When the user types in the search box (or changes the text) the listbox for suggested foods is filled with the Foods in the 
        /// FoodData base that start with the given substring.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            FoodSearchTextChanged();
        }
        #endregion

        #endregion
    }


}
