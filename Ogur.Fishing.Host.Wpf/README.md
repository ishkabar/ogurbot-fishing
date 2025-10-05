# Ogur.Fishing.Host.Wpf

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)

## Overview
**Ogur.Fishing.Host.Wpf** is a WPF MVVM-based host for running and debugging Ogur bot capabilities.  
It provides an interface for user authentication, server selection, and runtime state visualization.

## Architecture
- **MVVM:** based on `CommunityToolkit.Mvvm`.  
- **Views:** `LoginView`, `ServerSelectView`, `MainView`.  
- **Services:** `FishingActionExecutor`, `SelectedProcessAccessorAdapter`, `NullInput`.  
- **DI:** Microsoft.Extensions.DependencyInjection, ILogger<T>, IOptions<T>.

## Usage
Build and run `Ogur.Fishing.Host.Wpf`. Configure appsettings.json to point to capability plugins and signal sources.

## License
MIT License © Ogur Project
