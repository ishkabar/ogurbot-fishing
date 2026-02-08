# Ogur.Capabilities.Fishing

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)

Core fishing capability plugin implementing the fishing automation state machine.

## Responsibilities
- Fishing state machine lifecycle management
- EventBus integration for cross-cutting communication
- Bait selection and rod casting coordination
- Bite detection via memory scanning
- Hook execution with configurable timing

## State Machine
```
Stopped
  ↓
Casting (F2 bait + Space)
  ↓
Waiting (monitor memory for bite)
  ↓
Hooking (Space × N times based on message)
  ↓
Looting (delay for animation)
  ↓
Casting (loop)
```

## Events Published
| Event | Description |
|-------|-------------|
| `fishing.start` | Fishing automation started |
| `fishing.stop` | Fishing automation stopped |
| `fishing.cast.request` | Rod casting initiated |
| `fishing.waiting` | Waiting for bite signal |
| `fishing.bite` | Bite detected with hook count |
| `fishing.hook.request` | Hooking fish |
| `fishing.timeout` | No bite detected (timeout) |
| `fishing.error` | Error occurred |

## Dependencies
- `Ogur.Abstractions` - IApplicationCapability, IEventBus
- `Microsoft.Extensions.Logging` - Structured logging
- `Microsoft.Extensions.Options` - Configuration binding

## Usage
```csharp
// Register capability
services.AddSingleton<IApplicationCapability, FishingCapability>();

// Start fishing
var capability = serviceProvider.GetRequiredService<IApplicationCapability>();
await capability.StartAsync(new CapabilityStartContext(), CancellationToken.None);

// Subscribe to events
eventBus.Subscribe("fishing.*", (type, message) => 
{
    Console.WriteLine($"[{type}] {message}");
});
```

## Project Structure
```
Ogur.Capabilities.Fishing/
├── FishingCapability.cs    # State machine implementation
├── FishingPlugin.cs        # IApplicationPlugin registration
└── GlobalUsings.cs         # Common using directives
```
