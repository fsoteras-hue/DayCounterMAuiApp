using Android.App;
using Android.Content;
using AndroidX.Core.App;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp.Platforms.Android.Services;

public class NotificationBadgeService : INotificationBadgeService
{
    private const string ChannelId = "day_counter_channel";
    private const string ChannelName = "Day Counter";
    private const int NotificationId = 1001;

    private readonly Context _context;

    public NotificationBadgeService()
    {
        _context = global::Android.App.Application.Context;
        CreateNotificationChannel();
    }

    public NotificationBadgeService(Context context)
    {
        _context = context;
        CreateNotificationChannel();
    }

    private void CreateNotificationChannel()
    {
        if (_context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager)
        {
            var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Low)
            {
                Description = "Used to display day count badge on app icon"
            };
            channel.SetShowBadge(true);
            notificationManager.CreateNotificationChannel(channel);
        }
    }

    public void UpdateBadge(int count)
    {
        if (_context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager)
        {
            var notification = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle("Day Counter")
                .SetContentText($"{count} days")
                .SetSmallIcon(Resource.Drawable.dotnet_bot)
                .SetNumber(count)
                .SetShowWhen(false)
                .SetOngoing(true)
                .SetAutoCancel(false)
                .SetPriority(NotificationCompat.PriorityLow)
                .Build();

            notificationManager.Notify(NotificationId, notification);
        }
    }

    public void ClearBadge()
    {
        if (_context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager)
        {
            notificationManager.Cancel(NotificationId);
        }
    }
}
