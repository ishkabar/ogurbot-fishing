# Ogur.Infrastructure

[![wakatime](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing.svg?style=flat-square)](https://wakatime.com/badge/github/ishkabar/ogurbot-fishing)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat-square&logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)

Infrastructure layer providing Win32 API implementations for bot abstractions.

## Responsibilities

### Input Simulation
- **Win32Input**: Keyboard input via Win32 SendInput API
- Scan code-based input (compatible with Metin2 anti-cheat)
- 30ms delay between KeyDown/KeyUp for game detection
- Integration with legacy `Button.PressKey2()` for compatibility

### Memory Operations
- **ProcessMemoryReader**: ReadProcessMemory wrapper for game memory access
- Windows-1250 encoding support for Polish characters
- Null-terminated string reading with error handling
- Safe memory access with bounds checking

### Window Management
- **WindowActivator**: SetForegroundWindow + ShowWindow for game focus
- Required before sending keyboard input to Metin2 client
- Window state restoration

### Bite Detection
- **MemoryBiteSignalSource**: Monitors game memory for fishing messages
- Automatic chat buffer detection on first use
- Pattern matching against known Polish fishing phrases
- Returns hook count (1-5 spaces) based on message content

### Chat Detection
- **DifferentialChatBufferDetector**: Differential memory scanning algorithm
- 100 snapshots at 50ms intervals
- Byte-by-byte change tracking to identify chat buffer
- Validates regions using Metin2 color code pattern (`|cff`)
- Returns MessageStart and Digit addresses

## Key Classes

| Class | Purpose |
|-------|---------|
| `Win32Input` | Keyboard input via SendInput API |
| `WindowActivator` | Window focus management |
| `ProcessMemoryReader` | Memory reading utilities |
| `MemoryBiteSignalSource` | Fishing bite detection |
| `DifferentialChatBufferDetector` | Chat buffer auto-detection |
| `InMemoryEventBus` | In-memory pub/sub event bus |

## Project Structure
```
Ogur.Infrastructure/
├── Input/
│   └── Win32Input.cs
├── Memory/
│   └── Win32ProcessMemoryReader.cs
├── Windows/
│   └── WindowActivator.cs
├── Signals/
│   └── MemoryBiteSignalSource.cs
├── Events/
│   └── InMemoryEventBus.cs
└── Configuration/
    └── FishingOptions.cs
```

## Platform
- **Target**: net8.0-windows
- **Win32 APIs**: kernel32.dll, user32.dll
- **Encoding**: Windows-1250 (CodePagesEncodingProvider)

## Usage
```csharp
// Input simulation
var input = new Win32Input();
await input.PressAsync(InputKey.Space);

// Memory reading
var reader = new ProcessMemoryReader(process);
var text = reader.ReadNullTerminatedString(address, Encoding.GetEncoding(1250));

// Window activation
var activator = new WindowActivator();
activator.Activate(process.MainWindowHandle);

// Bite detection
var signalSource = new MemoryBiteSignalSource(reader, logger, options);
var hookCount = await signalSource.WaitForBiteAsync(ct);
```

## Dependencies
- `Tesseract` - OCR engine (future use)
- `SharpDX.DXGI` - Screen capture (future use)
- `System.Text.Encoding.CodePages` - Windows-1250 encoding
