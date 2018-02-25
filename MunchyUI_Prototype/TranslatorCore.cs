using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchyUI
{
    public static class TranslatorCore
    {
        public static string GetSuggestedRecipeInfo(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Sorry we ran out of suitable recipes for you. Try a manual search.";

                case Languages.Bulgarian:
                    return "Извинете, не успяхме да намерим рецепта. Опитайте ръчно търсене.";

                default:
                    return "Lang Err";
            }

        }

        public static string GetNoDataLabel(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "No Data";

                case Languages.Bulgarian:
                    return "Няма данни";

                default:
                    return "Lang Err";
            }

        }

        public static string GetTooLateToEatMessage(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "It's too late to eat! Wait until the morning.";

                case Languages.Bulgarian:
                    return "Много е късно да ядете! Изчакайте до сутринта.";

                default:
                    return "Lang Err";
            }            
        }

        public static string GetClickToAddFoodMessage(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Click on an item to add it";

                case Languages.Bulgarian:
                    return "Кликнете върху храната да я добавите.";

                default:
                    return "Lang Err";
            }            
        }

        public static string GetTypeForFoodPrompt(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Type what you would like to add.";

                case Languages.Bulgarian:
                    return "Напишете това което търсите.";

                default:
                    return "Lang Err";
            }           
        }

        public static string FoodAmountNullWarning(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Please add a food amount!";

                case Languages.Bulgarian:
                    return "Моля добавете количество за храната!";

                default:
                    return "Lang Err";
            }            
        }

        public static string ItemAddedMessage(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Item added!";

                case Languages.Bulgarian:
                    return "Продукта добавен!";

                default:
                    return "Lang Err";
            }            
        }

        public static string ItemAlreadyInFridgeMessage(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Item already in fridge.";

                case Languages.Bulgarian:
                    return "Продукта вече е в хладилника.";

                default:
                    return "Lang Err";
            }          
        }

        public static string GetTextboxDefaultText(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Search";

                case Languages.Bulgarian:
                    return "Търси";


                default:
                    return "Lang Err";
            }          
        }
    }
}
