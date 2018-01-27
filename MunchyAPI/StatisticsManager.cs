using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikola.Munchy.MunchyAPI
{
    class StatisticsManager
    {
        int DailyCalories { get; set; }
        int WeeklyCalories { get; set; }
        int MonthlyCalories { get; set; }
        int AnualCalories { get; set; }
        int TotalRecipesCooked { get; set; }
        int AverageDailyCalories { get; set; }
        int AverageMonthtlyCalories { get; set; }
        public void AddToCalorieStatistics(int AmountToAdd)
        {
            DailyCalories += AmountToAdd;
            WeeklyCalories += AmountToAdd;
            MonthlyCalories += AmountToAdd;
            AnualCalories += AmountToAdd;
        }
    }
}
