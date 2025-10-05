# Ogur.Capabilities.Fishing

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)

## Overview
**Ogur.Capabilities.Fishing** implements the fishing automation logic for Metin2 using the Ogur modular bot framework.  
It is built on top of `Ogur.Core` FSM and consumes input, signal, and session services via dependency injection.

## Architecture
- **FSM:** handles fishing states (Idle → Cast → Wait → Catch).  
- **Signal Sources:** memory and visual cues processed by `MemoryBiteSignalSource`.  
- **Configuration:** via `FishingMemoryOptions` and appsettings integration.  

## Development
- Extendable via `IBotCapability` interface.  
- Integrated with `Ogur.Infrastructure` for I/O abstractions.  
- Includes experimental memory-driven mode (not yet functional).

## License
MIT License © Ogur Project
