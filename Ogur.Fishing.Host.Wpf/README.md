# Ogur.Fishing.Host.Wpf

The WPF application host for the Fishing capability. Implements MVVM pattern with dependency injection and integrates with Ogur.Hub for centralized management.

## Responsibilities

- Authentication flow (JWT login + license validation)
- Server selection with visual cards
- Main fishing dashboard with real-time event logging
- Process detection and selection
- Chat buffer detection with progress indicator
- Hub integration (telemetry, SignalR commands, updates)

## Key Components

### ViewModels
- `LoginViewModel`: JWT authentication and license validation
- `ServerSelectViewModel`: Multi-server selection with enabled/disabled states
- `MainViewModel`: Main fishing interface with EventBus integration

### Services
- `HubIntegrationService`: Manages Hub connection and periodic license validation
- `HubCommandHandler`: Handles real-time SignalR commands
- `ProcessQueryService`: Detects and lists Metin2 game processes
- `SessionState`: Singleton state management (selected server, bait, process, memory address)

### Navigation
- `INavigationService`: View navigation coordination
- `INavigator`: View display management

## UI Features

- Dark theme with Material Design-inspired styling
- Button press animations (scale effect)
- Spinning refresh icon during process scan
- Progress bar for chat buffer detection
- Real-time event log with timestamps
- Bait slot selection with visual feedback
- Always-on-top toggle

## Build & Run
```bash
dotnet run --project Ogur.Fishing.Host.Wpf
```

## Dependencies

- CommunityToolkit.Mvvm (MVVM helpers)
- Microsoft.Extensions.Hosting (application lifetime)
- Ogur.Core (Hub integration)
- Ogur.Abstractions (shared interfaces)