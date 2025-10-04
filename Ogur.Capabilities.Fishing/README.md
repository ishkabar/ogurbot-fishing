# Ogur.Capabilities.Fishing

The **Fishing capability plugin**.  
Implements `IBotCapability` from `ogur.Abstractions`.

## Responsibilities
- Define the FSM for fishing (with `Stateless`).
- Expose configuration (`IOptions<FishingOptions>`).
- Emit bot events (`BotEvent`) to host/orchestrator.
- Implement Start, Pause, Stop lifecycle.

## Dependencies
- `ogur.Abstractions`
- `ogur.Core`
- `Stateless`
- `Microsoft.Extensions.Options`
- `Microsoft.Extensions.Logging.Abstractions`
