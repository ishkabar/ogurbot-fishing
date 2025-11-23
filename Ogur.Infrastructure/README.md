# Ogur.Infrastructure

Infrastructure layer providing OS-specific implementations for bot abstractions.

## Responsibilities

### Input Simulation
- `Win32Input`: Keyboard input via Win32 API using scan codes
- Integration with legacy `Button.PressKey2()` for Metin2 compatibility
- 30ms delay between KeyDown/KeyUp for game detection

### Memory Operations
- `ProcessMemoryReader`: ReadProcessMemory wrapper for game memory access
- Windows-1250 encoding support for Polish characters
- Null-terminated string reading with error handling

### Window Management
- `WindowActivator`: SetForegroundWindow + ShowWindow for game focus
- Required before sending keyboard input to Metin2 client

### Bite Detection
- `MemoryBiteSignalSource`: Monitors game memory for fishing messages
- Automatic chat buffer detection on first use
- Pattern matching against known Polish fishing phrases
- Returns hook count (1-5 spaces) based on message content

### Chat Detection
- `DifferentialChatBufferDetector`: Differential memory scanning algorithm
- Takes 100 snapshots at 50ms intervals
- Compares byte-by-byte changes to identify chat buffer
- Validates regions using Metin2 color code pattern (|cff)
- Returns MessageStart and Digit addresses

## Key Classes

- `Win32Input`: Keyboard input implementation
- `WindowActivator`: Window focus management
- `ProcessMemoryReader`: Memory reading utilities
- `MemoryBiteSignalSource`: Fishing bite detection
- `DifferentialChatBufferDetector`: Chat buffer auto-detection

## Platform

- Targets: net8.0-windows
- Win32 API: kernel32.dll, user32.dll
- Encoding: Windows-1250 (CodePagesEncodingProvider)