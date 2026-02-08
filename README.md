# Ogur.Fishing

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![WPF](https://img.shields.io/badge/WPF-Windows-512BD4?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)

Professional fishing automation bot for Metin2 built with Clean Architecture and integrated with Ogur.Hub centralized management.

## Features

### Core Capabilities
- **Automatic Chat Buffer Detection**: Differential memory scanning (100 snapshots) to auto-detect fishing messages
- **Real-time Fishing Automation**: Event-driven cast → wait → hook → loot cycle
- **Multi-Server Support**: 9 game servers (Proxima, Tamidia2, Glador, Glevia2, Monastyr2, MT2009, Pandora, Projekt Hard, Senthia)
- **Professional WPF UI**: Dark theme with Material Design styling and real-time event logging

### Hub Integration
- JWT authentication with license validation
- Device fingerprinting (HWID + GUID, 1 license = 2 devices)
- Real-time SignalR commands (logout, block, notify, force update)
- Automatic updates with required version enforcement
- Telemetry reporting

### Technical Highlights
- Clean Architecture: Domain → Application → Infrastructure → Presentation
- CQRS pattern with EventBus for cross-cutting concerns
- Win32 API integration for memory reading and keyboard input
- Polish character support (Windows-1250 encoding)
- MVVM pattern with CommunityToolkit.Mvvm

## Solution Structure
```
Ogur.Fishing.sln
├── Ogur.Fishing.Host.Wpf/         # WPF host with MVVM
├── Ogur.Capabilities.Fishing/     # Core fishing domain logic
├── Ogur.Infrastructure/           # Win32 implementations (input, memory)
├── Ogur.Abstractions/             # Cross-cutting abstractions (NuGet)
├── Ogur.Core/                     # Hub integration (NuGet)
└── Ogur.Core.Metin/              # Metin2-specific utilities
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

Edit `appsettings.json`:
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

## Architecture

### Fishing State Machine
```
Stopped
  ↓
Casting (F2 bait + Space)
  ↓
Waiting (monitor memory for bite)
  ↓
Hooking (Space × N times)
  ↓
Looting (animation delay)
  ↓
Casting (loop)
```

### EventBus Events
- `fishing.start` - Automation started
- `fishing.stop` - Automation stopped
- `fishing.cast.request` - Rod casting initiated
- `fishing.waiting` - Waiting for bite
- `fishing.bite` - Bite detected with hook count
- `fishing.hook.request` - Hooking fish
- `fishing.timeout` - No bite detected
- `fishing.error` - Error occurred

## Roadmap

**Completed:**
- Automatic chat buffer detection
- Real-time fishing automation
- Multi-server support
- Hub integration (auth, licensing, telemetry)
- Professional WPF UI with animations

**Planned:**
- Multi-character support (switch between processes)
- Advanced statistics and analytics
- Configurable timings and delays
- Memory address auto-detection improvements
- Additional bot capabilities (AutoLoot, Heal)

## Sub-Projects

### [Ogur.Capabilities.Fishing](./Ogur.Capabilities.Fishing)
Core fishing capability plugin implementing the fishing automation state machine.

### [Ogur.Fishing.Host.Wpf](./Ogur.Fishing.Host.Wpf)
WPF application host with MVVM pattern, authentication flow, and Hub integration.

### [Ogur.Infrastructure](./Ogur.Infrastructure)
Infrastructure layer providing Win32 API implementations for input, memory, and window management.

## License
Proprietary - All rights reserved
