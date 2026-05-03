using DayCounterApp.Services;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp;

public partial class MainPage : ContentPage
{
	private readonly DayCounterService _counterService;
	private readonly INotificationBadgeService? _badgeService;
	private IDispatcherTimer? _timer;

	public MainPage(DayCounterService counterService, INotificationBadgeService? badgeService = null)
	{
		InitializeComponent();
		_counterService = counterService;
		_badgeService = badgeService;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		UpdateDisplay();
		StartTimer();
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		StopTimer();
	}

	private void UpdateDisplay()
	{
		var state = _counterService.GetCounterState();

		if (state.IsActive)
		{
			DayCountLabel.Text = state.DaysElapsed.ToString();
			DayTextLabel.Text = state.DaysElapsed == 1 ? "Day" : "Days";
			StartButton.IsVisible = false;
			ResetButton.IsVisible = true;

			// Update badge
			_badgeService?.UpdateBadge(state.DaysElapsed);
		}
		else
		{
			DayCountLabel.Text = "0";
			DayTextLabel.Text = "Days";
			StartButton.IsVisible = true;
			ResetButton.IsVisible = false;

			// Clear badge
			_badgeService?.ClearBadge();
		}
	}

	private void StartTimer()
	{
		// Update display every hour while app is open
		_timer = Dispatcher.CreateTimer();
		_timer.Interval = TimeSpan.FromHours(1);
		_timer.Tick += (s, e) => UpdateDisplay();
		_timer.Start();
	}

	private void StopTimer()
	{
		_timer?.Stop();
	}

	private async void OnStartClicked(object? sender, EventArgs e)
	{
		_counterService.StartCounter();
		UpdateDisplay();

		// Provide haptic feedback
		try
		{
			HapticFeedback.Default.Perform(HapticFeedbackType.Click);
		}
		catch
		{
			// Haptic feedback not available on all devices
		}

		await StartButton.FadeToAsync(0, 200);
		await StartButton.FadeToAsync(1, 200);
	}

	private async void OnResetClicked(object? sender, EventArgs e)
	{
		bool confirm = await DisplayAlertAsync(
			"Reset Counter",
			"Are you sure you want to reset the day counter?",
			"Yes",
			"No");

		if (confirm)
		{
			_counterService.ResetCounter();
			UpdateDisplay();

			// Provide haptic feedback
			try
			{
				HapticFeedback.Default.Perform(HapticFeedbackType.Click);
			}
			catch
			{
				// Haptic feedback not available on all devices
			}
		}
	}
}
