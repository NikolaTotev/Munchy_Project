﻿using System;
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
    public enum Languages
    {
        English,
        Bulgarian
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Getting user applicaitondata folder.
        static string LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string ProgramFolder = System.IO.Path.Combine(LocalAppDataPath, "Munchy");
        static string ImageFolderPath = System.IO.Path.Combine(ProgramFolder, "Images");

        //Data File Locations
        string UserFile = System.IO.Path.Combine(ProgramFolder, "USER.json");

        string UserFridgeFile = System.IO.Path.Combine(ProgramFolder, "USER_FRIDGE.json");

        string FoodDefFile = System.IO.Path.Combine(ProgramFolder, "FoodData.json");
        string RecipeDatabase = System.IO.Path.Combine(ProgramFolder, "Recipes.json");

        string RecipeSaveFile = System.IO.Path.Combine(ProgramFolder, "RecipeSavesFile.json");
        string StatSavePath = System.IO.Path.Combine(ProgramFolder, "StatSavePath.json");

        //Booleans for determining what language to use in runtime. English is the default language.        
        Languages ActiveLanguage = Languages.English;
        //Variables that are used to display the fridge summary.
        float CalorieSum = 0;
        float ProteinSum = 0;
        float FatSum = 0;
        float CarbSum = 0;
        float SugarSum = 0;
        float SodiumSum = 0;

        //This textblock array is used to store all the summary textboxes. This enables easy population of said textboxes.
        TextBlock[] SummaryTextBlocks;

        //This float array stores the values mentioned above. It allows for fast population of the Textblock array
        float[] SummaryValues;

        //Integer that keeps track of daily calories the user has consumed.
        int DailyCalories = 0;

        //Integer that keeps track of what recipe the user is on. This integer is responsible for browsing through the suggested recipe list.
        int NumerOfRecipeToSuggest = 0;

        //A list of checkboxes that are used for saving the users settings and preferences
        List<CheckBox> SettingOptions;

        //This list is used to keep track of the elements in the SuggestedFoods listbox.
        List<string> ItemsInFoodSearch = new List<string>();

        //This list is used to keep track of the elements in the users fridge.
        List<string> ItemsFoodList = new List<string>();

        // This list is used for populating the Ingredients ListView in the UI.
        List<FoodDef> RecipeIngredientList;

        //This is the RecipeDef item that is used to store the user is supposed to see. It is updated everytime the user  wants to go to the next recipe
        // in the sugggested recipe list.
        RecipeDef SuggestedRecipe = new RecipeDef();

        //This is the main brain of the application. It is the interface between the UI and back end.
        ProgramManager CurrentManager;

        //RecipeImage handles image of the ful recipe view.
        ImageBrush RecipeImage = new ImageBrush();

        //SuggestedRecipeImage handles showing the image of the recipe on the front page.
        ImageBrush SuggestedRecipeImage = new ImageBrush();

        //These brushes are used for loading recently viewed recipes. Each is assigned to a different elipse on the UI.
        ImageBrush RecentRecipe_1 = new ImageBrush();
        ImageBrush RecentRecipe_2 = new ImageBrush();
        ImageBrush RecentRecipe_3 = new ImageBrush();
        ImageBrush RecentRecipe_4 = new ImageBrush();
        ImageBrush RecentRecipe_5 = new ImageBrush();
        ImageBrush RecentRecipe_6 = new ImageBrush();

        //This imagebrush array stores the RecentRecipe brushes.
        ImageBrush[] RecentlyViewedRecipeImages;

        //This Elipse array stores the elipses on the "SavedRecipes" panel.
        Ellipse[] RecentlyViewedRecipesImages;

        //This elipse array stores the elipses on the front page.
        Ellipse[] FrontPageRecentlyViewedImages;

        //These RadioButtons are used for setting the amounts of the fooditems the user adds.
        RadioButton[] AmountRadioButtons;


        // ================= UI LOGIC =================
        #region UI Logic

        #region Initialization functions
        //Main window initialization
        public MainWindow()
        {
            InitializeComponent();

            //Checks if FoodDefFile and RecipeDatabase exist, if they dont a message box is shown and the user is redirected to the Support page.
            //This prevents operation errors down the line. These two files are critical for the operation of the program.
            if (!File.Exists(FoodDefFile) || !File.Exists(RecipeDatabase))
            {
                MessageBox.Show("EN: The program seems to have encountered an error Press OK to open the support page and look for the following code : Error Code: ERR-RFM " + "\n" + "\n" + " BG: Грешка! : Липсват файлове нужни за функционирането на програмата.Моля посетете https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting за насоки да поправите грешката.");
                System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
                Close();
                return;
            }

            //Checks if the UserFile exists. If it doesn't this suggests that this is the first time the user is using the program and therefore he is prompted to
            //enter his information. The information is used to suggest recipes.
            if (!File.Exists(UserFile))
            {
                MessageBox.Show("It seems like your user file is empty. Take a moment to fill in some of your details. This will help Munchy suggest recipes exactly to your tastes." + "\n" + "\n" + "Изглежда че вашия личен файл е празен. Отделете няколко минути да попълните информация за вашите предпочитания. Това ще помогне на програмата да предляга подходящи за вас рецепти.");
            }

            //Intitially sets the default language.
            Localizer.SetDefaultLanguage(this);

            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, RecipeDatabase, FoodDefFile, RecipeSaveFile, StatSavePath);

            //Sets the users name.
            if (CurrentManager.User.UserName != null)
                tB_UserName.Text = CurrentManager.User.UserName;

            //Gets the language that the user has saved. It does not check if it is english as english is the default for the program.
            if (CurrentManager.User.LanguagePref == "BG")
            {
                ActiveLanguage = Languages.Bulgarian;
                Localizer.SwitchLanguage(this, "bg-BG");
            }

            SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };
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
        }

        //On start populates the fridge on the main page with 10 items from the UserFridge.
        private void InitialFridgeUISetup()
        {
            if (CurrentManager.User.UserFridge.USUsersFoods != null && CurrentManager.User.UserFridge.USUsersFoods.Count > 0)
            {
                RefreshFridge();
                foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.USUsersFoods)
                {
                    if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1)))
                    {
                        lb_Fridge.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1));
                    }
                }
            }
        }
        #endregion

        #region Recipe related functions
        #region Hanlding browsing recipes.
        //Algorithm responsible for suggesting a recipe based on time of day. It uses only compatable recipes that are sorted into lists of 
        // breakfast, lunch and dinner by the RecipeManager. (NOTE) It uses ONLY compatable recipes.
        private void SuggestRecipe()
        {
            //Breakfast.
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
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //Lunch.
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
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //Dinner.
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
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //No eating time.
            if (DateTime.Now.Hour < 7 && DateTime.Now.Hour <= 24)
            {
                tB_RecipeName.Text = null;
                tB_RecipeName.FontSize = 18;
                tB_RecipeName.Text = TranslatorCore.GetTooLateToEatMessage(ActiveLanguage);
                Img_SuggestedRecipeImage.Fill = Brushes.White;
            }

            CurrentManager.UserRecipeSaves.SaveRecipeSaver();
        }

        //Handles the suggested recipe. Updates the recently viewed list and adds the recipe to it, handles images of recently viewed recipes.
        private void ManageSuggestedRecipe(RecipeDef inputrecipe)
        {
            RecipeSaver saver = CurrentManager.UserRecipeSaves;
           
            if (saver.USRecentlyViewed.Count < 6 && saver.BGRecentlyViewed.Count < 6)
            {
                AddToList(saver.USRecentlyViewed, SuggestedRecipe.USName);
                AddToList(saver.BGRecentlyViewed, SuggestedRecipe.BGName);
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }
            else
            {
                if (!saver.USRecentlyViewed.Contains(SuggestedRecipe.USName.ToLower()))
                {
                    saver.USRecentlyViewed.RemoveAt(5);
                    saver.USRecentlyViewed.Insert(0, SuggestedRecipe.USName.ToLower());
                }

                if (!saver.BGRecentlyViewed.Contains(SuggestedRecipe.BGName.ToLower()))
                {
                    saver.BGRecentlyViewed.RemoveAt(5);
                    saver.BGRecentlyViewed.Insert(0, SuggestedRecipe.BGName.ToLower());
                }
                CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }

            if (File.Exists(System.IO.Path.Combine(ImageFolderPath, SuggestedRecipe.ImageFile)))
            {
                SuggestedRecipeImage.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(ImageFolderPath, SuggestedRecipe.ImageFile), UriKind.Relative));
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

        //Sets up the initial FullRecipe view. Called when the "Show Recipe" button is pressed.
        private void SetupFullRecipeViewImg()
        {
            if (SuggestedRecipe != null && CurrentManager.RecipieManag.CompatableRecipes.Count > 0)
            {
                if (File.Exists(System.IO.Path.Combine(ImageFolderPath, SuggestedRecipe.ImageFile)))
                {
                    RecipeImage.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(ImageFolderPath, SuggestedRecipe.ImageFile), UriKind.Relative));
                    img_RecipeImage.Fill = RecipeImage;
                }
                else
                {
                    img_RecipeImage.Fill = null;
                    MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                    System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
                }
            }

        }

        //Adds information about the recipe to the FullRecipeView panel.
        private void AddInformationToFullRecipeView()
        {
            RecipeIngredientList.Clear();

            if (GetIngredientList(SuggestedRecipe) != null && GetIngredientList(SuggestedRecipe).Count > 0)
            {
                foreach (string ingredient in GetIngredientList(SuggestedRecipe))
                {
                    if (GetIngredientList(SuggestedRecipe).Count == SuggestedRecipe.Units.Count)
                    {
                        RecipeIngredientList.Add(new FoodDef() { USName = ingredient, IngrAmount = SuggestedRecipe.Amounts[GetIngredientList(SuggestedRecipe).IndexOf(ingredient)].ToString() + " " + SuggestedRecipe.Units[GetIngredientList(SuggestedRecipe).IndexOf(ingredient)].ToString() });
                    }

                    else
                    {
                        MessageBox.Show("You seem to have a probelem with the Recipe File. Press OK to open the support page.");
                        System.Diagnostics.Process.Start("https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting");
                        break;
                    }
                }

                lv_Ingredients.ItemsSource = RecipeIngredientList;
                lv_Ingredients.Items.Refresh();
                tB_Directions.Text = GetDirections(SuggestedRecipe);
                tB_RecipeTitle.Text = GetSuggestedRecipeName(SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(SuggestedRecipe).ToString().Substring(1);
                tB_TimeToCookAmount.Text = SuggestedRecipe.TimeToCook.ToString();
            }
        }

        //Handles adding the suggested to the CookedRecipes list. A recipe is added only when the user pressed the "I'll Cook It" Button.
        private void AddToCookedRecipes()
        {
            RecipeSaver saver = CurrentManager.UserRecipeSaves;
            AddToList(saver.USCookedRecipes, SuggestedRecipe.USName);
            AddToList(saver.BGCookedRecipes, SuggestedRecipe.BGName);

            CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            CurrentManager.StatManager.AddToCalorieStatistics(SuggestedRecipe.Calories);
            DailyCalories = CurrentManager.StatManager.DailyCalories;
            L_DailyCalories.Text = DailyCalories.ToString();
            CurrentManager.UsersFridge.ModifyFoodItemAmount(SuggestedRecipe.USIngredients, SuggestedRecipe.Amounts, SuggestedRecipe.Units, CurrentManager.FoodManag);
            RefreshFridge();
        }

        //Handles adding the suggested recipe to saved recipes. A recipe is saved only when the user pressed the "Save" button.
        private void AddToSavedReicpe()
        {
            RecipeSaver saver = CurrentManager.UserRecipeSaves;
            AddToList(saver.USSavedRecipes, SuggestedRecipe.USName.ToString());
            AddToList(saver.BGSavedRecipes, SuggestedRecipe.BGName.ToString());           
        }

        //Handles adding recipes to lists.
        private void AddToList (List<string> savelist, string recipename)
        {
            if (!savelist.Contains(recipename.ToLower()))
            {
                savelist.Add(recipename.ToLower());
            }
        }
        #endregion

        #region Handling searching.
        //Handles searching in the SavedRecipe list.
        private void SavedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchSavedRecipes.Text))
            {
                if (tb_SearchSavedRecipes.Text != TranslatorCore.GetTextboxDefaultText(ActiveLanguage))
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

        //Handles searching in the CookedRecipe list
        private void CookedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchCookedRecipes.Text))
            {
                if (tb_SearchCookedRecipes.Text != TranslatorCore.GetTextboxDefaultText(ActiveLanguage))
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
        #endregion

        #region Panel related functions      
        //Configures and refreshes the content on the SavedRecipe pannel. Fucntion is called when the use presses the "Show All" button on the main menu
        private void ConfigureSavedRecipesPanel()
        {
            ShowAllSavedRecipes();
            CurrentManager.StatManager.CalculateAverageSums();
            tB_TotalCalories.Text = CurrentManager.StatManager.TotalCaloriesConsumed.ToString();
            tB_TotalRecipesCooked.Text = CurrentManager.StatManager.TotalRecipesCooked.ToString();
            tB_TotalRecipeSeen.Text = CurrentManager.StatManager.TotalRecipesSeen.ToString();

            if (CurrentManager.StatManager.AverageDailyCalories != 0)
            {
                tB_AverageDailyCalories.Text = CurrentManager.StatManager.AverageDailyCalories.ToString();
            }
            else
            {
                tB_AverageDailyCalories.Text = TranslatorCore.GetNoDataLabel(ActiveLanguage);
            }

            if (CurrentManager.StatManager.AverageMonthtlyCalories != 0)
            {
                tB_AverageMontlyCalories.Text = CurrentManager.StatManager.AverageMonthtlyCalories.ToString();
            }
            else
            {
                tB_AverageMontlyCalories.Text = TranslatorCore.GetNoDataLabel(ActiveLanguage);
            }

        }

        //Opens panel for searching in cooked recipes.
        private void ShowCookedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Cooked Recipes";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Visible;
        }

        //Opens panel to see recently viewed recipes.
        private void ShowRecentlyViewedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Recently Viewed";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Visible;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Hidden;
            SetRecentlyViewedImages();
        }

        //Handles showing the user what recipes he has recently viewed. This is done by showing images.(Tool tips will be added showing the name of the recipe)
        private void SetRecentlyViewedImages()
        {
            if (CurrentManager.UserRecipeSaves.USRecentlyViewed.Count > 0)
            {
                for (int i = 0; i < CurrentManager.UserRecipeSaves.USRecentlyViewed.Count; i++)
                {
                    if (i <= 6 && CurrentManager.RecipieManag.Recipies.ContainsKey(CurrentManager.UserRecipeSaves.USRecentlyViewed[i]))
                    {
                        if (File.Exists(System.IO.Path.Combine(ImageFolderPath, CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile)))
                        {
                            RecentlyViewedRecipeImages[i].ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(ImageFolderPath, CurrentManager.RecipieManag.Recipies[CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile), UriKind.Relative));
                            RecentlyViewedRecipesImages[i].ToolTip = GetRecentlyViewedList()[i];
                            RecentlyViewedRecipesImages[i].Fill = RecentlyViewedRecipeImages[i];
                            if (i <= 4)
                            {
                                FrontPageRecentlyViewedImages[i].ToolTip = GetRecentlyViewedList()[i];
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

        //Opens panel for searching in saved recipes.
        private void ShowAllSavedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = "Saved Recipes";
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Visible;
            P_CookedRecipes.Visibility = Visibility.Hidden;
        }

        /// Function will be used at a late date.
        private void ShowRecipesCookedToday()
        {
            //  tB_SavedRecipesPanelTitle.Text = "Cooked Today";
            //  P_CookedTodayRecipes.Visibility = Visibility.Visible;
            //  P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            //  P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            //  P_CookedRecipes.Visibility = Visibility.Hidden;
        }
        #endregion


        #region Fridge related functions
        #region Searching and Adding/Removing food items
        //Handles searching for fooditems. Function is called when the text in the FoodSearch textbox is changed
        private void SearchFoodItems()
        {
            // Checks if the search box is null or not.
            if (!string.IsNullOrWhiteSpace(tb_FoodSearch.Text))
            {
                //Makes sure that text is not the keyword "Search"
                if (tb_FoodSearch.Text != TranslatorCore.GetTextboxDefaultText(ActiveLanguage))
                {
                    l_SearchInfo.Text = TranslatorCore.GetClickToAddFoodMessage(ActiveLanguage);
                    string searchedWord = tb_FoodSearch.Text;
                    string ToLower = searchedWord.ToLower();

                    foreach (KeyValuePair<string, FoodDef> element in CurrentManager.FoodManag.Foods)
                    {
                        switch (ActiveLanguage)
                        {
                            case Languages.English:
                                if (element.Key.StartsWith(ToLower) && !lB_SuggestedFoods.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString()))
                                {
                                    lB_SuggestedFoods.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString());
                                    ItemsInFoodSearch.Add(element.Value.USName);
                                }
                                break;

                            case Languages.Bulgarian:
                                if (element.Value.BGName.StartsWith(ToLower) && !lB_SuggestedFoods.Items.Contains(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString()))
                                {
                                    lB_SuggestedFoods.Items.Add(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString());
                                    ItemsInFoodSearch.Add(element.Value.USName);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                lB_SuggestedFoods.Items.Clear();
                ItemsInFoodSearch.Clear();
                l_SearchInfo.Text = TranslatorCore.GetTypeForFoodPrompt(ActiveLanguage);
            }
        }

        //Handles adding a food item. 
        private void AddFoodItem()
        {
            //Items in food search has only english values(aka string corresponding to keys in the Foods dictionary) when selecting an element
            // in the Suggested foods textbox the user selects and "index" of the text box. The element at that index corresponds to and element in the ItemsInFoodSearch
            // list. The selected index is used to get that element and then that element is used to add the correct item to the user's fridge.
            // This extra list is required due to the bilingual nature of the application.
            FoodDef ItemToAdd = new FoodDef();
            if (lB_SuggestedFoods.SelectedIndex > 0)
            {
                ItemToAdd = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]];
            }

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
                L_FoodAmountWarning.Text = TranslatorCore.FoodAmountNullWarning(ActiveLanguage);
            }

            if (ItemToAdd.USName != null)
            {
                if (!CurrentManager.User.UserFridge.USUsersFoods.ContainsKey(ItemToAdd.USName))
                {
                    CurrentManager.User.UserFridge.AddToFridge(ItemToAdd);

                    switch (ActiveLanguage)
                    {
                        case Languages.English:
                            lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.USName.First().ToString().ToUpper() + ItemToAdd.USName.ToString().Substring(1), Amount = ItemToAdd.Amount });
                            break;
                        case Languages.Bulgarian:
                            lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.BGName, Amount = ItemToAdd.Amount });

                            break;
                        default:
                            break;
                    }

                    CurrentManager.UsersFridge.SaveFridge();
                    l_SearchInfo.Text = TranslatorCore.ItemAddedMessage(ActiveLanguage);

                    RefreshFridge();
                    PopulateFridgeSummary();
                }
                else
                {
                    l_SearchInfo.Text = TranslatorCore.ItemAlreadyInFridgeMessage(ActiveLanguage);
                }
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

        //Handles removing a food item.
        private void RemoveItem()
        {
            //Removing a food items works the same way as adding an item but in reverse.
            if (lb_FoodList.SelectedItem != null)
            {
                FoodDef ItemToRemove = CurrentManager.FoodManag.Foods[ItemsFoodList[lb_FoodList.SelectedIndex].ToLower()];
                CurrentManager.User.UserFridge.RemoveFromFridge(ItemToRemove.USName.ToLower());
                lb_FoodList.Items.Remove(lb_FoodList.SelectedIndex);
                lb_FoodList.Items.Clear();
                CurrentManager.User.UserFridge.SaveFridge();
                RefreshFridge();
                PopulateFridgeSummary();
            }
        }
        #endregion

        #region Refreshing and updating.
        //Refreshes the items in the fride view that is on the front page.
        private void RefreshFrontPageFride()
        {
            lb_Fridge.Items.Clear();
            foreach (KeyValuePair<string, FoodDef> element in CurrentManager.User.UserFridge.USUsersFoods)
            {
                switch (ActiveLanguage)
                {
                    case Languages.English:
                        if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1)))
                        {
                            lb_Fridge.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1));
                        }
                        break;
                    case Languages.Bulgarian:
                        if (lb_Fridge.Items.Count < 10 && !lb_Fridge.Items.Contains(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1)))
                        {
                            lb_Fridge.Items.Add(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1));
                        }
                        break;
                }
            }
        }

        //Refreshes the main fridge in the "FridgePanel" This function is called after adding or removing an item or when the language is changed.
        private void RefreshFridge()
        {
            lb_FoodList.Items.Clear();
            ItemsFoodList.Clear();
            foreach (KeyValuePair<string, FoodDef> ItemToAdd in CurrentManager.UsersFridge.USUsersFoods)
            {
                ItemsFoodList.Add(ItemToAdd.Key.ToLower());

                if (!lb_FoodList.Items.Contains(ItemToAdd.Value.USName) || !lb_FoodList.Items.Contains(ItemToAdd.Value.BGName))
                {
                    switch (ActiveLanguage)
                    {
                        case Languages.English:
                            lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.Value.USName.First().ToString().ToUpper() + ItemToAdd.Value.USName.Substring(1), Amount = ItemToAdd.Value.Amount });
                            break;

                        case Languages.Bulgarian:
                            lb_FoodList.Items.Add(new FoodDef() { USName = ItemToAdd.Value.BGName.First().ToString().ToUpper() + ItemToAdd.Value.BGName.Substring(1), Amount = ItemToAdd.Value.Amount });
                            break;
                    }
                }
            }

            lb_FoodList.Items.Refresh();
            RefreshFrontPageFride();
        }
        #endregion

        #region Configuring item amounts.       
        //Handles adding an amount to the fooditem the use wants to add.
        private void ConfigureClickedItem()
        {
            if (lB_SuggestedFoods.SelectedItem != null)
            {
                TB_AddedFoodNameItem.Text = lB_SuggestedFoods.SelectedItem.ToString().First().ToString().ToUpper() + lB_SuggestedFoods.SelectedItem.ToString().Substring(1);

                foreach (RadioButton checkBox in AmountRadioButtons)
                {
                    if (Array.IndexOf(AmountRadioButtons, checkBox) < CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].SuggestedAmounts.Count)
                        checkBox.Content = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].SuggestedAmounts[Array.IndexOf(AmountRadioButtons, checkBox)];
                }

                switch (ActiveLanguage)
                {
                    case Languages.English:
                        Tb_UOMLabel.Text = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].USUOM;

                        break;
                    case Languages.Bulgarian:
                        Tb_UOMLabel.Text = CurrentManager.FoodManag.Foods[ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].BGUOM;

                        break;
                }
            }
        }
        #endregion

        #region Fridge infromation management
        //Updates the content related to fridge statistics on the FridgePanel
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
                SummaryValues = new float[] { CalorieSum, ProteinSum, FatSum, CarbSum, SugarSum, SodiumSum };
                for (int i = 0; i < SummaryTextBlocks.Length; i++)
                {
                    SummaryTextBlocks[i].Text = SummaryValues[i].ToString();
                }
            }
            else
            {
                SummaryValues = new float[] { CalorieSum, ProteinSum, FatSum, CarbSum, SugarSum, SodiumSum };
                for (int i = 0; i < SummaryTextBlocks.Length; i++)
                {
                    SummaryTextBlocks[i].Text = SummaryValues[i].ToString();
                }
            }
        }

        //Handles showing the nutritional information about the item the user has selected.
        private void GetAndShowFoodInfo()
        {
            if (lb_FoodList.SelectedItem != null)
            {
                FoodDef SelectedItem = CurrentManager.FoodManag.Foods[ItemsFoodList[lb_FoodList.SelectedIndex].ToLower()];
                tb_FoodName.Text = SelectedItem.USName.First().ToString().ToUpper() + SelectedItem.USName.Substring(1);
                tb_FoodItemCalorie.Text = SelectedItem.Calories.ToString();
                tB_FoodProtein.Text = SelectedItem.Protein.ToString() + " " + "g";
                tB_FoodFat.Text = SelectedItem.Fat.ToString() + " " + "g";
                tB_FoodCarbs.Text = SelectedItem.Carbs.ToString() + " " + "g";
                tB_FoodSugar.Text = SelectedItem.Sugars.ToString() + " " + "g";
                tB_FoodSodium.Text = SelectedItem.Sodium.ToString() + " " + "g";
            }
            else
            {
                tb_FoodItemCalorie.Text = "0";
                tB_FoodProtein.Text = "0";
                tB_FoodFat.Text = "0";
                tB_FoodCarbs.Text = "0";
                tB_FoodSugar.Text = "0";
                tB_FoodSodium.Text = "0";
                tb_FoodName.Text = " ";
            }
        }
        #endregion
        #endregion


        #region User settings functions
        //Updates information on the UsersSettings panel
        private void UpdateSettingPanel()
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

            if (CurrentManager.User.Sex == "male")
                rb_Male.IsChecked = true;

            if (CurrentManager.User.Sex == "female")
                rb_Female.IsChecked = true;

            if (CurrentManager.User.Sex == "other")
                rb_Other.IsChecked = true;
        }

        //Handles saving the user settings. All values on the settings panel are set to the corresponding properties of the User class in the CurrentManager.
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
                ActiveLanguage = Languages.Bulgarian;
                CurrentManager.User.LanguagePref = "BG";
                Localizer.SwitchLanguage(this, "bg-BG");
            }

            if (CB_English.IsChecked == true)
            {
                ActiveLanguage = Languages.English;
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

            SuggestedRecipe = new RecipeDef();
            img_RecipeImage.Fill = Brushes.White;
            RecipeIngredientList = new List<FoodDef>();
            lv_Ingredients.ItemsSource = RecipeIngredientList;

            CurrentManager.SaveUser();
            CurrentManager.User.CalculateIndex();
            CurrentManager.SaveUser();
            tB_UserName.Text = CurrentManager.User.UserName;
            CurrentManager.RecipieManag.SortRecipes();
            SuggestRecipe();


            tB_Directions.Text = null;
            tB_RecipeTitle.Text = null;
            tB_TimeToCookAmount.Text = null;
        }
        #endregion
        #endregion


        // ================= UI EVENTS =================
        #region UI Event functions
        #region Fridge related
        //Called on foodlist selection changed. Handles showing fooditem info.
        private void ShowFoodInfo(object sender, SelectionChangedEventArgs e)
        {
            GetAndShowFoodInfo();
        }

        //Removes Item from fridge.
        private void Btn_RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem();
        }

        //Updates fridge on fridge open.
        private void Btn_Showfridge_Click(object sender, RoutedEventArgs e)
        {
            RefreshFridge();
        }

        //Changes text of the FoodSearch textbox when it looses focus.
        private void FoodSearchLostFocus(object sender, RoutedEventArgs e)
        {
            tb_FoodSearch.Text = TranslatorCore.GetTextboxDefaultText(ActiveLanguage);
        }

        //Removes "Search" text fomr FoodSearch textbox when it gets focus
        private void SearchFoodClearTextBox(object sender, RoutedEventArgs e)
        {
            tb_FoodSearch.Text = null;
        }

        //Handles adding items to fridge.
        private void AddClickedItem(object sender, SelectionChangedEventArgs e)
        {
            ConfigureClickedItem();
        }

        //Handles searching for fooditems. Function called everytime the text in the FoodSearch textbox is chang ed.
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchFoodItems();
        }

        //Adds the configured food item to the fridge.
        private void AddFoodItemClick(object sender, RoutedEventArgs e)
        {
            AddFoodItem();
        }
        #endregion

        #region Recipe related
        //Handles discovering of recipes.
        private void Btn_DiscoverClick(object sender, RoutedEventArgs e)
        {

        }

        //Handles suggesting a recipe.
        private void Btn_FindRecipe_Click(object sender, RoutedEventArgs e)
        {
            SuggestRecipe();
        }

        //Handles opening full recipe view and updating information in it.
        private void Btn_ShowRecipе(object sender, RoutedEventArgs e)
        {
            AddInformationToFullRecipeView();
            SetupFullRecipeViewImg();
        }

        //Handles opening favourite recipe panel.
        private void Btn_SeeAllSavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ConfigureSavedRecipesPanel();
            SetRecentlyViewedImages();
        }

        //Handles showing next recipe to the user.
        private void Btn_ShowNextRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Breakfast.Count || NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Lunch.Count || NumerOfRecipeToSuggest < CurrentManager.RecipieManag.Dinner.Count)
            {
                tB_RecipeName.FontSize = 24;
                NumerOfRecipeToSuggest++;
                SuggestRecipe();
                SetRecentlyViewedImages();
            }
        }

        //Handles Showing previour recipe  to the user.
        private void Btn_ShowPreviousRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (NumerOfRecipeToSuggest >= 0)
            {
                tB_RecipeName.FontSize = 24;
                NumerOfRecipeToSuggest--;
                SuggestRecipe();
            }
        }

        //Handles opening of the Cooked Recipes search panel.
        private void Btn_CookedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowCookedRecipes();
        }

        //Handles opening the recently viewed page.
        private void Btn_RecentlyViewed_Click(object sender, RoutedEventArgs e)
        {
            ShowRecentlyViewedRecipes();
            SetRecentlyViewedImages();
        }

        //Handles opening Saved Recipes search panel.
        private void Btn_SavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            ShowAllSavedRecipes();
        }

        //Function not in use yet.
        private void Btn_CookedToday_Click(object sender, RoutedEventArgs e)
        {
            //ShowRecipesCookedToday();
        }

        //Handles adding the recipe to the Cooked Recipes list.
        private void Btn_RecipeWillBeCooked_Click(object sender, RoutedEventArgs e)
        {
            AddToCookedRecipes();
        }

        // Cooked Recipes

        //Handles searching in the cooked recipes list. Function is called when the text in the textbox for searching in cooked recipes changes.
        private void CookedRecipesTextChanged(object sender, TextChangedEventArgs e)
        {
            CookedRecipesSearch();
        }

        //Clears text when the search coooked recipes textbox gets focus.
        private void SearchCookedRecipesFocused(object sender, RoutedEventArgs e)
        {
            tb_SearchCookedRecipes.Text = null;
        }

        //Resets text when the search cooked recipe textbox looses focus. Text is determined by the language that is being used.
        private void SearchCookedRecipesLostFocus(object sender, RoutedEventArgs e)
        {
            tb_SearchCookedRecipes.Text = TranslatorCore.GetTextboxDefaultText(ActiveLanguage);
        }

        //Saved Recipes

        //Handles searching in the saved recipes list. Function is called when the text in the textbox for searching in saved recipes changes.
        private void SearchSavedRecipesTextChanged(object sender, TextChangedEventArgs e)
        {
            SavedRecipesSearch();
        }

        //Clears text when the search saved recipes textbox gets focus.
        private void SearchSavedRecipesFocused(object sender, RoutedEventArgs e)
        {
            tb_SearchSavedRecipes.Text = null;
        }

        //Resets text when the search saved recipe textbox looses focus. Text is determined by the language that is being used.
        private void SavedRecipeSearchLostFocus(object sender, RoutedEventArgs e)
        {
            tb_SearchSavedRecipes.Text = TranslatorCore.GetTextboxDefaultText(ActiveLanguage);
        }

        //Handles adding the recipe to the saved recipe list.
        private void BTN_AddToSavedRecipes_Click(object sender, RoutedEventArgs e)
        {
            AddToSavedReicpe();
        }
        #endregion

        #region User related
        //Calls function to save user settings
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveUserSettings();
        }

        //Updates setting panel information on panel show.
        private void ShowSettings(object sender, MouseButtonEventArgs e)
        {
            UpdateSettingPanel();
        }

        //Allows only english checkbox to be checked.
        private void UncheckEnglish(object sender, RoutedEventArgs e)
        {
            CB_English.IsChecked = false;
        }

        //Allows only bulgarian checkbox to be checked.
        private void UncheckBulgarian(object sender, RoutedEventArgs e)
        {
            CB_Bulgarian.IsChecked = false;
        }
        #endregion
        #endregion

        #region Application translation handling
        //Handles changing languages.
        private void ChangeLanguage(object sender, RoutedEventArgs e)
        {
            string LocaleCode = (string)((Button)sender).Tag;
            Localizer.SwitchLanguage(this, LocaleCode);
            if (LocaleCode == "bg-BG")
            {
                ActiveLanguage = Languages.Bulgarian;
                tB_RecipeName.Text = SuggestedRecipe.BGName;
            }

            if (LocaleCode == "en-US")
            {
                ActiveLanguage = Languages.English;
                tB_RecipeName.Text = SuggestedRecipe.USName;
            }

            AddInformationToFullRecipeView();
            RefreshFridge();
        }

        //Gets appropriate Saved recipe list.
        private List<string> GetSavedRecipesList()
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return CurrentManager.UserRecipeSaves.USSavedRecipes;
                    }
                case Languages.Bulgarian:
                    {
                        return CurrentManager.UserRecipeSaves.BGSavedRecipes;
                    }

                default:
                    {
                        return new List<string>();
                    }
            }

        }

        //Gets appropriate list for recently viewed recipes name based on language setting.
        private List<string> GetRecentlyViewedList()
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return CurrentManager.UserRecipeSaves.USRecentlyViewed;
                    }
                case Languages.Bulgarian:
                    {
                        return CurrentManager.UserRecipeSaves.BGRecentlyViewed;
                    }

                default:
                    {
                        return new List<string>();
                    }
            }
        }

        //Gets approprite directions string based on language setting.
        private string GetDirections(RecipeDef recipe)
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return recipe.USDirections;
                    }
                case Languages.Bulgarian:
                    {
                        return recipe.BGDirections;
                    }

                default:
                    {
                        return " ";
                    }
            }
        }

        //Gets appropriate igredients list.
        private List<string> GetIngredientList(RecipeDef recipe)
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return recipe.USIngredients;
                    }
                case Languages.Bulgarian:
                    {
                        return recipe.BGIngredients;
                    }

                default:
                    {
                        return new List<string>();
                    }
            }

        }

        //Gets appropriate Cooked recipe list.
        private List<string> GetCookedRecipeList()
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return CurrentManager.UserRecipeSaves.USCookedRecipes;
                    }
                case Languages.Bulgarian:
                    {
                        return CurrentManager.UserRecipeSaves.BGCookedRecipes;
                    }

                default:
                    {
                        return new List<string>();
                    }
            }
        }

        //Gets appropriate Recipe name.
        private string GetSuggestedRecipeName(RecipeDef recipe)
        {
            switch (ActiveLanguage)
            {
                case Languages.English:

                    {
                        return recipe.USName;
                    }
                case Languages.Bulgarian:
                    {
                        return recipe.BGName;
                    }

                default:
                    {
                        return " ";
                    }
            }
        }
        #endregion
    }
}