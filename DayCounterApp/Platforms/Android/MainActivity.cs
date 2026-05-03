using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using DayCounterApp.Services;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	private const int RequestNotificationPermissionCode = 1001;

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		RequestNotificationPermission();
	}

	protected override void OnResume()
	{
		base.OnResume();
		UpdateBadgeOnResume();
	}

	private void RequestNotificationPermission()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
		{
			if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.PostNotifications) 
				!= Permission.Granted)
			{
				ActivityCompat.RequestPermissions(
					this,
					new[] { Android.Manifest.Permission.PostNotifications },
					RequestNotificationPermissionCode);
			}
		}
	}

	private void UpdateBadgeOnResume()
	{
		try
		{
			var counterService = IPlatformApplication.Current?.Services.GetService<DayCounterService>();
			var badgeService = IPlatformApplication.Current?.Services.GetService<INotificationBadgeService>();

			if (counterService != null && badgeService != null)
			{
				var state = counterService.GetCounterState();
				if (state.IsActive)
				{
					badgeService.UpdateBadge(state.DaysElapsed);
				}
				else
				{
					badgeService.ClearBadge();
				}
			}
		}
		catch
		{
			// Services not available
		}
	}
}
