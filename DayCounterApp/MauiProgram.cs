using Microsoft.Extensions.Logging;
using DayCounterApp.Services;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Register services
		builder.Services.AddSingleton<DayCounterService>();
		builder.Services.AddSingleton<MainPage>();

#if ANDROID
		builder.Services.AddSingleton<INotificationBadgeService, Platforms.Android.Services.NotificationBadgeService>();
#else
		builder.Services.AddSingleton<INotificationBadgeService>(sp => null);
#endif

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
