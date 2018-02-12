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
using System.IO;
namespace MunchyUI_Prototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string ProgramFolder = LocalAppDataPath + "\\Munchy";
        static string ImageFolderPath = ProgramFolder + "\\Images\\";

        //Save File Locations
        string UserFile = ProgramFolder + "\\USER5.json";
        string DefaultUserFile = ProgramFolder + "\\DEFAULT_USER.json";

        string UserFridgeFile = ProgramFolder + "\\USER_F.json";
        string DefaultFridgeFile = ProgramFolder + "\\DEFAULT_FRIDGE.json";

        string FoodDefFile = ProgramFolder + "\\FoodData.json";
        string RecipeDatabase = ProgramFolder + "\\Recipes.json";

        string RecipeSaveFile = ProgramFolder + "\\RecipeSavesFile.json";
        string StatSavePath = ProgramFolder + "\\StatSavePath.json";

        bool enUS;
        bool bgBG;

        //Variables for the summary of the fridge.
        int CalorieSum = 0;
        int ProteinSum = 0;
        int FatSum = 0;
        int CarbSum = 0;
        int SugarSum = 0;
        int SodiumSum = 0;

        int DailyCalories = 0;

        int NumerOfRecipeToSuggest = 0;
        // A list of checkboxes that are used for saving the users settings and preferences
        List<CheckBox> SettingOptions;

        // This list is used for populating the Ingredients ListView in the UI.
        List<FoodDef> RecipeIngredientList;

        TextBlock[] SummaryTextBlocks;
        int[] SummaryValues;

        RecipeDef SuggestedRecipe = new RecipeDef();
        ProgramManager CurrentManager;

        // RecipeImage handles the recipe image on the full recipe view aswell as the recipe view on the main menu.
        ImageBrush RecipeImage = new ImageBrush();
        ImageBrush SuggestedRecipeImage = new ImageBrush();


        ImageBrush RecentRecipe_1 = new ImageBrush();
        ImageBrush RecentRecipe_2 = new ImageBrush();
        ImageBrush RecentRecipe_3 = new ImageBrush();
        ImageBrush RecentRecipe_4 = new ImageBrush();
        ImageBrush RecentRecipe_5 = new ImageBrush();
        ImageBrush RecentRecipe_6 = new ImageBrush();

        ImageBrush[] RecentlyViewedRecipeImages;
        Ellipse[] RecentlyViewedRecipesImages;
        Ellipse[] FrontPageRecentlyViewedImages;

        // ================= UI LOGIC =================
        #region UI Logic

        #region Initialization functions

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists(FoodDefFile) || !File.Exists(RecipeDatabase))
            {
                MessageBox.Show("ERROR FAR404 : You are missing files required for the programs operation. Please vist == GITHUB URL == for potential fixes.");
                Close();
                return;
            }

            SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };

            if (!File.Exists(UserFile))
            {
                ShowSettingsPanel();
                MessageBox.Show("It seems like your use file is empty. Take a moment to fill in some of your details. This will help Munchy suggest recipes exactly to your tastes.");
            }

            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, StatSavePath);
            SummaryTextBlocks = new TextBlock[] { tB_CalorieSummary, tB_ProteinSummary, tB_FatSummary, tB_CarbsSummary, tB_SugarSumary, tB_SodiumSummary };
            RecentlyViewedRecipeImages = new ImageBrush[] { RecentRecipe_1, RecentRecipe_2, RecentRecipe_3, RecentRecipe_4, RecentRecipe_5, RecentRecipe_6 };
            RecentlyViewedRecipesImages = new Ellipse[] { Img_RecentlyViewed_1, Img_RecentlyViewed_2, Img_RecentlyViewed_3, Img_RecentlyViewed_4, Img_RecentlyViewed_5, Img_RecentlyViewed_6 };
            FrontPageRecentlyViewedImages = new Ellipse[] { Img_FrontRecentlyViewed_1, Img_FrontRecentlyViewed_2, Img_FrontRecentlyViewed_3, Img_FrontRecentlyViewed_4, Img_FrontRecentlyViewed_5 };
            RecipeIngredientList = new List<FoodDef>();
            InitialFridgeUISetup();
            PopulateFridgeSummary();
            SuggestRecipe();
            SetRecentlyViewedImages();

            CurrentManager.StatManager.DailyReset();
            DailyCalories = CurrentManager.StatManager.DailyCalories;
            L_DailyCalories.Text = DailyCalories.ToString();
            Localizer.SetDefaultLanguage(this);
        }

        // Handles initial Setup of the fridge UI. Called only on program start.
        private void InitialFridgeUISetup()
        {
            if (CurrentManager.User.UserFridge.UsersFoods != null && CurrentManager.User.UserFridge.UsersFoods.Count > 0)
            {
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.UsersFoods)
                {
                    lb_FoodList.Items.Add(element.Key);

                    if (lb_Fridge.Items.Count < 10)
                        lb_Fridge.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1));
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
                if (File.Exists(ImageFolderPath + SuggestedRecipe.ImageFile))
                {
                    RecipeImage.ImageSource = new BitmapImage(new Uri(ImageFolderPath + SuggestedRecipe.ImageFile, UriKind.Relative));
                    img_RecipeImage.Fill = RecipeImage;
                }
                else
                {
                    img_RecipeImage.Fill = null;
                }
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
            RecipeIngredientList.Clear();
            if (enUS == true)
            {
                if (SuggestedRecipe.USIngredients != null && SuggestedRecipe.USIngredients.Count > 0)
                {
                    foreach (string ingredient in SuggestedRecipe.USIngredients)
                    {
                        RecipeIngredientList.Add(new FoodDef() { USName = ingredient, Amount = SuggestedRecipe.Amounts[SuggestedRecipe.USIngredients.IndexOf(ingredient)] });
                    }

                    lv_Ingredients.ItemsSource = RecipeIngredientList;
                    lv_Ingredients.Items.Refresh();
                    tB_Directions.Text = SuggestedRecipe.USDirections.ToString();
                    tB_RecipeTitle.Text = SuggestedRecipe.USName.ToString();
                    tB_TimeToCook.Text = SuggestedRecipe.TimeToCook.ToString();
                }
            }
            if (bgBG == true)
            {
                if (SuggestedRecipe.USIngredients != null && SuggestedRecipe.USIngredients.Count > 0)
                {
                    foreach (string ingredient in SuggestedRecipe.USIngredients)
                    {
                        RecipeIngredientList.Add(new FoodDef() { BGName = ingredient, Amount = SuggestedRecipe.Amounts[SuggestedRecipe.USIngredients.IndexOf(ingredient)] });
                    }

                    lv_Ingredients.ItemsSource = RecipeIngredientList;
                    lv_Ingredients.Items.Refresh();
                    tB_Directions.Text = SuggestedRecipe.BGDirections.ToString();
                    tB_RecipeTitle.Text = SuggestedRecipe.BGName.ToString();
                    tB_TimeToCook.Text = SuggestedRecipe.TimeToCook.ToString();
                }
            }

        }

        private void ManageSuggestedRecipe(RecipeDef InputRecipe)
        {
            if (enUS == true)
            {
                if (CurrentManager.UserRecipeSaves.RecentlyViewed.Count < 6 && !CurrentManager.UserRecipeSaves.RecentlyViewed.Contains(SuggestedRecipe.USName))
                {
                    CurrentManager.UserRecipeSaves.RecentlyViewed.Add(SuggestedRecipe.USName);
                    CurrentManager.UserRecipeSaves.SaveRecipeSaver();
                }
                else if (!CurrentManager.UserRecipeSaves.RecentlyViewed.Contains(SuggestedRecipe.USName))
                {
                    CurrentManager.UserRecipeSaves.RecentlyViewed.RemoveAt(5);
                    CurrentManager.UserRecipeSaves.RecentlyViewed.Insert(0, SuggestedRecipe.USName);
                }

                if (File.Exists(ImageFolderPath + SuggestedRecipe.ImageFile))
                {
                    SuggestedRecipeImage.ImageSource = new BitmapImage(new Uri(ImageFolderPath + SuggestedRecipe.ImageFile, UriKind.Relative));
                    Img_SuggestedRecipeImage.Fill = SuggestedRecipeImage;
                }
                else
                {
                    Img_SuggestedRecipeImage.Fill = null;
                }

                CurrentManager.StatManager.TotalRecipesSeen++;
                CurrentManager.StatManager.SaveStatistics();
            }

            if (bgBG == true)
            {
                if (CurrentManager.UserRecipeSaves.RecentlyViewed.Count < 6 && !CurrentManager.UserRecipeSaves.RecentlyViewed.Contains(SuggestedRecipe.BGName))
                {
                    CurrentManager.UserRecipeSaves.RecentlyViewed.Add(SuggestedRecipe.BGName);
                    CurrentManager.UserRecipeSaves.SaveRecipeSaver();
                }
                else if (!CurrentManager.UserRecipeSaves.RecentlyViewed.Contains(SuggestedRecipe.BGName))
                {
                    CurrentManager.UserRecipeSaves.RecentlyViewed.RemoveAt(5);
                    CurrentManager.UserRecipeSaves.RecentlyViewed.Insert(0, SuggestedRecipe.BGName);
                }

                if (File.Exists(ImageFolderPath + SuggestedRecipe.ImageFile))
                {
                    SuggestedRecipeImage.ImageSource = new BitmapImage(new Uri(ImageFolderPath + SuggestedRecipe.ImageFile, UriKind.Relative));
                    Img_SuggestedRecipeImage.Fill = SuggestedRecipeImage;
                }
                else
                {
                    Img_SuggestedRecipeImage.Fill = null;
                }

                CurrentManager.StatManager.TotalRecipesSeen++;
                CurrentManager.StatManager.SaveStatistics();
            }

        }

        // Suggests a recipe based on the time of day. Recipe sorting is handled in the back end.
        private void SuggestRecipe()
        {
            if (enUS == true)
            {
                if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 11)
                {
                    if (CurrentManager.RecipieManag.Breakfast.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Breakfast.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Breakfast[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Sorry we ran out of suitable recipes for you. Try a manual search.";
                    }
                }

                if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17)
                {
                    if (CurrentManager.RecipieManag.Lunch.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Lunch.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Lunch[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Sorry we ran out of suitable recipes for you. Try a manual search.";
                    }
                }

                if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 24)
                {
                    if (CurrentManager.RecipieManag.Dinner.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Dinner.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Dinner[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Sorry we ran out of suitable recipes for you. Try a manual search.";
                    }
                }

                if (DateTime.Now.Hour < 7 && DateTime.Now.Hour <= 24)
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = "It's too late for you to eat! Wait till the morning.";
                }
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }

            if (bgBG == true)
            {
                if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 11)
                {
                    if (CurrentManager.RecipieManag.Breakfast.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Breakfast.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Breakfast[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Извинете не успяхме да намерим подходяща рецепте. Опитайте ръчно търсене.";
                    }
                }

                if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17)
                {
                    if (CurrentManager.RecipieManag.Lunch.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Lunch.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Lunch[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Извинете не успяхме да намерим подходяща рецепте. Опитайте ръчно търсене.";
                    }
                }

                if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 24)
                {
                    if (CurrentManager.RecipieManag.Dinner.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Dinner.Count && NumerOfRecipeToSuggest >= 0)
                    {
                        SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Dinner[NumerOfRecipeToSuggest]];
                        tB_RecipeName.Text = SuggestedRecipe.USName;
                        ManageSuggestedRecipe(SuggestedRecipe);
                    }
                    else
                    {
                        tB_RecipeName.Text = null;
                        tB_RecipeName.FontSize = 18;
                        tB_RecipeName.Text = "Извинете не успяхме да намерим подходяща рецепте. Опитайте ръчно търсене.";
                    }
                }

                if (DateTime.Now.Hour < 7 && DateTime.Now.Hour <= 24)
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = "Много е късно да ядете! Изчакайте до сутринта.";
                }
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }
        }

        private void AddToCookedRecipes()
        {
            if (enUS == true)
            {
                CurrentManager.UserRecipeSaves.CookedRecipes.Add(SuggestedRecipe.USName.ToLower());
                CurrentManager.UserRecipeSaves.CookedToday.Add(SuggestedRecipe.USName.ToLower());
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
                CurrentManager.StatManager.AddToCalorieStatistics(SuggestedRecipe.Calories);
                DailyCalories = CurrentManager.StatManager.DailyCalories;
                L_DailyCalories.Text = DailyCalories.ToString();
            }

            if (bgBG)
            {
                CurrentManager.UserRecipeSaves.CookedRecipes.Add(SuggestedRecipe.BGName.ToLower());
                CurrentManager.UserRecipeSaves.CookedToday.Add(SuggestedRecipe.BGName.ToLower());
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
                CurrentManager.StatManager.AddToCalorieStatistics(SuggestedRecipe.Calories);
                DailyCalories = CurrentManager.StatManager.DailyCalories;
                L_DailyCalories.Text = DailyCalories.ToString();
            }

        }

        private void AddToSavedReicpe()
        {
            if (enUS == true)
                CurrentManager.UserRecipeSaves.SavedRecipes.Add(SuggestedRecipe.USName.ToLower());

            if(bgBG == true)
                CurrentManager.UserRecipeSaves.SavedRecipes.Add(SuggestedRecipe.BGName.ToLower());


        }

        private void SavedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchSavedRecipes.Text))
            {
                if (tb_SearchSavedRecipes.Text != "Search")
                {
                    string SearchedSavedRecipe = tb_SearchSavedRecipes.Text;
                    string LowerCase = SearchedSavedRecipe.ToLower();

                    foreach (string Recipe in CurrentManager.UserRecipeSaves.SavedRecipes)
                    {
                        if (Recipe.StartsWith(LowerCase) && !lb_SavedRecipesList.Items.Contains((Recipe.First().ToString().ToUpper() + Recipe.Substring(1))))
                        {
                            lb_SavedRecipesList.Items.Add(Recipe.First().ToString().ToUpper() + Recipe.Substring(1));
                        }
                    }
                }
            }
            else
            {
                lb_SavedRecipesList.Items.Clear();
            }
        }

        private void CookedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchCookedRecipes.Text))
            {
                if (tb_SearchCookedRecipes.Text != "Search")
                {
                    string SearchedRecipe = tb_SearchCookedRecipes.Text;
                    string LowerCase = SearchedRecipe.ToLower();

                    foreach (string Recipe in CurrentManager.UserRecipeSaves.CookedRecipes)
                    {
                        if (Recipe.StartsWith(LowerCase) && !lb_ListOfCookedRecipes.Items.Contains((Recipe.First().ToString().ToUpper() + Recipe.Substring(1))))
                        {
                            lb_ListOfCookedRecipes.Items.Add(Recipe.First().ToString().ToUpper() + Recipe.Substring(1));
                        }
                    }
                }
            }
            else
            {
                lb_ListOfCookedRecipes.Items.Clear();
            }
        }
        #endregion

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

        private void ShowCookedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Cooked Recipes";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Visible;
        }

        private void ShowRecipesCookedToday()
        {
            tB_SavedRecipesPanelTitle.Text = "Cooked Today";
            P_CookedTodayRecipes.Visibility = Visibility.Visible;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Hidden;
        }

        private void ShowAllSavedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Saved Recipes";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Visible;
            P_CookedRecipes.Visibility = Visibility.Hidden;
        }

        private void SetRecentlyViewedImages()
        {
            if (CurrentManager.UserRecipeSaves.RecentlyViewed.Count > 0)
            {
                for (int i = 0; i < CurrentManager.UserRecipeSaves.RecentlyViewed.Count; i++)
                {
                    if (i <= 6 && CurrentManager.RecipieManag.Recipies.ContainsKey(CurrentManager.UserRecipeSaves.RecentlyViewed[i]))
                    {
                        if (File.Exists(ImageFolderPath + CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.RecentlyViewed[i]].ImageFile))
                        {
                            RecentlyViewedRecipeImages[i].ImageSource = new BitmapImage(new Uri(ImageFolderPath + CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.RecentlyViewed[i]].ImageFile, UriKind.Relative));
                            RecentlyViewedRecipesImages[i].Fill = RecentlyViewedRecipeImages[i];
                            if (i <= 4)
                            {
                                FrontPageRecentlyViewedImages[i].Fill = RecentlyViewedRecipeImages[i];
                            }
                        }
                    }
                    else
                    {
                        RecentlyViewedRecipesImages[i].Fill = null;
                        Warning_Label.Text = "There is a corrupted element in a save file! Vist the offical Munchy github for potential fixes";
                    }
                }
            }
        }

        private void ShowRecentlyViewedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Recently Viewed";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Visible;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Hidden;
            SetRecentlyViewedImages();
        }

        private void ShowSavedRecipePanel()
        {
            if (p_SavedRecipes.Visibility == Visibility.Hidden)
            {
                p_SavedRecipes.Visibility = Visibility.Visible;
            }
            else
            {
                p_SavedRecipes.Visibility = Visibility.Hidden;
            }

            ShowAllSavedRecipes();
            CurrentManager.StatManager.CalculateAverageSums();
            tB_TotalCalories.Text = CurrentManager.StatManager.TotalCaloriesConsumed.ToString();
            tB_TotalRecipesCooked.Text = CurrentManager.StatManager.TotalRecipesCooked.ToString();
            tB_TotalRecipeSeen.Text = CurrentManager.StatManager.TotalRecipesSeen.ToString();

            if (CurrentManager.StatManager.AverageDailyCalories != 0)
                tB_AverageDailyCalories.Text = CurrentManager.StatManager.AverageDailyCalories.ToString();
            else
                tB_AverageDailyCalories.Text = "No Data";


            if (CurrentManager.StatManager.AverageMonthtlyCalories != 0)
                tB_AverageMontlyCalories.Text = CurrentManager.StatManager.AverageMonthtlyCalories.ToString();
            else
                tB_AverageMontlyCalories.Text = "No Data";
        }
        #endregion

        #region Fridge related functions

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
                        if (element.Key.StartsWith(ToLower) && !lB_SuggestedFoods.Items.Contains(element.Key))
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
                    lb_FoodList.Items.Add((CurrentManager.FoodManag.Foods[lB_SuggestedFoods.SelectedItem.ToString()].USName));
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
                tb_FoodName.Text = SelectedItem.USName.First().ToString().ToUpper() + SelectedItem.USName.Substring(1);
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
            if (File.Exists(UserFile))
            {
                tb_NameInput.Text = CurrentManager.User.UserName;
                tb_AgeInput.Text = CurrentManager.User.Age.ToString();
                tb_WeightInput.Text = CurrentManager.User.Weight.ToString();

                foreach (CheckBox element in SettingOptions)
                {
                    if (CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[SettingOptions.IndexOf(element)]))
                        element.IsChecked = true;
                    else
                        element.IsChecked = false;
                }
            }
            else
            {
                tb_NameInput.Text = null;
                tb_AgeInput.Text = null;
                tb_WeightInput.Text = null;
            }

            if (p_Settings.Visibility == Visibility.Hidden)
                p_Settings.Visibility = Visibility.Visible;

            else
                p_Settings.Visibility = Visibility.Hidden;

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
        private void btn_ShowRecipе(object sender, RoutedEventArgs e)
        {
            AddRecipeIngredientsToListView();
            ShowOrCloseFullRecipeView();
        }

        /// <summary>
        /// Shows all recipes saved by the user. Function is called on button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SeeAllSavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowSavedRecipePanel();
        }


        private void btn_SuggestRecipe_Click(object sender, RoutedEventArgs e)
        {
            SuggestRecipe();
        }

        private void Btn_ShowNextRecipe_Click(object sender, RoutedEventArgs e)
        {
            tB_RecipeName.FontSize = 24;
            NumerOfRecipeToSuggest++;
            SuggestRecipe();
        }

        private void Btn_ShowPreviousRecipe_Click(object sender, RoutedEventArgs e)
        {
            tB_RecipeName.FontSize = 24;
            NumerOfRecipeToSuggest--;
            SuggestRecipe();
        }

        private void Btn_CookedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowCookedRecipes();
        }

        private void Btn_RecentlyViewed_Click(object sender, RoutedEventArgs e)
        {
            ShowRecentlyViewedRecipes();
        }

        private void Btn_CookedToday_Click(object sender, RoutedEventArgs e)
        {
            ShowRecipesCookedToday();
        }

        private void Btn_SavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowAllSavedRecipes();
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

        private void CloseSavedRecipesPanel(object sender, MouseButtonEventArgs e)
        {
            ShowSavedRecipePanel();
        }

        private void Btn_RecipeWillBeCooked_Click(object sender, RoutedEventArgs e)
        {
            AddToCookedRecipes();
        }

        private void CookedRecipesTextChanged(object sender, TextChangedEventArgs e)
        {
            CookedRecipesSearch();
        }

        private void SearchCookedRecipesFocused(object sender, RoutedEventArgs e)
        {
            tb_SearchCookedRecipes.Text = null;
        }

        private void SearchCookedRecipesLostFocus(object sender, RoutedEventArgs e)
        {
            tb_SearchCookedRecipes.Text = "Search";
        }

        private void SavedRecipeSearchLostFocus(object sender, RoutedEventArgs e)
        {
            tb_SearchSavedRecipes.Text = "Search";
        }

        private void SearchSavedRecipesFocused(object sender, RoutedEventArgs e)
        {
            tb_SearchSavedRecipes.Text = null;
        }

        private void SearchSavedRecipesTextChanged(object sender, TextChangedEventArgs e)
        {
            SavedRecipesSearch();
        }

        private void BTN_AddToSavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            AddToSavedReicpe();
        }

        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            string LocaleCode = (string)((Button)sender).Tag;
            Localizer.SwitchLanguage(this, LocaleCode);
            if (LocaleCode == "bg-bgBG")
            {
                bgBG = true;
                enUS = false;
            }

            if (LocaleCode == "en-US")
            {
                enUS = true;
                bgBG = false;
            }
        }
    }
}