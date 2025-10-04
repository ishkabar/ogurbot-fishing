# Ogur.Fishing

Ogur.Fishing is a modular bot capability for **Metin2**, built on the `ogur` framework.  
It can run as a **standalone host (WPF UI)** or be plugged into a **multi-bot orchestrator**.

## Features
- Built on **.NET 8** with **WPF (MVVM)**.
- Uses `ogur.Abstractions` and `ogur.Core` (NuGet).
- Provides the Fishing capability as a plugin.
- Extensible architecture (more bots like AutoLoot, Heal).
- Infrastructure layer ready for Input, Screen Capture, OCR.

## Solution Structure
```
/src
  Ogur.Fishing.sln
  Ogur.Fishing.Host.Wpf/      
  Ogur.Capabilities.Fishing/  
  Ogur.Infrastructure/        
```

## Build & Run
```bash
dotnet restore Ogur.Fishing.sln
dotnet build Ogur.Fishing.sln -c Debug
dotnet run --project Ogur.Fishing.Host.Wpf
```

## Roadmap
- [ ] Real login & HWID check (VPS DB).
- [ ] Fishing FSM (waiting → biting → catching).
- [ ] Configurable overlay (debug).
- [ ] More bot capabilities (AutoLoot, Heal).
- [ ] Integration with multi-bot orchestrator.
