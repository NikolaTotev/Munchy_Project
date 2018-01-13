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

namespace MunchyUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserSettings UserSettingsWindow;
        ProgramManager CurrentManager;

        string UserFile = @"d:\Desktop\USER5.json";
        string DefaultUserFile = @"d:\Desktop\DEFAULT_USER.json";

        string UserFridgeFile = @"d:\Desktop\USER_F.json";
        string DefaultFridgeFile = @"d:\Desktop\DEFAULT_FRIDGE.json";

        string FoodDefFile = @"d:\Desktop\FoodData.json";
        string RecipeDatabase = @"d:\Desktop\Recipes.json";

       List<CheckBox> PreferenceSettings;

        public MainWindow()
        {
            InitializeComponent();
            PreferenceSettings = new  List<CheckBox> { cb_Vegetarian, cb_Vegan, cb_Diabetic, cb_A_Egg, cb_A_Dairy, cb_A_Fish, cb_A_Nuts, cb_A_Gluten, cb_A_Soy};
            UserSettingsWindow = new UserSettings();
            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile);
            btn_UserName.Content = CurrentManager.User.UserName;
            l_RecipeTitle.Text= CurrentManager.RecipieManag.Breakfast[0];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!p_UserSettings.IsVisible)
            {
                p_UserSettings.Visibility = Visibility.Visible;
            }
            else
            {
                p_UserSettings.Visibility = Visibility.Hidden;
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!p_Menu.IsVisible)
            {
                p_Menu.Visibility = Visibility.Visible;
            }
            else
            {
                p_Menu.Visibility = Visibility.Collapsed;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox element in PreferenceSettings)
            {
                if (element.IsChecked == true)
                {
                    if (!CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[PreferenceSettings.IndexOf(element)]))
                    {
                        CurrentManager.User.Preferences.Add(CurrentManager.CompatabilityMap[PreferenceSettings.IndexOf(element)]);
                    }
                }
                else
                {
                    if (CurrentManager.User.Preferences.Contains(CurrentManager.CompatabilityMap[PreferenceSettings.IndexOf(element)]))
                    {
                        CurrentManager.User.Preferences.Remove(CurrentManager.CompatabilityMap[PreferenceSettings.IndexOf(element)]);
                    }
                }
            
            }

            CurrentManager.SaveUser();




        }

        private void cb_Vegetarian_Checked(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
