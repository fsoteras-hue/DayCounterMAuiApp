using DayCounterApp.Services;
using DayCounterApp.Services.Abstractions;

namespace DayCounterApp;

public partial class MainPage : ContentPage
{
	private readonly DayCounterService _counterService;
	private readonly INotificationBadgeService? _badgeService;
	private IDispatcherTimer? _timer;
	private LinearGradientBrush? _gradientBrush;

	public MainPage(DayCounterService counterService, INotificationBadgeService? badgeService = null)
	{
		InitializeComponent();
		_counterService = counterService;
		_badgeService = badgeService;
		InitializeGradientBackground();
	}

	private void InitializeGradientBackground()
	{
		_gradientBrush = new LinearGradientBrush
		{
			StartPoint = new Point(0, 0),
			EndPoint = new Point(1, 1)
		};
		
		// Create gradient stops with explicit purple colors
		var stop1 = new GradientStop 
		{ 
			Color = Color.FromRgb(139, 92, 246), // #8B5CF6 Purple
			Offset = 0.0f 
		};
		var stop2 = new GradientStop 
		{ 
			Color = Color.FromRgb(99, 102, 241), // #6366F1 Indigo
			Offset = 1.0f 
		};
		
		_gradientBrush.GradientStops.Add(stop1);
		_gradientBrush.GradientStops.Add(stop2);
		
		BackgroundColor = Colors.Transparent;
		Background = _gradientBrush;
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
			CustomStartButton.IsVisible = false;
			ResetButton.IsVisible = true;

			// Update badge
			_badgeService?.UpdateBadge(state.DaysElapsed);
		}
		else
		{
			DayCountLabel.Text = "0";
			DayTextLabel.Text = "Days";
			StartButton.IsVisible = true;
			// Show custom start button only on first launch (counter never started)
			CustomStartButton.IsVisible = true;
			ResetButton.IsVisible = false;

			// Clear badge
			_badgeService?.ClearBadge();
		}

		// Update gradient background based on days elapsed
		UpdateGradientBackground(state.DaysElapsed);
	}

	private void UpdateGradientBackground(int days)
	{
		if (_gradientBrush == null || _gradientBrush.GradientStops.Count < 2)
			return;

		// Calculate hue shift: 2.5 degrees per day, creating a full color wheel every ~144 days
		float baseHue1 = 270; // Purple
		float baseHue2 = 240; // Blue-Violet
		float hueShift = (days * 2.5f) % 360;

		// Calculate new hues with shift
		float hue1 = (baseHue1 + hueShift) % 360;
		float hue2 = (baseHue2 + hueShift) % 360;

		// Convert HSL to RGB with alpha blending
		var color1 = ColorFromHSL(hue1, 0.75f, 0.60f, 0.95f);
		var color2 = ColorFromHSL(hue2, 0.70f, 0.65f, 0.95f);

		// Update gradient stops
		_gradientBrush.GradientStops[0].Color = color1;
		_gradientBrush.GradientStops[1].Color = color2;
	}

	private Color ColorFromHSL(float hue, float saturation, float lightness, float alpha)
	{
		// Normalize values
		hue = hue % 360;
		saturation = Math.Clamp(saturation, 0, 1);
		lightness = Math.Clamp(lightness, 0, 1);
		alpha = Math.Clamp(alpha, 0, 1);

		float c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
		float x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
		float m = lightness - c / 2;

		float r = 0, g = 0, b = 0;

		if (hue < 60)
		{
			r = c; g = x; b = 0;
		}
		else if (hue < 120)
		{
			r = x; g = c; b = 0;
		}
		else if (hue < 180)
		{
			r = 0; g = c; b = x;
		}
		else if (hue < 240)
		{
			r = 0; g = x; b = c;
		}
		else if (hue < 300)
		{
			r = x; g = 0; b = c;
		}
		else
		{
			r = c; g = 0; b = x;
		}

		return Color.FromRgba(
			(int)((r + m) * 255),
			(int)((g + m) * 255),
			(int)((b + m) * 255),
			(int)(alpha * 255)
		);
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

	private async void OnCustomStartClicked(object? sender, EventArgs e)
	{
		// Prompt user to enter number of days
		string? result = await DisplayPromptAsync(
			"Custom Start Days",
			"Enter the number of days to start from:",
			"OK",
			"Cancel",
			"Enter days...",
			maxLength: 6,
			keyboard: Keyboard.Numeric);

		if (string.IsNullOrWhiteSpace(result))
			return;

		if (int.TryParse(result, out int daysOffset) && daysOffset >= 0)
		{
			_counterService.StartCounterWithOffset(daysOffset);
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

			await CustomStartButton.FadeToAsync(0, 200);
			await CustomStartButton.FadeToAsync(1, 200);
		}
		else
		{
			await DisplayAlertAsync(
				"Invalid Input",
				"Please enter a valid positive number.",
				"OK");
		}
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
