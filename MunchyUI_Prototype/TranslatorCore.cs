using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchyUI
{
    public static class TranslatorCore
    {

        private static Dictionary<string, string> Units = new Dictionary<string, string> { { "Cup", "Чаша" }, { "Tbsp", "С.Л" }, { "Tsp", "Ч.Л" }, { "Count", "Брой" }, { "ml", "мл" } };

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

        public static string GetCookedRecipesTitle(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Cooked recipes";

                case Languages.Bulgarian:
                    return "Сготвени рецепти";

                default:
                    return "Lang Err";
            }
        }

        public static string GetSavedRecipesTitle(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Saved recipes";

                case Languages.Bulgarian:
                    return " Запазени рецепти";

                default:
                    return "Lang Err";
            }
        }


        public static string GetRecentlySeenTitle(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Recently seen recipes";

                case Languages.Bulgarian:
                    return "Скоро видяни рецепти";

                default:
                    return "Lang Err";
            }
        }

        public static string GetMessageTitleForAllIngrPresent(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "You have everything!";

                case Languages.Bulgarian:
                    return "Имате всички продукти!";

                default:
                    return "Lang Err";
            }
        }


        public static string GetMessageContentForAllIngrPresent(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Ready to cook! Ingredient amounts have been removed from fridge.";

                case Languages.Bulgarian:
                    return "Готви сте за готвене! Количествата на продуктите премахнати от хладилника.";

                default:
                    return "Lang Err";
            }
        }

        public static string GetMessageTitleForIngrNotPresent(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "You dont have all ingredients!";

                case Languages.Bulgarian:
                    return "Нямате всички продукти!";

                default:
                    return "Lang Err";
            }
        }

        public static string GetMessageContentForIngrNotPresent(Languages activeLang)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return "Some ingredients are missnig, would you a list of them to be sent to your e-mail?";

                case Languages.Bulgarian:
                    return "Някои продукти липсват, искате ли списък с тях да бъде пратен на вашия имейл?";

                default:
                    return "Lang Err";
            }
        }

        public static string GetUnit(Languages activeLang, string inputUnit)
        {
            switch (activeLang)
            {
                case Languages.English:
                    return inputUnit;

                case Languages.Bulgarian:
                    if (Units.TryGetValue(inputUnit, out string BgUnit))
                    {
                        return BgUnit;
                    }
                    else
                    {
                        return inputUnit;
                    }
                     
                default:
                    return "Lang Err";
            }
        }

    }
}
