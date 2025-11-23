# Ogur.Fishing

**Ogur.Fishing** is a professional fishing automation bot for Metin2, built with Clean Architecture principles and integrated with the Ogur.Hub centralized management system.

## Features

### Core Capabilities
- **Automatic Chat Buffer Detection**: Differential memory scanning with 100 snapshots to detect fishing messages
- **Real-time Fishing Automation**: Event-driven architecture with complete cast -> wait -> hook -> loot cycle
- **Multi-Server Support**: 9 game servers (Proxima, Tamidia2, Glador, Glevia2, Monastyr2, MT2009, Pandora, Projekt Hard, Senthia)
- **Professional WPF UI**: Dark theme with smooth animations and real-time event logging

### Hub Integration
- JWT authentication with license validation
- Device fingerprinting (HWID + GUID)
- Real-time SignalR commands (logout, block, notify, force update)
- Automatic updates with required version enforcement
- Telemetry reporting

### Technical Highlights
- Built on .NET 8 with WPF (MVVM using CommunityToolkit.Mvvm)
- Clean Architecture: Domain -> Application -> Infrastructure -> Presentation
- CQRS pattern with EventBus for cross-cutting concerns
- Win32 API integration for memory reading and keyboard input
- Polish character support (Windows-1250 encoding)

## Solution Structure
```
/src
  Ogur.Fishing.sln
  Ogur.Fishing.Host.Wpf/      # WPF application host with MVVM
  Ogur.Capabilities.Fishing/  # Core fishing capability (domain logic)
  Ogur.Infrastructure/        # Infrastructure implementations (input, memory, signals)
  Ogur.Abstractions/          # Cross-cutting abstractions (NuGet package)
  Ogur.Core/                  # Hub integration (NuGet package)
  Ogur.Core.Metin/           # Metin2-specific utilities (legacy + modern)
```

## Build & Run
```bash
# Restore dependencies
dotnet restore Ogur.Fishing.sln

# Build solution
dotnet build Ogur.Fishing.sln -c Release

# Run application
dotnet run --project Ogur.Fishing.Host.Wpf
```

## Configuration

Configuration is managed via `appsettings.json`:
```json
{
  "Hub": {
    "HubUrl": "https://api.hub.ogur.dev",
    "ApiKey": "your-api-key",
    "ApplicationName": "OgurFishing"
  },
  "Fishing": {
    "Legacy": {
      "KnownKeys": ["|cfffff400", "|cffff3219"],
      "KnownCountPhrases": ["1 spacji", "2 spacji", "3 spacji", "4 spacji", "5 spacji"]
    }
  },
  "ChatDetection": {
    "ScanStart": 214958080,
    "ScanEnd": 224395264,
    "SnapshotCount": 100,
    "IntervalMs": 50,
    "MinChangeCount": 10,
    "RegionGroupingGap": 1024,
    "ReadChunkSize": 4096
  }
}
```

## Roadmap

### In Progress
- [x] Automatic chat buffer detection
- [x] Real-time fishing automation
- [x] Multi-server support
- [x] Hub integration (auth, licensing, telemetry)
- [x] Professional WPF UI with animations

### Planned
- [ ] Multi-character support (switch between processes)
- [ ] Advanced statistics and analytics
- [ ] Configurable timings and delays
- [ ] Memory address auto-detection improvements
- [ ] Additional bot capabilities (AutoLoot, Heal, etc.)

## License

Proprietary - All rights reserved