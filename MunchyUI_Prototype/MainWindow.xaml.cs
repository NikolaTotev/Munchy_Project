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

        ProgramManager CurrentManager;
        
        public MainWindow()
        {
            InitializeComponent();
            CurrentManager = new ProgramManager(UserFile, UserFridgeFile, DefaultFridgeFile, DefaultUserFile, RecipeDatabase, FoodDefFile);
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
            if (p_Settings.Visibility == Visibility.Hidden)
            {
                p_Settings.Visibility = Visibility.Visible;
            }
            else
            {
                p_Settings.Visibility = Visibility.Hidden;
            }
        }
    }
}
