# Project State

Last reviewed: October 18, 2026

## Summary

*Dante's Inferno* is currently a playable 2D action prototype built around Spine-based characters, melee combat, reactive enemies, and an early behavior-driven sin system.

The project has a functioning vertical slice, but several systems still contain transitional or duplicated architecture. The next development phase should stabilize the foundations before expanding content.

## Technical Snapshot

- Unity: `6000.2.8f1`
- Render pipeline: Universal Render Pipeline 2D
- Character animation: Spine and Fantazia Character Editor
- Water: Modern Water 2D
- Main branch: `main`
- Current milestone: `V.0.2.1`
- Custom scripts: approximately 52 files and 3,600 lines

The Unity Editor currently compiles the project successfully.

## Scenes

### Limbo

The primary combat and sin-system sandbox. It contains the player, enemies, hostility logic, deterministic seed management, and current world systems.

`Limbo` is currently the only scene enabled in Build Settings.

### Acheron

A river and boat introduction featuring environmental presentation, camera movement, rowing motion, and whispered narrative text.

### Training Camp

An early tutorial sequence that introduces movement, attacks, defense, and the four enemy archetypes.

## Current Gameplay

### Player

- Horizontal movement and jumping
- Directional facing
- Blocking from the front
- Three melee attacks
- Buffered attack inputs
- Damage, knockback, hurt, death, and respawn behavior
- Camera return after respawn

### Combat

- `J`: top-down swing
- `K`: upward launcher
- `L`: forward pierce
- Centralized hit resolution
- Timed hit windows
- Damage scaling from the sin system
- Knockback reactions for the player and enemies

### Enemies

The current reusable enemy foundation consists of `EnemyController`, `EnemyState`, `EnemyPerception`, `IEnemyMovement`, and `IEnemyAttack`.

Implemented archetypes:

- Zombie: basic pursuit and attack
- Dasher: committed dash behavior
- Giant: slow movement and heavy committed attack
- Thief: movement and armor-stealing behavior

Enemies can also react to repeated player behavior through counterattacks and can be selected as potential backstabbers.

### Sin System

The current implementation tracks one dominant sin, global momentum, global stability, a temporary sin state, collapse count, and a threshold-based Pure Form flag.

Player actions are reported through `PlayerActionEvents`. `PlayerBehaviorTracker` detects repeated, alternating, and overcommitted attack patterns. `WorldReactionDirector` translates those signals into pressure consumed by `HostilityDirector`.

This is a useful prototype of the intended feedback loop, but it does not yet represent all seven sins simultaneously or evaluate long-term mastery.

## Strong Foundations

- Event-driven connection between combat and sin state
- Reusable enemy movement and attack interfaces
- Central hit resolution
- Deterministic run seed support
- Data-oriented direction through `SinDefinition`
- Separate world-reaction layer
- Respawn flow rather than destroying the player permanently

## Known Technical Debt

- Locomotion and action animations are controlled by two different animation components.
- Legacy and replacement knockback components coexist.
- Gameplay uses hardcoded legacy input even though Unity Input System is installed.
- Tutorial tracking duplicates gameplay key checks.
- Global scene searches, singletons, and runtime component creation create initialization-order risks.
- Persistent managers preserve their root object, potentially carrying scene-specific children between scenes.
- Some components assume required references exist and can fail during `Awake` if prefab wiring changes.
- Backstabber damage and knockback settings are currently unused; betrayal only changes hostility.
- No project-specific automated tests or custom assembly definitions exist.
- A complete build and scene-transition flow has not been established.

## Immediate Priorities

1. Establish a clear bootstrap and persistent-system lifecycle.
2. Consolidate player animation ownership.
3. Complete migration to one knockback system.
4. Introduce an input abstraction based on Unity Input System.
5. Redesign the sin state model before adding more sin-specific content.
6. Add focused tests for deterministic gameplay logic.
7. Define the intended order and transition rules for all scenes.

