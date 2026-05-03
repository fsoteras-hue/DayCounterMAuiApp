using Microsoft.Extensions.DependencyInjection;
using DayCounterApp.Services;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	protected override void OnStart()
	{
		base.OnStart();
		UpdateBadge();
	}

	protected override void OnResume()
	{
		base.OnResume();
		UpdateBadge();
	}

	private void UpdateBadge()
	{
		try
		{
			var counterService = Handler?.MauiContext?.Services.GetService<DayCounterService>();
			var badgeService = Handler?.MauiContext?.Services.GetService<INotificationBadgeService>();

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
			// Services not available yet or badge service not implemented for platform
		}
	}
}