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
        string UserFile = @"d:\Desktop\USER5.json";
        string DefaultUserFile = @"d:\Desktop\DEFAULT_USER.json";

        string UserFridgeFile = @"d:\Desktop\USER_F.json";
        string DefaultFridgeFile = @"d:\Desktop\DEFAULT_FRIDGE.json";

        string FoodDefFile = @"d:\Desktop\FoodData.json";
        string RecipeDatabase = @"d:\Desktop\Recipes.json";

        List<CheckBox> SettingOptions;

        ProgramManager CurrentManager;

        public MainWindow()
        {
            InitializeComponent();
            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile);
            SettingOptions = new List<CheckBox> { cb_Vegan, cb_Vegetarian, cb_Diabetic, cb_Eggs, cb_Dairy, cb_Fish, cb_Nuts, cb_Gluten, cb_Soy };
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_SeeAllSavedRecipes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SuggestRecipe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ShowRecipe_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Showfridge_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_SaveSettings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowSettings(object sender, MouseButtonEventArgs e)
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

        private void cb_Nuts_Copy1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void cb_Nuts_Copy_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_NameInput.Text) && !string.IsNullOrWhiteSpace(tb_AgeInput.Text) && !string.IsNullOrWhiteSpace(tb_WeightInput.Text))
            {
                CurrentManager.User.UserName = tb_NameInput.Text;            
                CurrentManager.User.Age = int.Parse(tb_AgeInput.Text);
                CurrentManager.User.Weight = int.Parse(tb_WeightInput.Text);
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
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
