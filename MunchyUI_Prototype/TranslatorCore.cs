using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunchyUI_Prototype
{
    public static class TranslatorCore
    {
        public static string GetSuggestedRecipeInfo(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Sorry we ran out of suitable recipes for you. Try a manual search.";

            if (bgBG)
                MessageToReturn = "Извинете, не успяхме да намерим рецепта. Опитайте ръчно търсене.";

            return MessageToReturn;
        }

        public static string GetTooLateToEatMessage(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "It's too late to eat! Wait until the morning.";

            if (bgBG)
                MessageToReturn = "Много е късно да ядете! Изчакайте до сутринта.";

            return MessageToReturn;
        }

        public static string GetClickToAddFoodMessage(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Click on an item to add it";

            if (bgBG)
                MessageToReturn = "Кликнете върху храната да я добавите.";

            return MessageToReturn;
        }

        public static string GetTypeForFoodPrompt(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Type what you would like to add.";

            if (bgBG)
                MessageToReturn = "Напишете това което търсите.";

            return MessageToReturn;
        }

        public static string FoodAmountNullWarning(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Please add a food amount!";

            if (bgBG)
                MessageToReturn = "Моля добавете количество за храната!";

            return MessageToReturn;
        }

        public static string ItemAddedMessage (bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Item added!";

            if (bgBG)
                MessageToReturn = "Продукта добавен!";

            return MessageToReturn;
        }

        public static string ItemAlreadyInFridgeMessage(bool enUS, bool bgBG)
        {
            string MessageToReturn = "SuggestedRecipeMSG";

            if (enUS)
                MessageToReturn = "Item already in fridge.";

            if (bgBG)
                MessageToReturn = "Продукта вече е в хладилника.";

            return MessageToReturn;
        }
    }
}
