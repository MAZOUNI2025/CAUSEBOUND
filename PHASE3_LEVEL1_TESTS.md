# Phase 3 — Level 1 Vertical Slice Tests

Scene: `Assets/Scenes/Level01.unity`

The player-facing objective is **REACH THE EXIT**. The intended discovery chain is:

`Lever: Off → On` → `Bridge: Raised → Lowered` → `SecondaryMechanism: Initial → Activated` → `Gate: Closed → Opening → Open` → `Exit: Locked → Available` → `Level Complete`.

## Static/code checks

The source and serialized scene are checked for the required objects, references, state names, reusable `CauseEffectGraph` rules, pointer movement path, access gating, gate transition, exit completion, reset reuse, no timer, and no monetization. The Phase 3 file test script is outside the Unity project and is not a shipped runtime system.

## Editor/runtime checks for the human developer

Open `Level01.unity` on a licensed Unity 6000.0.81f1 installation and allow the project to import. Confirm that all serialized references are assigned and that the scene opens without Console errors.

1. **Level load:** Confirm Player, Ground, Lever, Bridge, SecondaryMechanism, Gate, Exit, LevelCamera, HUD, CauseEffectGraph, and LevelResetController are present.
2. **Movement:** Tap or click the ground and confirm the blue player capsule moves to the selected destination.
3. **First interaction:** Tap or click Lever and confirm it changes from orange `Off` to green `On`.
4. **First cause/effect:** Confirm Bridge changes from yellow raised presentation to cyan lowered presentation.
5. **Access:** Before Bridge is lowered, SecondaryMechanism should reject interaction. After Bridge is lowered, it should accept interaction.
6. **Second interaction:** Activate SecondaryMechanism and confirm it changes from purple `Initial` to mint `Activated`.
7. **Second cause/effect:** Confirm Gate changes `Closed → Opening → Open` and its visual narrows/changes color.
8. **Exit:** Confirm Exit changes from locked gray to available cyan.
9. **Completion:** Move the player into Exit and confirm `LEVEL COMPLETE` appears.
10. **Reset:** Press `R` or the HUD RESET button after changing states. Confirm Lever is `Off`, Bridge is `Raised`, SecondaryMechanism is `Initial`, Gate is `Closed`, and Exit is `Locked`.
11. **Replay:** Solve the sequence again after Reset.
12. **Loop safety:** Confirm the reusable graph uses queued processing and its transition budget; no level-specific core conditionals are present.

Runtime, visual, and Android tests are not claimed by this document because the available environment does not provide a valid Unity Editor entitlement for project execution.
