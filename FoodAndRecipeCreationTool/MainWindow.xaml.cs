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

namespace FoodAndRecipeCreationTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
