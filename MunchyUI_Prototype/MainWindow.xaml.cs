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
namespace MunchyUI
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
        string UserFile = ProgramFolder + "\\USER.json";

        string UserFridgeFile = ProgramFolder + "\\USER_FRIDGE.json";

        string FoodDefFile = ProgramFolder + "\\FoodData.json";
        string RecipeDatabase = ProgramFolder + "\\Recipes.json";

        string RecipeSaveFile = ProgramFolder + "\\RecipeSavesFile.json";
        string StatSavePath = ProgramFolder + "\\StatSavePath.json";

        bool enUS = true;
        bool bgBG = false;

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

        List<string> ItemsInFoodSearch = new List<string>();
        List<string> ItemsFoodList = new List<string>();

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
        RadioButton[] AmountRadioButtons;
        // ================= UI LOGIC =================
        #region UI Logic

        #region Initialization functions

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists(FoodDefFile) || !File.Exists(RecipeDatabase))
            {
                MessageBox.Show("EN: The program seems to have encountered an error Press OK to open the support page and look for the following code : Error Code: ERR-RFM " + "\n" + "\n" + " BG: Грешка! : Липсват файлове нужни за функционирането на програмата.Моля посетете https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting за насоки да поправите грешката.");
                System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
                Close();
                return;
            }

            SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };

            if (!File.Exists(UserFile))
            {
                
                MessageBox.Show("It seems like your user file is empty. Take a moment to fill in some of your details. This will help Munchy suggest recipes exactly to your tastes." + "\n" + "\n" + "Изглежда че вашия личен файл е празен. Отделете няколко минути да попълните информация за вашите предпочитания. Това ще помогне на програмата да предляга подходящи за вас рецепти.");
            }

            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, StatSavePath);

            if (CurrentManager.User.LanguagePref == "BG")
            {
                bgBG = true;
                enUS = false;
                Localizer.SwitchLanguage(this, "bg-BG");

            }

            SummaryTextBlocks = new TextBlock[] { tB_CalorieSummary, tB_ProteinSummary, tB_FatSummary, tB_CarbsSummary, tB_SugarSumary, tB_SodiumSummary };
            RecentlyViewedRecipeImages = new ImageBrush[] { RecentRecipe_1, RecentRecipe_2, RecentRecipe_3, RecentRecipe_4, RecentRecipe_5, RecentRecipe_6 };
            RecentlyViewedRecipesImages = new Ellipse[] { Img_RecentlyViewed_1, Img_RecentlyViewed_2, Img_RecentlyViewed_3, Img_RecentlyViewed_4, Img_RecentlyViewed_5, Img_RecentlyViewed_6 };
            FrontPageRecentlyViewedImages = new Ellipse[] { Img_FrontRecentlyViewed_1, Img_FrontRecentlyViewed_2, Img_FrontRecentlyViewed_3, Img_FrontRecentlyViewed_4, Img_FrontRecentlyViewed_5 };
            AmountRadioButtons = new RadioButton[] { Cb_SuggestedAmount_1, Cb_SuggestedAmount_2, Cb_SuggestedAmount_3, Cb_SuggestedAmount_4 };
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
            if (CurrentManager.User.UserFridge.USUsersFoods != null && CurrentManager.User.UserFridge.USUsersFoods.Count > 0)
            {
                RefreshFridge();
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.USUsersFoods)
                {
                    if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1)))
                        lb_Fridge.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1));
                }
            }
        }
        #endregion


        #region Recipe related functions

        // Opens or closes the full recipe view.
        private void ShowOrCloseFullRecipeView()
        {
            if (File.Exists(ImageFolderPath + SuggestedRecipe.ImageFile))
            {
                RecipeImage.ImageSource = new BitmapImage(new Uri(ImageFolderPath + SuggestedRecipe.ImageFile, UriKind.Relative));
                img_RecipeImage.Fill = RecipeImage;
            }
            else
            {
                img_RecipeImage.Fill = null;
                MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
            }
        }

        // Adds recipe information to the full recipe view. 
        // Function is called when the recipe view is opened, or when a new recipe is suggested.
        private void AddRecipeIngredientsToListView()
        {
            RecipeIngredientList.Clear();

            if (GetIngredientList(SuggestedRecipe) != null && GetIngredientList(SuggestedRecipe).Count > 0)
            {
                foreach (string ingredient in GetIngredientList(SuggestedRecipe))
                {
                    RecipeIngredientList.Add(new FoodDef() { USName = ingredient, Amount = SuggestedRecipe.Amounts[GetIngredientList(SuggestedRecipe).IndexOf(ingredient)] });
                }

                lv_Ingredients.ItemsSource = RecipeIngredientList;
                lv_Ingredients.Items.Refresh();
                tB_Directions.Text = GetDirections(SuggestedRecipe);
                tB_RecipeTitle.Text = GetSuggestedRecipeName(SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(SuggestedRecipe).ToString().Substring(1);
                tB_TimeToCookAmount.Text = SuggestedRecipe.TimeToCook.ToString();
            }

        }

        private void ManageSuggestedRecipe(RecipeDef InputRecipe)
        {

            if (CurrentManager.UserRecipeSaves.USRecentlyViewed.Count < 6 && CurrentManager.UserRecipeSaves.BGRecentlyViewed.Count < 6)
            {
                if (!CurrentManager.UserRecipeSaves.USRecentlyViewed.Contains(SuggestedRecipe.USName))
                    CurrentManager.UserRecipeSaves.USRecentlyViewed.Add(SuggestedRecipe.USName);

                if (!CurrentManager.UserRecipeSaves.BGRecentlyViewed.Contains(SuggestedRecipe.BGName.ToLower()))
                    CurrentManager.UserRecipeSaves.BGRecentlyViewed.Add(SuggestedRecipe.BGName.ToLower());

                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }

            else
            {
                if (!CurrentManager.UserRecipeSaves.USRecentlyViewed.Contains(SuggestedRecipe.USName.ToLower()))
                {
                    CurrentManager.UserRecipeSaves.USRecentlyViewed.RemoveAt(5);
                    CurrentManager.UserRecipeSaves.USRecentlyViewed.Insert(0, SuggestedRecipe.USName.ToLower());
                }

                if (!CurrentManager.UserRecipeSaves.BGRecentlyViewed.Contains(SuggestedRecipe.BGName.ToLower()))
                {
                    CurrentManager.UserRecipeSaves.BGRecentlyViewed.RemoveAt(5);
                    CurrentManager.UserRecipeSaves.BGRecentlyViewed.Insert(0, SuggestedRecipe.BGName.ToLower());
                }
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }

            if (File.Exists(ImageFolderPath + SuggestedRecipe.ImageFile))
            {
                SuggestedRecipeImage.ImageSource = new BitmapImage(new Uri(ImageFolderPath + SuggestedRecipe.ImageFile, UriKind.Relative));
                Img_SuggestedRecipeImage.Fill = SuggestedRecipeImage;
            }
            else
            {
                Img_SuggestedRecipeImage.Fill = null;
                MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
            }

            CurrentManager.StatManager.TotalRecipesSeen++;
            CurrentManager.StatManager.SaveStatistics();

        }

        // Suggests a recipe based on the time of day. Recipe sorting is handled in the back end.
        private void SuggestRecipe()
        {

            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 11)
            {
                if (CurrentManager.RecipieManag.Breakfast.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Breakfast.Count && NumerOfRecipeToSuggest >= 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Breakfast[NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(enUS, bgBG);
                }
            }

            if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17)
            {
                if (CurrentManager.RecipieManag.Lunch.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Lunch.Count && NumerOfRecipeToSuggest >= 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Lunch[NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(enUS, bgBG);
                }
            }

            if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 24)
            {
                if (CurrentManager.RecipieManag.Dinner.Count > 0 && NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Dinner.Count && NumerOfRecipeToSuggest >= 0)
                {
                    SuggestedRecipe = CurrentManager.RecipieManag.Recipies[CurrentManager.RecipieManag.Dinner[NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(enUS, bgBG);
                }
            }

            if (DateTime.Now.Hour < 7 && DateTime.Now.Hour <= 24)
            {
                tB_RecipeName.Text = null;
                tB_RecipeName.FontSize = 18;
                tB_RecipeName.Text = TranslatorCore.GetTooLateToEatMessage(enUS, bgBG);
            }
            CurrentManager.UserRecipeSaves.SaveRecipeSaver();

        }

        private void AddToCookedRecipes()
        {
            if (!CurrentManager.UserRecipeSaves.USCookedRecipes.Contains(SuggestedRecipe.USName.ToLower()))
                CurrentManager.UserRecipeSaves.USCookedRecipes.Add(SuggestedRecipe.USName.ToLower());

            //CurrentManager.UserRecipeSaves.USCookedToday.Add(SuggestedRecipe.USName.ToLower());

            if (!CurrentManager.UserRecipeSaves.BGCookedRecipes.Contains(SuggestedRecipe.BGName.ToLower()))
                CurrentManager.UserRecipeSaves.BGCookedRecipes.Add(SuggestedRecipe.BGName.ToLower());


            // CurrentManager.UserRecipeSaves.BGCookedToday.Add(SuggestedRecipe.BGName.ToLower());

            CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            CurrentManager.StatManager.AddToCalorieStatistics(SuggestedRecipe.Calories);
            DailyCalories = CurrentManager.StatManager.DailyCalories;
            L_DailyCalories.Text = DailyCalories.ToString();
            CurrentManager.UsersFridge.ModifyFoodItemAmount(SuggestedRecipe.USIngredients, SuggestedRecipe.Amounts);
            RefreshFridge();
        }

        private void AddToSavedReicpe()
        {
            if (!CurrentManager.UserRecipeSaves.USSavedRecipes.Contains(SuggestedRecipe.USName.ToLower()))
                CurrentManager.UserRecipeSaves.USSavedRecipes.Add(SuggestedRecipe.USName.ToLower());

            if (!CurrentManager.UserRecipeSaves.BGSavedRecipes.Contains(SuggestedRecipe.BGName.ToLower()))
                CurrentManager.UserRecipeSaves.BGSavedRecipes.Add(SuggestedRecipe.BGName.ToLower());

        }

        private void SavedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchSavedRecipes.Text))
            {
                if (tb_SearchSavedRecipes.Text != TranslatorCore.GetTextboxDefaultText(enUS, bgBG))
                {
                    string SearchedSavedRecipe = tb_SearchSavedRecipes.Text;
                    string LowerCase = SearchedSavedRecipe.ToLower();

                    foreach (string Recipe in GetSavedRecipesList())
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
                if (tb_SearchCookedRecipes.Text != TranslatorCore.GetTextboxDefaultText(enUS, bgBG))
                {
                    string SearchedRecipe = tb_SearchCookedRecipes.Text;
                    string LowerCase = SearchedRecipe.ToLower();

                    foreach (string Recipe in GetCookedRecipeList())
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
            if (CurrentManager.UserRecipeSaves.USRecentlyViewed.Count > 0)
            {
                for (int i = 0; i < CurrentManager.UserRecipeSaves.USRecentlyViewed.Count; i++)
                {
                    if (i <= 6 && CurrentManager.RecipieManag.Recipies.ContainsKey(CurrentManager.UserRecipeSaves.USRecentlyViewed[i]))
                    {
                        if (File.Exists(ImageFolderPath + CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile))
                        {
                            RecentlyViewedRecipeImages[i].ImageSource = new BitmapImage(new Uri(ImageFolderPath + CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile, UriKind.Relative));
                            RecentlyViewedRecipesImages[i].Fill = RecentlyViewedRecipeImages[i];
                            if (i <= 4)
                            {
                                FrontPageRecentlyViewedImages[i].Fill = RecentlyViewedRecipeImages[i];
                            }
                        }
                        else
                        {
                            MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                            System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
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

        private void RefreshFrontPageFride()
        {
            lb_Fridge.Items.Clear();
            foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.USUsersFoods)
            {
                if (enUS)
                {
                    if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1)))
                        lb_Fridge.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1));
                }

                if (bgBG)
                {
                    if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1)))
                        lb_Fridge.Items.Add(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1));
                }
                
            }
        }

        // Called when the text in the textbox for searching for foods is changed
        private void FoodSearchTextChanged()
        {
            // Checks if the search box is null or not.
            if (!string.IsNullOrWhiteSpace(tb_Search.Text))
            {
                //Makes sure that text is not the keyword "Search"
                if (tb_Search.Text != TranslatorCore.GetTextboxDefaultText(enUS, bgBG))
                {
                    l_SearchInfo.Text = TranslatorCore.GetClickToAddFoodMessage(enUS, bgBG);
                    string searchedWord = tb_Search.Text;
                    string ToLower = searchedWord.ToLower();

                    foreach (KeyValuePair<string, FoodDef> element in CurrentManager.FoodManag.Foods)
                    {
                        if (enUS == true)
                        {
                            if (element.Key.StartsWith(ToLower) && !lB_SuggestedFoods.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString()))
                            {
                                lB_SuggestedFoods.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString());
                                ItemsInFoodSearch.Add(element.Value.USName);
                            }
                        }

                        if (bgBG == true)
                        {
                            if (element.Value.BGName.StartsWith(ToLower) && !lB_SuggestedFoods.Items.Contains(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString()))
                            {
                                lB_SuggestedFoods.Items.Add(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString());
                                ItemsInFoodSearch.Add(element.Value.USName);
                            }
                        }
                    }
                }
            }
            else
            {
                lB_SuggestedFoods.Items.Clear();
                ItemsInFoodSearch.Clear();
                l_SearchInfo.Text = TranslatorCore.GetTypeForFoodPrompt(enUS, bgBG);
            }
        }

        private void RefreshFridge()
        {
            lb_FoodList.Items.Clear();
            ItemsFoodList.Clear();
            foreach (KeyValuePair<string, FoodDef> ItemToAdd in CurrentManager.UsersFridge.USUsersFoods)
            {
                ItemsFoodList.Add(ItemToAdd.Key.ToLower());

                if (!lb_FoodList.Items.Contains(ItemToAdd.Value.USName) || !lb_FoodList.Items.Contains(ItemToAdd.Value.BGName))
                {
                    if (enUS == true)
                        lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.Value.USName.First().ToString().ToUpper() + ItemToAdd.Value.USName.Substring(1), Amount = ItemToAdd.Value.Amount });

                    if (bgBG == true)
                        lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.Value.BGName.First().ToString().ToUpper() + ItemToAdd.Value.BGName.Substring(1), Amount = ItemToAdd.Value.Amount });
                }
            }
            lb_FoodList.Items.Refresh();
            RefreshFrontPageFride();
        }

        private void TranslateFridge()
        {
            lb_FoodList.Items.Clear();

            RefreshFridge();
        }

        private void AddFoodItem()
        {
            FoodDef ItemToAdd = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]];

            foreach (RadioButton element in AmountRadioButtons)
            {
                if (element.IsChecked == true)
                {
                    ItemToAdd.Amount = (float)(element.Content);
                }
            }

            // Add check if tb input number.
            if (ItemToAdd.Amount == 0 && !string.IsNullOrWhiteSpace(Tb_CustomAmount.Text))
            {
                ItemToAdd.Amount = float.Parse(Tb_CustomAmount.Text);
                L_FoodAmountWarning.Text = null;
            }

            if (ItemToAdd.Amount == 0)
            {
                L_FoodAmountWarning.Text = TranslatorCore.FoodAmountNullWarning(enUS, bgBG);
            }

            if (!CurrentManager.User.UserFridge.USUsersFoods.ContainsKey(ItemToAdd.USName))
            {
                CurrentManager.User.UserFridge.AddToFridge(ItemToAdd);

                if (enUS == true)
                    lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.USName.First().ToString().ToUpper() + ItemToAdd.USName.ToString().Substring(1), Amount = ItemToAdd.Amount });

                if (bgBG == true)
                    lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.BGName, Amount = ItemToAdd.Amount });

                CurrentManager.UsersFridge.SaveFridge();
                l_SearchInfo.Text = TranslatorCore.ItemAddedMessage(enUS, bgBG);

                RefreshFridge();
                PopulateFridgeSummary();
            }
            else
            {
                l_SearchInfo.Text = TranslatorCore.ItemAlreadyInFridgeMessage(enUS, bgBG);
            }

            Tb_CustomAmount.Clear();
            foreach (RadioButton element in AmountRadioButtons)
            {
                if (element.IsChecked == true)
                {
                    element.IsChecked = false;
                }
            }
        }

        // Opens food item configuration panel and loads suggested amounts to add to the fridge.
        private void ConfigureClickedItem()
        {
            if (lB_SuggestedFoods.SelectedItem != null)
            {
                TB_AddedFoodNameItem.Text = lB_SuggestedFoods.SelectedItem.ToString().First().ToString().ToUpper() + lB_SuggestedFoods.SelectedItem.ToString().Substring(1);

                foreach (RadioButton checkBox in AmountRadioButtons)
                {
                    checkBox.Content = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].SuggestedAmounts[Array.IndexOf(AmountRadioButtons, checkBox)];
                }

                Tb_UOMLabel.Text = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].UOM;
            }
        }

        private void RemoveItem()
        {

            if (lb_FoodList.SelectedItem != null)
            {
                FoodDef ItemToRemove = (FoodDef)CurrentManager.FoodManag.Foods[ItemsFoodList[lb_FoodList.SelectedIndex].ToLower()];
                CurrentManager.User.UserFridge.RemoveFromFridge(ItemToRemove.USName.ToLower());
                lb_FoodList.Items.Remove(lb_FoodList.SelectedIndex);
                lb_FoodList.Items.Clear();
                CurrentManager.User.UserFridge.SaveFridge();
                RefreshFridge();
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
            if (CurrentManager.User.UserFridge.USUsersFoods.Count > 0)
            {
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.USUsersFoods)
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
                
                    if (CurrentManager.User.LanguagePref == "BG")
                        CB_Bulgarian.IsChecked = true;

                    if (CurrentManager.User.LanguagePref == "US")
                        CB_English.IsChecked = true;                              
            }
            else
            {
                tb_NameInput.Text = null;
                tb_AgeInput.Text = null;
                tb_WeightInput.Text = null;
            }

            //if (p_Settings.Visibility == Visibility.Hidden)
            //    p_Settings.Visibility = Visibility.Visible;

            //else
            //    p_Settings.Visibility = Visibility.Hidden;

        }

        private void SaveUserSettings()
        {
            if (!string.IsNullOrWhiteSpace(tb_NameInput.Text) && !string.IsNullOrWhiteSpace(tb_AgeInput.Text) && !string.IsNullOrWhiteSpace(tb_WeightInput.Text))
            {
                CurrentManager.User.UserName = tb_NameInput.Text;

                if (int.TryParse(tb_AgeInput.Text, out int parsedValue))
                    CurrentManager.User.Age = int.Parse(tb_AgeInput.Text);
                else MessageBox.Show("Age is a number value!");

                if (int.TryParse(tb_WeightInput.Text, out parsedValue))
                    CurrentManager.User.Weight = int.Parse(tb_WeightInput.Text);
                else MessageBox.Show("Weight is a number value!");
            }

            if (rb_Male.IsChecked == true)
                CurrentManager.User.Sex = "male";

            if (rb_Female.IsChecked == true)
                CurrentManager.User.Sex = "female";

            if (rb_Other.IsChecked == true)
                CurrentManager.User.Sex = "other";

            if (CB_English.IsChecked == true)
                CurrentManager.User.LanguagePref = "US";

            if (CB_Bulgarian.IsChecked == true)
            {
                bgBG = true;
                enUS = false;
                CurrentManager.User.LanguagePref = "BG";
                Localizer.SwitchLanguage(this, "bg-BG");
            }

            if (CB_English.IsChecked == true)
            {
                bgBG = false;
                enUS = true;
                CurrentManager.User.LanguagePref = "US";
                Localizer.SwitchLanguage(this, "en-US");
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
        private void Btn_ShowRecipe_Click(object sender, RoutedEventArgs e)
        {
            ShowOrCloseFullRecipeView();
        }


        /// <summary>
        /// Opens full fridge view for the fridge. Function called by the "Show Fridge" button and the close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Showfridge_Click(object sender, RoutedEventArgs e)
        {
            RefreshFridge();
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
        private void Btn_RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem();
        }

        #endregion

        #region Recipe related
        private void Btn_ShowRecipе(object sender, RoutedEventArgs e)
        {
            AddRecipeIngredientsToListView();
            ShowOrCloseFullRecipeView();
        }

        /// <summary>
        /// Shows all recipes saved by the user. Function is called on button press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SeeAllSavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowSavedRecipePanel();
            SetRecentlyViewedImages();
        }
                       
        private void Btn_ShowNextRecipe_Click(object sender, RoutedEventArgs e)
        {
            tB_RecipeName.FontSize = 24;
            NumerOfRecipeToSuggest++;
            SuggestRecipe();
            SetRecentlyViewedImages();
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
            SetRecentlyViewedImages();
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
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
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
            tb_Search.Text = TranslatorCore.GetTextboxDefaultText(enUS, bgBG);
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
            ConfigureClickedItem();
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
            tb_SearchCookedRecipes.Text = TranslatorCore.GetTextboxDefaultText(enUS, bgBG);
        }

        private void SavedRecipeSearchLostFocus(object sender, RoutedEventArgs e)
        {
            tb_SearchSavedRecipes.Text = TranslatorCore.GetTextboxDefaultText(enUS, bgBG);
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
            if (LocaleCode == "bg-BG")
            {
                bgBG = true;
                enUS = false;
            }

            if (LocaleCode == "en-US")
            {
                enUS = true;
                bgBG = false;
            }

            AddRecipeIngredientsToListView();
            TranslateFridge();
        }

        private List<string> GetSavedRecipesList()
        {
            List<string> ListToUse = new List<string>();

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.USSavedRecipes;
                }


                if (bgBG == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.BGSavedRecipes;
                }
            }
            else
            {
                ListToUse = new List<string>();
            }

            return ListToUse;
        }

        private List<string> GetRecentlyViewedList()
        {
            List<string> ListToUse = new List<string>();

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.USRecentlyViewed;
                }


                if (bgBG == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.BGRecentlyViewed;
                }
            }

            else
            {
                ListToUse = new List<string>();
            }
            return ListToUse;
        }

        private string GetDirections(RecipeDef Recipe)
        {
            string RecipeName = "RecipeName";

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    RecipeName = Recipe.USDirections;
                }


                if (bgBG == true)
                {
                    RecipeName = Recipe.BGDirections;
                }
            }
            else
            {
                MessageBox.Show("EN: Error Code : ERR-RFC : There seems to be a corrupted file. Please click Ok to open the support page. ");
                System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
                RecipeName = "Error Code: ERR-RFC";
            }

            return RecipeName;
        }

        private List<string> GetIngredientList(RecipeDef Recipe)
        {
            List<string> ListToUse = new List<string>();

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    ListToUse = Recipe.USIngredients;
                }

                if (bgBG == true)
                {
                    ListToUse = ListToUse = Recipe.BGIngredients;
                }
            }
            else
            {
                ListToUse = new List<string>();
            }


            return ListToUse;

        }

        private List<string> GetCookedRecipeList()
        {
            List<string> ListToUse = new List<string>();

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.USCookedRecipes;
                }


                if (bgBG == true)
                {
                    ListToUse = CurrentManager.UserRecipeSaves.BGCookedRecipes;
                }
            }
            else
            {
                ListToUse = new List<string>();
            }

            return ListToUse;
        }


        private string GetSuggestedRecipeName(RecipeDef Recipe)
        {
            string RecipeName = "RecipeName";

            if (enUS == true || bgBG == true)
            {
                if (enUS == true)
                {
                    RecipeName = Recipe.USName;
                }


                if (bgBG == true)
                {
                    RecipeName = Recipe.BGName;
                }
            }
            else
            {
                RecipeName = "Error TE303";
            }

            return RecipeName;
        }

        private void AddFoodItemClick(object sender, RoutedEventArgs e)
        {
            AddFoodItem();
        }

        private void Btn_DiscoverClick(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_FindRecipe_Click(object sender, RoutedEventArgs e)
        {
            SuggestRecipe();
        }

        private void Btn_Showfridge_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void UncheckEnglish(object sender, RoutedEventArgs e)
        {
            CB_English.IsChecked = false;
        }

        private void UncheckBulgarian(object sender, RoutedEventArgs e)
        {
            CB_Bulgarian.IsChecked = false;
        }
    }
}