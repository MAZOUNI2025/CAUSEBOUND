# Phase 2 InteractionSandbox Tests

The internal scene is `Assets/Scenes/InteractionSandbox.unity`. It contains generic stateful objects named Lever, Bridge, and Gate, plus the reusable pointer interaction path, CauseEffectGraph, LevelResetController, and fixed camera.

## Test A — Single Cause to Single Effect

Configure or run the sandbox checks with Lever as the cause and Gate as the effect. The expected result is `Lever: Off → On` and `Gate: Closed → Open`.

## Test B — Chained Cause to Effect

Configure Lever → Bridge and Bridge → Gate. The expected result is `Lever: Off → On`, then `Bridge: Raised → Lowered`, then `Gate: Closed → Open`. The graph processes a queue rather than recursively calling through the chain.

## Test C — Single Cause to Multiple Effects

Configure Lever with two ordered effects: Bridge → Lowered and Gate → Open. The expected result is that both effects are applied deterministically from one cause transition.

## Test D — Reset

After changing any combination of object states, call `LevelResetController.ResetLevel()` or press `R` through `SandboxResetInput` in the editor. Every resettable child returns to its serialized initial state without restarting the application.

## Loop safety

`CauseEffectGraph` processes pending state changes iteratively and stops after `maxTransitionsPerDispatch` transitions, clearing the queue and emitting a warning. This prevents uncontrolled recursive chains from freezing the game.

## Validation status

The interaction contracts, state transitions, reset path, data-driven graph, multiple-effect order, chained-effect queue, loop guard, and sandbox references were checked at file/code level. Unity runtime execution and editor ContextMenu execution require a valid Unity license and must be verified by the human developer on a licensed machine.
