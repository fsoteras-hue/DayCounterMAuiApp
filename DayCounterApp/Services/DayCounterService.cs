using DayCounterApp.Models;

namespace DayCounterApp.Services;

public class DayCounterService
{
    private const string StartDateKey = "DayCounterStartDate";

    public CounterState GetCounterState()
    {
        var startDateString = Preferences.Get(StartDateKey, string.Empty);
        
        if (string.IsNullOrEmpty(startDateString))
        {
            return new CounterState
            {
                StartDate = null,
                DaysElapsed = 0,
                IsActive = false
            };
        }

        if (DateTime.TryParse(startDateString, out var startDate))
        {
            var daysElapsed = CalculateDaysElapsed(startDate);
            return new CounterState
            {
                StartDate = startDate,
                DaysElapsed = daysElapsed,
                IsActive = true
            };
        }

        return new CounterState
        {
            StartDate = null,
            DaysElapsed = 0,
            IsActive = false
        };
    }

    public void StartCounter()
    {
        var now = DateTime.Now;
        Preferences.Set(StartDateKey, now.ToString("O")); // ISO 8601 format
    }

    public void ResetCounter()
    {
        Preferences.Remove(StartDateKey);
    }

    public int CalculateDaysElapsed(DateTime startDate)
    {
        var timeSpan = DateTime.Now - startDate;
        return (int)timeSpan.TotalDays;
    }
}
