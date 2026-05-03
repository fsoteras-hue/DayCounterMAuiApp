# Day Counter MAUI App

A simple yet elegant day counter application built with .NET MAUI for Android. Track days elapsed from a specific date with a clean purple-themed UI.

## Features

- 🔢 **Large Day Display** - Shows elapsed days in bold, easy-to-read format
- 💾 **Data Persistence** - Start date saved locally using MAUI Preferences
- 🔔 **App Icon Badge** - Displays day count on the app icon (Android 8.0+)
- 🔄 **Auto-Update** - Updates on app launch/resume
- 📱 **MIUI Optimized** - Tested and working on Xiaomi devices
- 🎨 **Clean UI** - Purple-themed design with smooth animations

## Screenshots

The app features:
- Large day counter display (120pt font)
- "Start Counting" button to begin tracking
- "Reset" button with confirmation dialog
- Haptic feedback for button interactions
- White text on purple gradient background

## Technical Details

### Architecture
- **Framework**: .NET MAUI 10.0
- **Language**: C# with nullable reference types
- **Pattern**: Dependency Injection with services
- **Platform**: Android (API 21+)

### Key Components

#### Services
- `DayCounterService` - Manages date persistence and day calculations
- `NotificationBadgeService` - Handles Android notification badges

#### Models
- `CounterState` - Represents counter state (StartDate, DaysElapsed, IsActive)

#### Platform-Specific
- Android notification channels for badge display
- Runtime permission handling for Android 13+
- Uncompressed native libraries for proper deployment

## Building

### Prerequisites
- .NET 10.0 SDK
- Android SDK (API 36)
- Android emulator or physical device

### Build Commands

```bash
# Debug build
dotnet build -f net10.0-android -c Debug

# Release build (optimized, smaller size)
dotnet build -f net10.0-android -c Release

# Run on connected device
dotnet build -t:Run -f net10.0-android
```

## Installation

### On Android Device

1. Enable USB debugging in Developer Options
2. Connect device via USB
3. Run: `adb devices` to verify connection
4. Build and install:
   ```bash
   dotnet build -f net10.0-android -c Release
   adb install DayCounterApp/bin/Release/net10.0-android/com.companyname.daycounterapp-Signed.apk
   ```

### On Emulator

1. Start Android emulator
2. Run: `dotnet build -t:Run -f net10.0-android`

## Usage

1. **Start Counting**: Tap the "Start Counting" button to begin tracking days from today
2. **View Days**: The large number shows days elapsed (updates hourly while open, on launch/resume)
3. **Check Badge**: Look at your app icon to see the day count badge
4. **Reset**: Tap "Reset" button and confirm to clear the counter

## Configuration

### Enable App Icon Badge (MIUI)

1. Long-press the Day Counter app icon
2. Tap "App info"
3. Go to "Notifications"
4. Enable "Show notifications" and "App icon badges"

Or: **Settings > Notifications & Control Center > App notifications > DayCounterApp > Enable Badge**

## Project Structure

```
DayCounterApp/
├── Models/
│   └── CounterState.cs
├── Services/
│   ├── DayCounterService.cs
│   └── Abstractions/
│       └── INotificationBadgeService.cs
├── Platforms/
│   └── Android/
│       ├── MainActivity.cs
│       ├── AndroidManifest.xml
│       └── Services/
│           └── NotificationBadgeService.cs
├── MainPage.xaml
├── MainPage.xaml.cs
├── App.xaml.cs
└── MauiProgram.cs
```

## Implementation Notes

### Fixed Issues
- **Assembly Packaging**: Configured `EmbedAssembliesIntoApk=true` to prevent Fast Deployment crashes
- **Native Libraries**: Set `AndroidStoreUncompressedNativeLibraries=true` for proper loading
- **MIUI Compatibility**: Added proper notification permissions for Android 13+

### Known Limitations
- Badge only updates when app is launched/resumed (no background service)
- Badge support varies by Android launcher
- Currently Android-only (iOS support not implemented)

## Supported Devices

- **Minimum**: Android 5.0 (API 21)
- **Badge Support**: Android 8.0+ (API 26)
- **Permission Dialog**: Android 13+ (API 33)
- **Tested On**: Xiaomi Mi 9T (Android 10, MIUI)

## License

This project is available for educational and personal use.

## Development

Built with ❤️ using .NET MAUI

### Future Enhancements
- Background WorkManager for daily badge updates
- iOS version with badge support
- Multiple counters
- Counter history and statistics
- Home screen widget
- Customizable themes
