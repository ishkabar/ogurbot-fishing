# Ogur.Infrastructure

Infrastructure layer for ogur bots.  
Provides OS-specific implementations for low-level abstractions.

## Responsibilities
- Input simulation (`IInput` via SendInput).
- Screen capture (`IScreenCapture` via DXGI/BitBlt).
- OCR (`IOcr` via Tesseract).
- Logging support.

## Notes
- Currently stubbed – extend with real implementations.
- Targets `net8.0-windows`.
