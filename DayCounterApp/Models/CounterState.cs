namespace DayCounterApp.Models;

public class CounterState
{
    public DateTime? StartDate { get; set; }
    public int DaysElapsed { get; set; }
    public bool IsActive { get; set; }
}
