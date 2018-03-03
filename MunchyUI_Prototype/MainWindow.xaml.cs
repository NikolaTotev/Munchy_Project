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
    public enum Languages
    {
        English,
        Bulgarian
    }

    public enum ActiveFoodsearch
    {
        Fridge,
        ShoppingList
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Getting user applicaitondata folder.
        static string m_LocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string m_ProgramFolder = System.IO.Path.Combine(m_LocalAppDataPath, "Munchy");
        static string m_ImageFolderPath = System.IO.Path.Combine(m_ProgramFolder, "Images");

        //Data File Locations
        string m_UserFile = System.IO.Path.Combine(m_ProgramFolder, "USER.json");

        string m_UserFridgeFile = System.IO.Path.Combine(m_ProgramFolder, "USER_FRIDGE.json");

        string m_FoodDefFile = System.IO.Path.Combine(m_ProgramFolder, "FoodData.json");
        string m_RecipeDatabase = System.IO.Path.Combine(m_ProgramFolder, "Recipes.json");

        string m_RecipeSaveFile = System.IO.Path.Combine(m_ProgramFolder, "RecipeSavesFile.json");
        string m_StatSavePath = System.IO.Path.Combine(m_ProgramFolder, "StatSavePath.json");
        string m_ShoppingListFile = System.IO.Path.Combine(m_ProgramFolder, "ShoppingList.json");

        //Checks what food search is open.
        ActiveFoodsearch m_ActiveFoodsearch;

        //Enum for determining what language to use in runtime. English is the default language.        
        Languages m_ActiveLanguage = Languages.English;
        //Variables that are used to display the fridge summary.
        float m_CalorieSum = 0;
        float m_ProteinSum = 0;
        float m_FatSum = 0;
        float m_CarbSum = 0;
        float m_SugarSum = 0;
        float m_SodiumSum = 0;

        //This textblock array is used to store all the summary textboxes. This enables easy population of said textboxes.
        TextBlock[] m_SummaryTextBlocks;

        //This float array stores the values mentioned above. It allows for fast population of the Textblock array
        float[] m_SummaryValues;

        //Integer that keeps track of daily calories the user has consumed.
        int m_DailyCalories = 0;

        //Integer that keeps track of what recipe the user is on. This integer is responsible for browsing through the suggested recipe list.
        int m_NumerOfRecipeToSuggest = 0;

        //A list of checkboxes that are used for saving the users settings and preferences
        List<CheckBox> m_SettingOptions;

        //This list is used to keep track of the elements in the SuggestedFoods listbox.
        List<string> m_ItemsInFoodSearch = new List<string>();

        //This list is used to keep track of the elements in the users fridge.
        List<string> m_ItemsFoodList = new List<string>();

        // This list is used for populating the Ingredients ListView in the UI.
        List<FoodDef> m_RecipeIngredientList;

        //This is the RecipeDef item that is used to store the user is supposed to see. It is updated everytime the user  wants to go to the next recipe
        // in the sugggested recipe list.
        RecipeDef m_SuggestedRecipe = new RecipeDef();

        //This is the main brain of the application. It is the interface between the UI and back end.
        ProgramManager m_CurrentManager;

        //RecipeImage handles image of the ful recipe view.
        ImageBrush m_RecipeImage = new ImageBrush();

        //SuggestedRecipeImage handles showing the image of the recipe on the front page.
        ImageBrush m_SuggestedRecipeImage = new ImageBrush();

        //These brushes are used for loading recently viewed recipes. Each is assigned to a different elipse on the UI.
        ImageBrush m_RecentRecipe_1 = new ImageBrush();
        ImageBrush m_RecentRecipe_2 = new ImageBrush();
        ImageBrush m_RecentRecipe_3 = new ImageBrush();
        ImageBrush m_RecentRecipe_4 = new ImageBrush();
        ImageBrush m_RecentRecipe_5 = new ImageBrush();
        ImageBrush m_RecentRecipe_6 = new ImageBrush();

        //This imagebrush array stores the RecentRecipe brushes.
        ImageBrush[] m_RecentlyViewedRecipeImages;

        //This Elipse array stores the elipses on the "SavedRecipes" panel.
        Ellipse[] m_RecentlyViewedRecipesImages;

        //This elipse array stores the elipses on the front page.
        Ellipse[] m_FrontPageRecentlyViewedImages;

        //These RadioButtons are used for setting the amounts of the fooditems the user adds.
        RadioButton[] m_AmountRadioButtons;

        // ================= UI LOGIC =================
        #region UI Logic

        #region Initialization functions
        //Main window initialization
        public MainWindow()
        {
            InitializeComponent();

            //Checks if FoodDefFile and RecipeDatabase exist, if they dont a message box is shown and the user is redirected to the Support page.
            //This prevents operation errors down the line. These two files are critical for the operation of the program.
            if (!File.Exists(m_FoodDefFile) || !File.Exists(m_RecipeDatabase))
            {
                MessageBox.Show("EN: The program seems to have encountered an error Press OK to open the support page and look for the following code : Error Code: ERR-RFM " + "\n" + "\n" + " BG: Грешка! : Липсват файлове нужни за функционирането на програмата.Моля посетете https://github.com/ProjectMunchy/Munchy/wiki/Troubleshooting за насоки да поправите грешката.");
                System.Diagnostics.Process.Start("https://github.com/NikolaTotev/Munchy/wiki/Toubleshooting#fix-instructions");
                Close();
                return;
            }

            //Checks if the UserFile exists. If it doesn't this suggests that this is the first time the user is using the program and therefore he is prompted to
            //enter his information. The information is used to suggest recipes.
            if (!File.Exists(m_UserFile))
            {
                MessageBox.Show("It seems like your user file is empty. Take a moment to fill in some of your details. This will help Munchy suggest recipes exactly to your tastes." + "\n" + "\n" + "Изглежда че вашия личен файл е празен. Отделете няколко минути да попълните информация за вашите предпочитания. Това ще помогне на програмата да предляга подходящи за вас рецепти.");
                tB_UserName.Text = "Click me/Кликни ме";
                Rect_UNameBackground.Stroke = Brushes.Red;
            }

            //Intitially sets the default language.
            Localizer.SetDefaultLanguage(this);

            m_CurrentManager = new ProgramManager(m_UserFile, m_UserFridgeFile, m_RecipeDatabase, m_FoodDefFile, m_RecipeSaveFile, m_StatSavePath, m_ShoppingListFile);

            //Sets the users name.
            if (m_CurrentManager.User.UserName != null)
                tB_UserName.Text = m_CurrentManager.User.UserName;

            //Gets the language that the user has saved. It does not check if it is english as english is the default for the program.
            if (m_CurrentManager.User.LanguagePref == "BG")
            {
                m_ActiveLanguage = Languages.Bulgarian;
                Localizer.SwitchLanguage(this, "bg-BG");
            }

            m_SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };
            m_SummaryTextBlocks = new TextBlock[] { tB_CalorieSummary, tB_ProteinSummary, tB_FatSummary, tB_CarbsSummary, tB_SugarSumary, tB_SodiumSummary };
            m_RecentlyViewedRecipeImages = new ImageBrush[] { m_RecentRecipe_1, m_RecentRecipe_2, m_RecentRecipe_3, m_RecentRecipe_4, m_RecentRecipe_5, m_RecentRecipe_6 };
            m_RecentlyViewedRecipesImages = new Ellipse[] { Img_RecentlyViewed_1, Img_RecentlyViewed_2, Img_RecentlyViewed_3, Img_RecentlyViewed_4, Img_RecentlyViewed_5, Img_RecentlyViewed_6 };
            m_FrontPageRecentlyViewedImages = new Ellipse[] { Img_FrontRecentlyViewed_1, Img_FrontRecentlyViewed_2, Img_FrontRecentlyViewed_3, Img_FrontRecentlyViewed_4, Img_FrontRecentlyViewed_5 };
            m_AmountRadioButtons = new RadioButton[] { Cb_SuggestedAmount_1, Cb_SuggestedAmount_2, Cb_SuggestedAmount_3, Cb_SuggestedAmount_4 };
            m_RecipeIngredientList = new List<FoodDef>();
            InitialFridgeUISetup();
            PopulateFridgeSummary();
            SuggestRecipe();
            SetRecentlyViewedImages();

            m_CurrentManager.StatManager.DailyReset();
            m_DailyCalories = m_CurrentManager.StatManager.DailyCalories;
            L_DailyCalories.Text = m_DailyCalories.ToString();
        }

        //On start populates the fridge on the main page with 10 items from the UserFridge.
        private void InitialFridgeUISetup()
        {
            if (m_CurrentManager.User.UserFridge.USUsersFoods != null && m_CurrentManager.User.UserFridge.USUsersFoods.Count > 0)
            {
                RefreshFridge();
                foreach (KeyValuePair<string, FoodDef> element in m_CurrentManager.User.UserFridge.USUsersFoods)
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
                if (m_CurrentManager.RecipieManag.Breakfast.Count > 0 && m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Breakfast.Count && m_NumerOfRecipeToSuggest >= 0)
                {
                    m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[m_CurrentManager.RecipieManag.Breakfast[m_NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(m_SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(m_SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(m_SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(m_ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //Lunch.
            if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17)
            {
                if (m_CurrentManager.RecipieManag.Lunch.Count > 0 && m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Lunch.Count && m_NumerOfRecipeToSuggest >= 0)
                {
                    m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[m_CurrentManager.RecipieManag.Lunch[m_NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(m_SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(m_SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(m_SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(m_ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //Dinner.
            if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 24)
            {
                if (m_CurrentManager.RecipieManag.Dinner.Count > 0 && m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Dinner.Count && m_NumerOfRecipeToSuggest >= 0)
                {
                    m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[m_CurrentManager.RecipieManag.Dinner[m_NumerOfRecipeToSuggest]];
                    tB_RecipeName.Text = GetSuggestedRecipeName(m_SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(m_SuggestedRecipe).ToString().Substring(1);
                    ManageSuggestedRecipe(m_SuggestedRecipe);
                }
                else
                {
                    tB_RecipeName.Text = null;
                    tB_RecipeName.FontSize = 18;
                    tB_RecipeName.Text = TranslatorCore.GetSuggestedRecipeInfo(m_ActiveLanguage);
                    Img_SuggestedRecipeImage.Fill = Brushes.White;
                }
            }

            //No eating time.
            if (DateTime.Now.Hour < 7 && DateTime.Now.Hour <= 24)
            {
                tB_RecipeName.Text = null;
                tB_RecipeName.FontSize = 18;
                tB_RecipeName.Text = TranslatorCore.GetTooLateToEatMessage(m_ActiveLanguage);
                Img_SuggestedRecipeImage.Fill = Brushes.White;
            }

            m_CurrentManager.UserRecipeSaves.SaveRecipeSaver();
        }

        //Handles the suggested recipe. Updates the recently viewed list and adds the recipe to it, handles images of recently viewed recipes.
        private void ManageSuggestedRecipe(RecipeDef inputrecipe)
        {
            RecipeSaver saver = m_CurrentManager.UserRecipeSaves;

            if (saver.USRecentlyViewed.Count < 6 && saver.BGRecentlyViewed.Count < 6)
            {
                AddToList(saver.USRecentlyViewed, m_SuggestedRecipe.USName);
                AddToList(saver.BGRecentlyViewed, m_SuggestedRecipe.BGName);
                m_CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }
            else
            {
                if (!saver.USRecentlyViewed.Contains(m_SuggestedRecipe.USName.ToLower()))
                {
                    saver.USRecentlyViewed.RemoveAt(5);
                    saver.USRecentlyViewed.Insert(0, m_SuggestedRecipe.USName.ToLower());
                }

                if (!saver.BGRecentlyViewed.Contains(m_SuggestedRecipe.BGName.ToLower()))
                {
                    saver.BGRecentlyViewed.RemoveAt(5);
                    saver.BGRecentlyViewed.Insert(0, m_SuggestedRecipe.BGName.ToLower());
                }
                m_CurrentManager.UserRecipeSaves.SaveRecipeSaver();
            }

            if (File.Exists(System.IO.Path.Combine(m_ImageFolderPath, m_SuggestedRecipe.ImageFile)))
            {
                m_SuggestedRecipeImage.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(m_ImageFolderPath, m_SuggestedRecipe.ImageFile), UriKind.Relative));
                Img_SuggestedRecipeImage.Fill = m_SuggestedRecipeImage;
            }
            else
            {
                Img_SuggestedRecipeImage.Fill = null;
                MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                System.Diagnostics.Process.Start("https://github.com/NikolaTotev/Munchy/wiki/Toubleshooting#fix-instructions");
            }

            m_CurrentManager.StatManager.TotalRecipesSeen++;
            m_CurrentManager.StatManager.SaveStatistics();
        }

        //Sets up the initial FullRecipe view. Called when the "Show Recipe" button is pressed.
        private void SetupFullRecipeViewImg()
        {
            if (m_SuggestedRecipe != null && m_CurrentManager.RecipieManag.CompatableRecipes.Count > 0 && m_SuggestedRecipe.ImageFile != null)
            {
                if (File.Exists(System.IO.Path.Combine(m_ImageFolderPath, m_SuggestedRecipe.ImageFile)))
                {
                    m_RecipeImage.ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(m_ImageFolderPath, m_SuggestedRecipe.ImageFile), UriKind.Relative));
                    img_RecipeImage.Fill = m_RecipeImage;
                }
                else
                {
                    img_RecipeImage.Fill = null;
                    MessageBox.Show("You are missing Image files please Click OK to open the support page to reslove the issue. Look for error code : ERR-ImgFM");
                    System.Diagnostics.Process.Start("https://github.com/NikolaTotev/Munchy/wiki/Toubleshooting#fix-instructions");
                }
            }

            if (m_CurrentManager.UsersFridge.FridgeConatains(m_SuggestedRecipe.USIngredients, m_SuggestedRecipe.Amounts, m_SuggestedRecipe.Units, m_CurrentManager.FoodManag))
            {
                Tb_IngrMessageTitle.Foreground = Brushes.Green;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForAllIngrPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForAllIngrPresent(m_ActiveLanguage);
            }
            else
            {
                Tb_IngrMessageTitle.Foreground = Brushes.Red;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForIngrNotPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForIngrNotPresent(m_ActiveLanguage);
            }

        }

        //Adds information about the recipe to the FullRecipeView panel.
        private void AddInformationToFullRecipeView()
        {
            m_RecipeIngredientList.Clear();

            if (GetIngredientList(m_SuggestedRecipe) != null && GetIngredientList(m_SuggestedRecipe).Count > 0)
            {
                foreach (string ingredient in GetIngredientList(m_SuggestedRecipe))
                {
                    if (GetIngredientList(m_SuggestedRecipe).Count == m_SuggestedRecipe.Units.Count)
                    {
                        m_RecipeIngredientList.Add(new FoodDef() { USName = ingredient, IngrAmount = m_SuggestedRecipe.Amounts[GetIngredientList(m_SuggestedRecipe).IndexOf(ingredient)].ToString() + " " + TranslatorCore.GetUnit(m_ActiveLanguage, m_SuggestedRecipe.Units[GetIngredientList(m_SuggestedRecipe).IndexOf(ingredient)].ToString()) });
                    }

                    else
                    {
                        MessageBox.Show("You seem to have a probelem with the Recipe File. Press OK to open the support page.");
                        System.Diagnostics.Process.Start("https://github.com/NikolaTotev/Munchy/wiki/Toubleshooting#fix-instructions");
                        break;
                    }
                }

                lv_Ingredients.ItemsSource = m_RecipeIngredientList;
                lv_Ingredients.Items.Refresh();
                tB_Directions.Text = GetDirections(m_SuggestedRecipe);
                tB_RecipeTitle.Text = GetSuggestedRecipeName(m_SuggestedRecipe).First().ToString().ToUpper() + GetSuggestedRecipeName(m_SuggestedRecipe).ToString().Substring(1);
                tB_TimeToCookAmount.Text = m_SuggestedRecipe.TimeToCook.ToString();
            }
        }

        //Handles adding the suggested to the CookedRecipes list. A recipe is added only when the user pressed the "I'll Cook It" Button.
        private void AddToCookedRecipes()
        {
            if (m_CurrentManager.UsersFridge.FridgeConatains(m_SuggestedRecipe.USIngredients, m_SuggestedRecipe.Amounts, m_SuggestedRecipe.Units, m_CurrentManager.FoodManag))
            {
                RecipeSaver saver = m_CurrentManager.UserRecipeSaves;
                AddToList(saver.USCookedRecipes, m_SuggestedRecipe.USName);
                AddToList(saver.BGCookedRecipes, m_SuggestedRecipe.BGName);

                m_CurrentManager.UserRecipeSaves.SaveRecipeSaver();
                m_CurrentManager.StatManager.AddToCalorieStatistics(m_SuggestedRecipe.Calories);
                m_DailyCalories = m_CurrentManager.StatManager.DailyCalories;
                L_DailyCalories.Text = m_DailyCalories.ToString();
                m_CurrentManager.UsersFridge.ModifyFoodItemAmount(m_SuggestedRecipe.USIngredients, m_SuggestedRecipe.Amounts, m_SuggestedRecipe.Units, m_CurrentManager.FoodManag);
                RefreshFridge();
                Tb_IngrMessageTitle.Foreground = Brushes.Green;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForAllIngrPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForAllIngrPresent(m_ActiveLanguage);
            }
            else
            {
                Tb_IngrMessageTitle.Foreground = Brushes.Red;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForIngrNotPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForIngrNotPresent(m_ActiveLanguage);
            }
        }

        //Handles adding the suggested recipe to saved recipes. A recipe is saved only when the user pressed the "Save" button.
        private void AddToSavedReicpe()
        {
            RecipeSaver saver = m_CurrentManager.UserRecipeSaves;
            AddToList(saver.USSavedRecipes, m_SuggestedRecipe.USName.ToString());
            AddToList(saver.BGSavedRecipes, m_SuggestedRecipe.BGName.ToString());
        }

        //Handles adding recipes to lists.
        private void AddToList(List<string> savelist, string recipename)
        {
            if (!savelist.Contains(recipename.ToLower()))
            {
                savelist.Add(recipename.ToLower());
            }
        }

        //Shows the full recipe view of the recenly viewed recipe the user clicked on.
        private void ShowRecentlyViewedRecipe(object sender, MouseButtonEventArgs e)
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    string recipeName = (string)((Ellipse)sender).ToolTip.ToString().ToLower();
                    m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[recipeName];
                    AddInformationToFullRecipeView();
                    SetupFullRecipeViewImg();
                    break;
                case Languages.Bulgarian:
                    string rName = (string)((Ellipse)sender).ToolTip.ToString().ToLower();
                    string USRecipeName = m_CurrentManager.UserRecipeSaves.USRecentlyViewed[GetRecentlyViewedList().IndexOf(rName)];
                    m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[USRecipeName.ToLower()];
                    AddInformationToFullRecipeView();
                    SetupFullRecipeViewImg();

                    break;
            }
        }

        //Handles opening the recipe the user clicked on from saved recipes.
        private void SavedRecipesItemClicked(object sender, SelectionChangedEventArgs e)
        {
            if (lb_SavedRecipesList.SelectedItem != null)
            {
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        string recipeName = lb_SavedRecipesList.SelectedItem.ToString().ToLower();
                        m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[recipeName];
                        AddInformationToFullRecipeView();
                        SetupFullRecipeViewImg();
                        break;
                    case Languages.Bulgarian:
                        string rName = lb_SavedRecipesList.SelectedItem.ToString().ToLower();
                        string USRecipeName = m_CurrentManager.UserRecipeSaves.USSavedRecipes[GetSavedRecipesList().IndexOf(rName)];
                        m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[USRecipeName.ToLower()];
                        AddInformationToFullRecipeView();
                        SetupFullRecipeViewImg();
                        break;
                }
            }
        }

        //Handles opening the recipe the user clicked on from cooked recipes.
        private void CookedRecipeClicked(object sender, SelectionChangedEventArgs e)
        {
            if (lb_ListOfCookedRecipes.SelectedItem != null)
            {
                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        string recipeName = lb_ListOfCookedRecipes.SelectedItem.ToString().ToLower();
                        m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[recipeName];
                        AddInformationToFullRecipeView();
                        SetupFullRecipeViewImg();
                        break;
                    case Languages.Bulgarian:
                        string rName = lb_ListOfCookedRecipes.SelectedItem.ToString().ToLower();
                        string USRecipeName = m_CurrentManager.UserRecipeSaves.USCookedRecipes[GetCookedRecipeList().IndexOf(rName)];
                        m_SuggestedRecipe = m_CurrentManager.RecipieManag.Recipies[USRecipeName.ToLower()];
                        AddInformationToFullRecipeView();
                        SetupFullRecipeViewImg();
                        break;
                }
            }
        }
        #endregion

        #region Handling searching.
        //Handles searching in the SavedRecipe list.
        private void SavedRecipesSearch()
        {
            if (!string.IsNullOrWhiteSpace(tb_SearchSavedRecipes.Text))
            {
                if (tb_SearchSavedRecipes.Text != TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage))
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
                if (tb_SearchCookedRecipes.Text != TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage))
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
            m_CurrentManager.StatManager.CalculateAverageSums();
            tB_TotalCalories.Text = m_CurrentManager.StatManager.TotalCaloriesConsumed.ToString();
            tB_TotalRecipesCooked.Text = m_CurrentManager.StatManager.TotalRecipesCooked.ToString();
            tB_TotalRecipeSeen.Text = m_CurrentManager.StatManager.TotalRecipesSeen.ToString();

            if (m_CurrentManager.StatManager.AverageDailyCalories != 0)
            {
                tB_AverageDailyCalories.Text = m_CurrentManager.StatManager.AverageDailyCalories.ToString();
            }
            else
            {
                tB_AverageDailyCalories.Text = TranslatorCore.GetNoDataLabel(m_ActiveLanguage);
            }

            if (m_CurrentManager.StatManager.AverageMonthtlyCalories != 0)
            {
                tB_AverageMontlyCalories.Text = m_CurrentManager.StatManager.AverageMonthtlyCalories.ToString();
            }
            else
            {
                tB_AverageMontlyCalories.Text = TranslatorCore.GetNoDataLabel(m_ActiveLanguage);
            }

        }

        //Opens panel for searching in cooked recipes.
        private void ShowCookedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = TranslatorCore.GetCookedRecipesTitle(m_ActiveLanguage);
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Visible;
            lb_ListOfCookedRecipes.Items.Clear();

            foreach (string element in GetCookedRecipeList())
            {
                lb_ListOfCookedRecipes.Items.Add(element.First().ToString().ToUpper() + element.Substring(1));
            }
        }

        //Opens panel to see recently viewed recipes.
        private void ShowRecentlyViewedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = TranslatorCore.GetRecentlySeenTitle(m_ActiveLanguage);
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Visible;
            P_SavedRecipesSearch.Visibility = Visibility.Hidden;
            P_CookedRecipes.Visibility = Visibility.Hidden;
            SetRecentlyViewedImages();
        }

        //Handles showing the user what recipes he has recently viewed. This is done by showing images.(Tool tips will be added showing the name of the recipe)
        private void SetRecentlyViewedImages()
        {
            if (m_CurrentManager.UserRecipeSaves.USRecentlyViewed.Count > 0)
            {
                for (int i = 0; i < m_CurrentManager.UserRecipeSaves.USRecentlyViewed.Count; i++)
                {
                    if (i <= 6 && m_CurrentManager.RecipieManag.Recipies.ContainsKey(m_CurrentManager.UserRecipeSaves.USRecentlyViewed[i]))
                    {
                        if (File.Exists(System.IO.Path.Combine(m_ImageFolderPath, m_CurrentManager.RecipieManag.Recipies[m_CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile)))
                        {
                            m_RecentlyViewedRecipeImages[i].ImageSource = new BitmapImage(new Uri(System.IO.Path.Combine(m_ImageFolderPath, m_CurrentManager.RecipieManag.Recipies[m_CurrentManager.UserRecipeSaves.USRecentlyViewed[i]].ImageFile), UriKind.Relative));
                            m_RecentlyViewedRecipesImages[i].ToolTip = GetRecentlyViewedList()[i];
                            m_RecentlyViewedRecipesImages[i].Fill = m_RecentlyViewedRecipeImages[i];
                            if (i <= 4)
                            {
                                m_FrontPageRecentlyViewedImages[i].ToolTip = GetRecentlyViewedList()[i];
                                m_FrontPageRecentlyViewedImages[i].Fill = m_RecentlyViewedRecipeImages[i];
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
                        m_RecentlyViewedRecipesImages[i].Fill = null;
                        Warning_Label.Text = "There is a corrupted element in a save file! Vist the offical Munchy github for potential fixes";
                    }
                }
            }
        }

        //Opens panel for searching in saved recipes.
        private void ShowAllSavedRecipes()
        {
            tB_SavedRecipesPanelTitle.Text = TranslatorCore.GetSavedRecipesTitle(m_ActiveLanguage);
            P_CookedTodayRecipes.Visibility = Visibility.Hidden;
            P_RecenltlyViewedRecipes.Visibility = Visibility.Hidden;
            P_SavedRecipesSearch.Visibility = Visibility.Visible;
            P_CookedRecipes.Visibility = Visibility.Hidden;
            lb_SavedRecipesList.Items.Clear();

            foreach (string element in GetSavedRecipesList())
            {
                lb_SavedRecipesList.Items.Add(element.First().ToString().ToUpper() + element.Substring(1));
            }
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
            TextBox textBoxToUse = new TextBox();
            ListBox listToTarget = new ListBox();
            switch (m_ActiveFoodsearch)
            {
                case ActiveFoodsearch.Fridge:
                    textBoxToUse = tb_FoodSearch;
                    listToTarget = lB_SuggestedFoods;
                    break;
                case ActiveFoodsearch.ShoppingList:
                    textBoxToUse = Tb_AddToShoppingListSearch;
                    listToTarget = L_ShoppingListSuggestedItem;
                    break;
                default:
                    break;
            }

            // Checks if the search box is null or not.
            if (!string.IsNullOrWhiteSpace(textBoxToUse.Text))
            {
                //Makes sure that text is not the keyword "Search"
                if (textBoxToUse.Text != TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage))
                {
                    l_SearchInfo.Text = TranslatorCore.GetClickToAddFoodMessage(m_ActiveLanguage);
                    string searchedWord = textBoxToUse.Text;
                    string ToLower = searchedWord.ToLower();

                    foreach (KeyValuePair<string, FoodDef> element in m_CurrentManager.FoodManag.Foods)
                    {
                        switch (m_ActiveLanguage)
                        {
                            case Languages.English:
                                if (element.Key.StartsWith(ToLower) && !listToTarget.Items.Contains(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString()))
                                {
                                    listToTarget.Items.Add(element.Key.First().ToString().ToUpper() + element.Key.Substring(1).ToString());
                                    m_ItemsInFoodSearch.Add(element.Value.USName);
                                }
                                break;

                            case Languages.Bulgarian:
                                if (element.Value.BGName.StartsWith(ToLower) && !listToTarget.Items.Contains(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString()))
                                {
                                    listToTarget.Items.Add(element.Value.BGName.First().ToString().ToUpper() + element.Value.BGName.Substring(1).ToString());
                                    m_ItemsInFoodSearch.Add(element.Value.USName);
                                }
                                break;

                        }
                    }
                }
            }
            else
            {
                listToTarget.Items.Clear();
                m_ItemsInFoodSearch.Clear();
                l_SearchInfo.Text = TranslatorCore.GetTypeForFoodPrompt(m_ActiveLanguage);
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
            if (lB_SuggestedFoods.SelectedIndex >= 0)
            {
                ItemToAdd = m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]];
            }

            foreach (RadioButton element in m_AmountRadioButtons)
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
                L_FoodAmountWarning.Text = TranslatorCore.FoodAmountNullWarning(m_ActiveLanguage);
            }

            if (ItemToAdd.USName != null)
            {
                if (!m_CurrentManager.User.UserFridge.USUsersFoods.ContainsKey(ItemToAdd.USName))
                {
                    m_CurrentManager.User.UserFridge.AddToFridge(ItemToAdd);

                    switch (m_ActiveLanguage)
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

                    m_CurrentManager.UsersFridge.SaveFridge();
                    l_SearchInfo.Text = TranslatorCore.ItemAddedMessage(m_ActiveLanguage);

                    RefreshFridge();
                    PopulateFridgeSummary();
                }
                else
                {
                    l_SearchInfo.Text = TranslatorCore.ItemAlreadyInFridgeMessage(m_ActiveLanguage);
                }
            }
            Tb_CustomAmount.Clear();
            foreach (RadioButton element in m_AmountRadioButtons)
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
                FoodDef ItemToRemove = m_CurrentManager.FoodManag.Foods[m_ItemsFoodList[lb_FoodList.SelectedIndex].ToLower()];
                m_CurrentManager.User.UserFridge.RemoveFromFridge(ItemToRemove.USName.ToLower());
                lb_FoodList.Items.Remove(lb_FoodList.SelectedIndex);
                lb_FoodList.Items.Clear();
                m_CurrentManager.User.UserFridge.SaveFridge();
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
            foreach (KeyValuePair<string, FoodDef> element in m_CurrentManager.User.UserFridge.USUsersFoods)
            {
                switch (m_ActiveLanguage)
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
            m_ItemsFoodList.Clear();
            foreach (KeyValuePair<string, FoodDef> ItemToAdd in m_CurrentManager.UsersFridge.USUsersFoods)
            {
                m_ItemsFoodList.Add(ItemToAdd.Key.ToLower());

                if (!lb_FoodList.Items.Contains(ItemToAdd.Value.USName) || !lb_FoodList.Items.Contains(ItemToAdd.Value.BGName))
                {
                    switch (m_ActiveLanguage)
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

                foreach (RadioButton checkBox in m_AmountRadioButtons)
                {
                    if (Array.IndexOf(m_AmountRadioButtons, checkBox) < m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].SuggestedAmounts.Count)
                        checkBox.Content = m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].SuggestedAmounts[Array.IndexOf(m_AmountRadioButtons, checkBox)];
                }

                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        Tb_UOMLabel.Text = m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].USUOM;

                        break;
                    case Languages.Bulgarian:
                        Tb_UOMLabel.Text = m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[lB_SuggestedFoods.SelectedIndex]].BGUOM;

                        break;
                }
            }
        }
        #endregion

        #region Fridge infromation management
        //Updates the content related to fridge statistics on the FridgePanel
        private void PopulateFridgeSummary()
        {
            m_CalorieSum = 0;
            m_ProteinSum = 0;
            m_FatSum = 0;
            m_CarbSum = 0;
            m_SugarSum = 0;
            m_SodiumSum = 0;
            if (m_CurrentManager.User.UserFridge.USUsersFoods.Count > 0)
            {
                foreach (KeyValuePair<string, FoodDef> element in m_CurrentManager.User.UserFridge.USUsersFoods)
                {
                    m_CalorieSum += element.Value.Calories;
                    m_ProteinSum += element.Value.Protein;
                    m_FatSum += element.Value.Fat;
                    m_CarbSum += element.Value.Carbs;
                    m_SugarSum += element.Value.Sugars;
                    m_SodiumSum += element.Value.Sodium;
                }
                m_SummaryValues = new float[] { m_CalorieSum, m_ProteinSum, m_FatSum, m_CarbSum, m_SugarSum, m_SodiumSum };
                for (int i = 0; i < m_SummaryTextBlocks.Length; i++)
                {
                    m_SummaryTextBlocks[i].Text = m_SummaryValues[i].ToString() + " " + "g";
                }
            }
            else
            {
                m_SummaryValues = new float[] { m_CalorieSum, m_ProteinSum, m_FatSum, m_CarbSum, m_SugarSum, m_SodiumSum };
                for (int i = 0; i < m_SummaryTextBlocks.Length; i++)
                {
                    m_SummaryTextBlocks[i].Text = m_SummaryValues[i].ToString() + " " + "g";
                }
            }
        }

        //Handles showing the nutritional information about the item the user has selected.
        private void GetAndShowFoodInfo()
        {
            if (lb_FoodList.SelectedItem != null)
            {
                FoodDef SelectedItem = m_CurrentManager.FoodManag.Foods[m_ItemsFoodList[lb_FoodList.SelectedIndex].ToLower()];
                tb_FoodName.Text = SelectedItem.USName.First().ToString().ToUpper() + SelectedItem.USName.Substring(1);
                tb_FoodItemCalorie.Text = SelectedItem.Calories.ToString();
                tB_FoodProtein.Text = SelectedItem.Protein.ToString() + " " + "g";
                tB_FoodFat.Text = SelectedItem.Fat.ToString() + " " + "g";
                tB_FoodCarbs.Text = SelectedItem.Carbs.ToString() + " " + "g";
                tB_FoodSugar.Text = SelectedItem.Sugars.ToString() + " " + "g";
                tB_FoodSodium.Text = SelectedItem.Sodium.ToString() + " " + "g";

                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        tb_FoodName.Text = SelectedItem.USName.First().ToString().ToUpper() + SelectedItem.USName.Substring(1);
                        break;
                    case Languages.Bulgarian:
                        tb_FoodName.Text = SelectedItem.BGName.First().ToString().ToUpper() + SelectedItem.BGName.Substring(1);
                        break;
                    default:
                        break;
                }


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

        #region Shopping List 
        //Handles refreshing the shopping list listbox
        private void ManageShoppingList()
        {
            Lb_ShoppingList.Items.Clear();
            Lb_ShoppingList.Items.Refresh();
            List<string> listToUse = new List<string>();
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    listToUse = m_CurrentManager.User.UserShoppingList.USFoodsToBuy;
                    break;
                case Languages.Bulgarian:
                    listToUse = m_CurrentManager.User.UserShoppingList.BGFoodsToBuy;
                    break;
            }
            foreach (string element in listToUse)
            {
                if (!Lb_ShoppingList.Items.Contains(element))
                {
                    Lb_ShoppingList.Items.Add(element);
                }
            }
        }

        //Handles adding to the shopping list
        private void Btn_AddToShoppingList_Click(object sender, RoutedEventArgs e)
        {
            if (!Lb_ShoppingList.Items.Contains(Tb_AddToShoppingListSearch.Text.ToLower()) && Tb_AddToShoppingListSearch.Text != TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage))
            {
                FoodDef foodItemBeingUsed = m_CurrentManager.FoodManag.Foods[m_ItemsInFoodSearch[L_ShoppingListSuggestedItem.SelectedIndex]];

                switch (m_ActiveLanguage)
                {
                    case Languages.English:
                        Lb_ShoppingList.Items.Add(foodItemBeingUsed.USName);
                        break;
                    case Languages.Bulgarian:
                        Lb_ShoppingList.Items.Add(foodItemBeingUsed.BGName);
                        break;
                }

                m_CurrentManager.UserShoppingList.AddToShoppingList(foodItemBeingUsed.USName, foodItemBeingUsed.BGName);
                m_CurrentManager.SaveShoppingList();
                ManageShoppingList();
            }
            Tb_AddToShoppingListSearch.Text = null;
        }

        //Handles adding the name of the itme to add and closing the suggesed items list.
        private void ShoppingListSuggestedItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (L_ShoppingListSuggestedItem.SelectedItem != null)
            {
                Tb_AddToShoppingListSearch.Text = L_ShoppingListSuggestedItem.SelectedItem.ToString();
                L_ShoppingListSuggestedItem.Visibility = Visibility.Hidden;
            }
        }

        //Handles removing items from the shopping list.
        private void Btn_RemoveFromShoppingList_Click_1(object sender, RoutedEventArgs e)
        {
            if (Lb_ShoppingList.SelectedItem != null)
            {
                m_CurrentManager.UserShoppingList.RemoveFromShoppingList(Lb_ShoppingList.SelectedIndex);
                m_CurrentManager.SaveShoppingList();
                ManageShoppingList();
            }
        }

        //Handles showing the suggested items to add list.
        private void ShoppingSearchMouseEnter(object sender, MouseEventArgs e)
        {
            L_ShoppingListSuggestedItem.Visibility = Visibility.Visible;
        }

        //Handles hiding the suggested items to add when there is no text in the textbox.
        private void ShoppingSearchMouseLeave(object sender, MouseEventArgs e)
        {
            if (Tb_AddToShoppingListSearch.Text == null || Tb_AddToShoppingListSearch.Text == TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage))
            {
                L_ShoppingListSuggestedItem.Visibility = Visibility.Hidden;
            }
        }

        //Hides suggested items to add to shopping list when mouse leaves.
        private void SuggestedItemsMouseLeave(object sender, MouseEventArgs e)
        {
            L_ShoppingListSuggestedItem.Visibility = Visibility.Hidden;
        }

        //Handles hiding suggested items list on shopping list close.
        private void Btn_CloseShoppingList_Click(object sender, RoutedEventArgs e)
        {
            L_ShoppingListSuggestedItem.Visibility = Visibility.Hidden;
        }

        #endregion

        #region User settings functions
        //Updates information on the UsersSettings panel
        private void UpdateSettingPanel()
        {
            if (File.Exists(m_UserFile))
            {
                tb_NameInput.Text = m_CurrentManager.User.UserName;
                tb_AgeInput.Text = m_CurrentManager.User.Age.ToString();
                tb_WeightInput.Text = m_CurrentManager.User.Weight.ToString();

                foreach (CheckBox element in m_SettingOptions)
                {
                    if (m_CurrentManager.User.Preferences.Contains(m_CurrentManager.CompatabilityMap[m_SettingOptions.IndexOf(element)]))
                        element.IsChecked = true;
                    else
                        element.IsChecked = false;
                }

                if (m_CurrentManager.User.LanguagePref == "BG")
                    CB_Bulgarian.IsChecked = true;

                if (m_CurrentManager.User.LanguagePref == "US")
                    CB_English.IsChecked = true;
            }
            else
            {
                tb_NameInput.Text = null;
                tb_AgeInput.Text = null;
                tb_WeightInput.Text = null;
            }

            if (m_CurrentManager.User.Sex == "male")
            {
                rb_Male.IsChecked = true;
            }

            if (m_CurrentManager.User.Sex == "female")
            {
                rb_Female.IsChecked = true;
            }
        }

        //Handles saving the user settings. All values on the settings panel are set to the corresponding properties of the User class in the CurrentManager.
        private void SaveUserSettings()
        {
            if (!string.IsNullOrWhiteSpace(tb_NameInput.Text) && !string.IsNullOrWhiteSpace(tb_AgeInput.Text) && !string.IsNullOrWhiteSpace(tb_WeightInput.Text))
            {
                m_CurrentManager.User.UserName = tb_NameInput.Text;

                if (int.TryParse(tb_AgeInput.Text, out int parsedValue))
                    m_CurrentManager.User.Age = int.Parse(tb_AgeInput.Text);
                else MessageBox.Show("Age is a number value!");

                if (int.TryParse(tb_WeightInput.Text, out parsedValue))
                    m_CurrentManager.User.Weight = int.Parse(tb_WeightInput.Text);
                else MessageBox.Show("Weight is a number value!");
            }

            if (rb_Male.IsChecked == true)
            {
                m_CurrentManager.User.Sex = "male";
            }

            if (rb_Female.IsChecked == true)
            {
                m_CurrentManager.User.Sex = "female";
            }


            if (CB_English.IsChecked == true)
            {
                m_ActiveLanguage = Languages.English;
                m_CurrentManager.User.LanguagePref = "US";
                Localizer.SwitchLanguage(this, "en-US");
            }

            if (CB_Bulgarian.IsChecked == true)
            {
                m_ActiveLanguage = Languages.Bulgarian;
                m_CurrentManager.User.LanguagePref = "BG";
                Localizer.SwitchLanguage(this, "bg-BG");
            }

            if (CB_English.IsChecked == true)
            {
                m_ActiveLanguage = Languages.English;
                m_CurrentManager.User.LanguagePref = "US";
                Localizer.SwitchLanguage(this, "en-US");
            }

            foreach (CheckBox element in m_SettingOptions)
            {
                if (element.IsChecked == true)
                {
                    if (!m_CurrentManager.User.Preferences.Contains(m_CurrentManager.CompatabilityMap[m_SettingOptions.IndexOf(element)]))
                    {
                        m_CurrentManager.User.Preferences.Add(m_CurrentManager.CompatabilityMap[m_SettingOptions.IndexOf(element)]);
                    }
                }

                if (element.IsChecked == false)
                {
                    if (m_CurrentManager.User.Preferences.Contains(m_CurrentManager.CompatabilityMap[m_SettingOptions.IndexOf(element)]))
                    {
                        m_CurrentManager.User.Preferences.Remove(m_CurrentManager.CompatabilityMap[m_SettingOptions.IndexOf(element)]);
                    }
                }
            }

            m_SuggestedRecipe = new RecipeDef();
            img_RecipeImage.Fill = Brushes.White;
            m_RecipeIngredientList = new List<FoodDef>();
            lv_Ingredients.ItemsSource = m_RecipeIngredientList;

            m_CurrentManager.SaveUser();
            m_CurrentManager.User.CalculateIndex();
            m_CurrentManager.SaveUser();
            tB_UserName.Text = m_CurrentManager.User.UserName;
            m_CurrentManager.RecipieManag.SortRecipes();
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
            tb_FoodSearch.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
        }

        //Removes "Search" text fomr FoodSearch textbox when it gets focus
        private void SearchFoodClearTextBox(object sender, RoutedEventArgs e)
        {
            tb_FoodSearch.Text = null;
        }

        //Removes "Search" text from ShoppingListSearch textbox when it gets focus
        private void ShoppingListSearchGotFocus(object sender, RoutedEventArgs e)
        {
            if (L_ShoppingListSuggestedItem != null)
            {
                L_ShoppingListSuggestedItem.Visibility = Visibility.Visible;
            }
            Tb_AddToShoppingListSearch.Text = null;
        }

        //Changes text of the ShoppingListSearch textbox when it looses focus.
        private void ShoppingListSearchLostFocus(object sender, RoutedEventArgs e)
        {
            Tb_AddToShoppingListSearch.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
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


        private void Tb_AddToShoppingListTxtChanged(object sender, TextChangedEventArgs e)
        {
            SearchFoodItems();
        }

        //Adds the configured food item to the fridge.
        private void AddFoodItemClick(object sender, RoutedEventArgs e)
        {
            L_ShoppingListSuggestedItem.Visibility = Visibility.Visible;
            AddFoodItem();
        }

        //Suggests items the user might want to add to their shopping list.
        private void ItemToAddToListSelected(object sender, MouseButtonEventArgs e)
        {
            Tb_AddToShoppingListSearch.Text = L_ShoppingListSuggestedItem.SelectedItem.ToString();
            L_ShoppingListSuggestedItem.Visibility = Visibility.Hidden;
        }

        //Sets active food search enum to fridge.
        private void Btn_SearchFoods_Click(object sender, RoutedEventArgs e)
        {
            m_ActiveFoodsearch = ActiveFoodsearch.Fridge;
        }

        //Sets active food search enum to shopping list.
        private void Btn_OpenShoppingCart_Click(object sender, RoutedEventArgs e)
        {
            m_ActiveFoodsearch = ActiveFoodsearch.ShoppingList;
            ManageShoppingList();
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
            if (m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Breakfast.Count || m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Lunch.Count || m_NumerOfRecipeToSuggest < m_CurrentManager.RecipieManag.Dinner.Count)
            {
                tB_RecipeName.FontSize = 24;
                m_NumerOfRecipeToSuggest++;
                SuggestRecipe();
                SetRecentlyViewedImages();
            }
        }

        //Handles Showing previour recipe  to the user.
        private void Btn_ShowPreviousRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (m_NumerOfRecipeToSuggest >= 0)
            {
                tB_RecipeName.FontSize = 24;
                m_NumerOfRecipeToSuggest--;
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
            tb_SearchCookedRecipes.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
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
            tb_SearchSavedRecipes.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
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
            Rect_UNameBackground.Stroke = Brushes.Transparent;
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
                m_ActiveLanguage = Languages.Bulgarian;
                tB_RecipeName.Text = m_SuggestedRecipe.BGName.First().ToString().ToUpper() + m_SuggestedRecipe.BGName.Substring(1);
            }

            if (LocaleCode == "en-US")
            {
                m_ActiveLanguage = Languages.English;
                tB_RecipeName.Text = m_SuggestedRecipe.USName.First().ToString().ToUpper() + m_SuggestedRecipe.USName.Substring(1);
            }

            AddInformationToFullRecipeView();
            SetRecentlyViewedImages();
            RefreshFridge();
            ConfigureSavedRecipesPanel();
            lb_SavedRecipesList.Items.Clear();
            lb_ListOfCookedRecipes.Items.Clear();
            GetAndShowFoodInfo();
            ManageShoppingList();
            foreach (string element in GetSavedRecipesList())
            {
                lb_SavedRecipesList.Items.Add(element.First().ToString().ToUpper() + element.Substring(1));
            }

            foreach (string element in GetCookedRecipeList())
            {
                lb_ListOfCookedRecipes.Items.Add(element.First().ToString().ToUpper() + element.Substring(1));
            }

            tb_FoodSearch.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
            tb_SearchSavedRecipes.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
            tb_SearchCookedRecipes.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);
            Tb_AddToShoppingListSearch.Text = TranslatorCore.GetTextboxDefaultText(m_ActiveLanguage);

            if (m_CurrentManager.UsersFridge.FridgeConatains(m_SuggestedRecipe.USIngredients, m_SuggestedRecipe.Amounts, m_SuggestedRecipe.Units, m_CurrentManager.FoodManag))
            {
                Tb_IngrMessageTitle.Foreground = Brushes.Green;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForAllIngrPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForAllIngrPresent(m_ActiveLanguage);
            }
            else
            {
                Tb_IngrMessageTitle.Foreground = Brushes.Red;
                Tb_IngrMessageTitle.Text = TranslatorCore.GetMessageTitleForIngrNotPresent(m_ActiveLanguage);
                Tb_IngrMessageContents.Text = TranslatorCore.GetMessageContentForIngrNotPresent(m_ActiveLanguage);
            }
        }

        //Gets appropriate Saved recipe list.
        private List<string> GetSavedRecipesList()
        {
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    {
                        return m_CurrentManager.UserRecipeSaves.USSavedRecipes;
                    }
                case Languages.Bulgarian:
                    {
                        return m_CurrentManager.UserRecipeSaves.BGSavedRecipes;
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
            switch (m_ActiveLanguage)
            {
                case Languages.English:

                    {
                        return m_CurrentManager.UserRecipeSaves.USRecentlyViewed;
                    }
                case Languages.Bulgarian:
                    {
                        return m_CurrentManager.UserRecipeSaves.BGRecentlyViewed;
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
            switch (m_ActiveLanguage)
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
            switch (m_ActiveLanguage)
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
            switch (m_ActiveLanguage)
            {
                case Languages.English:
                    {
                        return m_CurrentManager.UserRecipeSaves.USCookedRecipes;
                    }
                case Languages.Bulgarian:
                    {
                        return m_CurrentManager.UserRecipeSaves.BGCookedRecipes;
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
            switch (m_ActiveLanguage)
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