using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Nikola.Munchy.MunchyAPI
{
    public class StatisticsManager
    {
        public int TotalRecipesCooked { get; set; }
        public int TotalRecipesSeen { get; set; }

        public int DailyCalories { get; set; }
        public int WeeklyCalories { get; set; }
        public int MonthlyCalories { get; set; }
        public int AnualCalories { get; set; }
        public int AverageDailyCalories { get; set; }
        public int AverageMonthtlyCalories { get; set; }
        public int TotalCaloriesConsumed { get; set; }

        public int TotalDaysUsingProgram { get; set; }
        public int TotalMonthsUsingProgram { get; set; }
        public int TotalYearsUsingProgram { get; set; }

        public int PreviousDay = 0;
        public int PreviousMonth = 0;
        public int PreviousYear = 0;

        [NonSerialized]
        public string SaveLocation;

        public StatisticsManager(string SavePath)
        {
            SaveLocation = SavePath;
            if (PreviousDay == 0 || PreviousMonth == 0 || PreviousYear == 0)
            {
                PreviousDay = DateTime.Now.Day;
                PreviousMonth = DateTime.Now.Month;
                PreviousYear = DateTime.Now.Year;
            }
            SaveStatistics();
        }

        //Adds calories to the statistics and then saves information.
        public void AddToCalorieStatistics(int AmountToAdd)
        {
            TotalCaloriesConsumed += AmountToAdd;
            DailyCalories += AmountToAdd;
            WeeklyCalories += AmountToAdd;
            MonthlyCalories += AmountToAdd;
            AnualCalories += AmountToAdd;
            TotalRecipesCooked++;
            SaveStatistics();
            DailyReset();
        }

        //Handles saving statistics to JSON file.
        public void SaveStatistics()
        {
            using (StreamWriter file = File.CreateText(SaveLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, this);
            }
        }

        //Handles resetting calories daily.
        public void DailyReset()
        {
            if (DateTime.Now.Day > PreviousDay)
            {
                DailyCalories = 0;
            }
        }

        //Handles tracking date changes.
        public void DateManagement()
        {
            int CurrentDate = DateTime.Now.Day;
            int CurrentMonth = DateTime.Now.Month;
            int CurrentYear = DateTime.Now.Year;

            if (CurrentDate != PreviousDay)
            {
                PreviousDay = CurrentDate;
                TotalDaysUsingProgram++;
            }

            if (CurrentMonth != PreviousMonth)
            {
                PreviousMonth = CurrentMonth;
                TotalMonthsUsingProgram++;
            }

            if (CurrentYear != PreviousYear)
            {
                PreviousYear = CurrentYear;
            }
            SaveStatistics();
        }

        //Calcualates average values.
        public void CalculateAverageSums()
        {
            if (TotalDaysUsingProgram > 0)
            {
                AverageDailyCalories = TotalCaloriesConsumed / TotalDaysUsingProgram;
            }
            else
            {
                AverageMonthtlyCalories = 0;
            }

            if (TotalMonthsUsingProgram > 0)
            {
                AverageMonthtlyCalories = TotalCaloriesConsumed / TotalMonthsUsingProgram;
            }
            else
            {
                AverageMonthtlyCalories = 0;
            }
            SaveStatistics();
        }

    }
}
