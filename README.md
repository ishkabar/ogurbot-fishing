# Ogur.Fishing Solution

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)

## Overview
**Ogur.Fishing** is the main solution hosting all components of the Ogur modular bot system.  
It demonstrates a fully decoupled architecture, where each capability (Fishing, AutoLoot, Heal, etc.) acts as an independent plugin.

## Architecture
The solution includes multiple projects:
- `Ogur.Abstractions` — shared contracts.
- `Ogur.Core` — runtime orchestration and FSM engine.
- `Ogur.Capabilities.Fishing` — fishing automation capability.
- `Ogur.Infrastructure` — platform integration (input, screen, memory).
- `Ogur.Fishing.Host.Wpf` — WPF host with MVVM UI.

## Development
Each capability can be built, packaged, and distributed as a standalone module or used by the multibot orchestrator.  
The `WPF` host provides visual debugging and configuration.

## License
MIT License © Ogur Project
