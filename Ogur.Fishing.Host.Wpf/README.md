# Ogur.Fishing.Host.Wpf

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![WPF](https://img.shields.io/badge/WPF-Windows-512BD4?style=flat-square)

WPF application host for the Fishing capability. Implements MVVM pattern with dependency injection and Ogur.Hub integration.

## Responsibilities
- Authentication flow (JWT login + license validation)
- Server selection with visual cards (9 Metin2 servers)
- Main fishing dashboard with real-time event logging
- Process detection and selection
- Chat buffer auto-detection with progress indicator
- Hub integration (telemetry, SignalR commands, updates)

## Key Components

### ViewModels
- **LoginViewModel**: JWT authentication and license validation
- **ServerSelectViewModel**: Multi-server selection with enabled/disabled states
- **MainViewModel**: Fishing interface with EventBus integration and state management
- **UpdateRequiredViewModel**: Force update enforcement

### Services
- **HubIntegrationService**: Hub connection and periodic license validation
- **HubCommandHandler**: Real-time SignalR command processing
- **ProcessQueryService**: Metin2 process detection and enumeration
- **SessionState**: Singleton state (selected server, bait, process, memory address)
- **FishingActionExecutor**: Coordinates fishing capability lifecycle

### Navigation
- **INavigationService**: View navigation coordination
- **NavigationService**: View display management and transitions

## UI Features
- Dark theme with Material Design-inspired styling
- Button press animations (scale effect)
- Spinning refresh icon during process scan
- Progress bar for chat buffer detection (100 snapshots)
- Real-time event log with timestamps
- Bait slot selection (F2/F3/F4 keys)
- Always-on-top window toggle
- Server logos and visual feedback

## Project Structure
```
Ogur.Fishing.Host.Wpf/
├── Views/
│   ├── LoginView.xaml          # Authentication UI
│   ├── ServerSelectView.xaml   # Server selection grid
│   ├── MainView.xaml           # Fishing dashboard
│   └── UpdateRequiredView.xaml # Update enforcement
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── ServerSelectViewModel.cs
│   ├── MainViewModel.cs
│   └── UpdateRequiredViewModel.cs
├── Services/
│   ├── HubIntegration/
│   │   ├── HubIntegrationService.cs
│   │   └── LicenseManager.cs
│   ├── Implementations/
│   │   ├── ProcessQueryService.cs
│   │   ├── HotkeyListener.cs
│   │   └── BaitCatalog.cs
│   └── AppFlowCoordinator.cs
├── Navigation/
│   ├── INavigationService.cs
│   └── NavigationService.cs
├── Assets/
│   └── Servers/                # Server logo images
└── Configuration/
    ├── OgurHubOptions.cs
    └── UiOptions.cs
```

## Build & Run
```bash
dotnet run --project Ogur.Fishing.Host.Wpf
```

## Dependencies
- `CommunityToolkit.Mvvm` - MVVM helpers (ObservableObject, RelayCommand)
- `Microsoft.Extensions.Hosting` - Application lifetime management
- `Ogur.Core` - Hub integration (auth, licensing, telemetry)
- `Ogur.Abstractions` - Shared interfaces
- `NLog.Extensions.Logging` - Structured logging
