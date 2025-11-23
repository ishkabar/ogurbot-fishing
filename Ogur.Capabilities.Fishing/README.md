# Ogur.Capabilities.Fishing

The core fishing capability plugin implementing the fishing automation state machine.

## Responsibilities

- Fishing state machine: Stopped -> Casting -> Waiting -> Hooking -> Looting
- EventBus integration for cross-cutting communication
- Bait selection and rod casting coordination
- Bite detection via memory scanning
- Hook execution with configurable timing

## State Machine
```
Stopped
  |
  v
Casting (F2 bait + Space)
  |
  v
Waiting (monitor memory for bite)
  |
  v
Hooking (Space x N times)
  |
  v
Looting (delay for animation)
  |
  v
Casting (loop)
```

## Events Published

- `fishing.start`: Fishing automation started
- `fishing.stop`: Fishing automation stopped
- `fishing.cast.request`: Rod casting initiated
- `fishing.waiting`: Waiting for bite signal
- `fishing.bite`: Bite detected with hook count
- `fishing.hook.request`: Hooking fish
- `fishing.timeout`: No bite detected (timeout)
- `fishing.error`: Error occurred

## Dependencies

- Ogur.Abstractions (IApplicationCapability, IEventBus)
- Microsoft.Extensions.Logging
- Microsoft.Extensions.Options