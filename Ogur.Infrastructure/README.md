# Ogur.Infrastructure

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)

## Overview
**Ogur.Infrastructure** provides platform-dependent implementations for the Ogur bot ecosystem.  
It bridges abstractions defined in `Ogur.Abstractions` with actual Windows API integrations.

## Components
- **Input:** `Win32Input` and `NullInput` implementations using SendInput.  
- **Memory:** `NullProcessMemoryReader` (placeholder) and future `ReadProcessMemory` backend.  
- **Signals:** `MemoryBiteSignalSource` for fishing event detection.  
- **Windows:** `WindowActivator` for foreground window management.

## Development
Infrastructure components are registered via DI and can be replaced or extended independently.  
Supports modular testing and host-level composition.

## License
MIT License © Ogur Project
