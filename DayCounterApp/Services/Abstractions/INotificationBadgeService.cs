namespace DayCounterApp.Services.Abstractions;

public interface INotificationBadgeService
{
    void UpdateBadge(int count);
    void ClearBadge();
}
