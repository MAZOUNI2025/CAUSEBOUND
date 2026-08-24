# CAUSEBOUND

CAUSEBOUND is a Unity 6 mobile-first project targeting Android in portrait orientation. This repository currently contains the Phase 1 project foundation only; gameplay mechanics and the five production levels belong to later phases.

## Unity

The project targets **Unity 6000.0.81f1**. Android settings are prepared for portrait orientation and ARM64.

## Foundation

The project separates core game state, level data, runtime level state, object state and reset contracts, player spawn markers, fixed camera behavior, input routing, UI routing, local save access, and replaceable audio, analytics, and monetization service contracts.

## Scenes

Build Settings contain the following scenes in order:

1. `Assets/Scenes/Bootstrap.unity`
2. `Assets/Scenes/MainMenu.unity`
3. `Assets/Scenes/Gameplay.unity`

## Deliberately not implemented

Cause-and-effect gameplay, box, pressure plate, lever, gate, bridge, chain, levels 1–5, complete hints, stars, progression, advertisements, analytics SDKs, enemies, combat, inventory, shop, energy, multiplayer, accounts, and cloud save are intentionally not part of this phase.
