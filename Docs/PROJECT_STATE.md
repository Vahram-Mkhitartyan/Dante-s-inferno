# Project State

Last reviewed: October 19, 2026

## Summary

*Dante's Inferno* is currently a playable 2D action prototype built around Spine-based characters, melee combat, reactive enemies, and a seven-axis behavior-driven sin foundation.

The project has a functioning vertical slice, but several systems still contain transitional or duplicated architecture. The next development phase should stabilize the foundations before expanding content.

## Technical Snapshot

- Unity: `6000.2.8f1`
- Render pipeline: Universal Render Pipeline 2D
- Character animation: Spine and Fantazia Character Editor
- Water: Modern Water 2D
- Main branch: `main`
- Current committed milestone: `V.0.2.2`

The Unity Editor currently compiles the project successfully.

## Scenes

### Limbo

The primary combat and sin-system sandbox. It contains the player, enemies, hostility logic, deterministic seed management, and current world systems.

Limbo, Acheron, and Training Camp are enabled in Build Settings.

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

The authoritative model tracks all seven sins simultaneously through independent influence, pressure, and mastery axes. It preserves a fixed Origin Sin, stabilizes changes to Current Dominant Sin, publishes read-only snapshots, and aggregates local pressure into overall pressure.

Player actions now produce contextual records with run-scoped event identities. Separate Wrath and Greed evaluators convert those records into state deltas. `WorldReactionDirector` consumes authoritative snapshots while the existing scalar momentum API remains available through a compatibility bridge.

Versioned save data preserves all axes, deterministic seed progress, processed events, collapse history, Death corruption, companion choice, and resolved Greed temptations. Wrath is connected to current combat; Greed has a reusable temptation choice component and evaluator contract but still needs authored scene content and presentation.

## Strong Foundations

- Event-driven connection between combat and sin state
- Reusable enemy movement and attack interfaces
- Central hit resolution
- Deterministic run seed support
- Direct-scene runtime bootstrap and isolated persistent services
- Versioned save schema and rules versioning
- Contextual action evaluation with duplicate protection
- Fifteen deterministic Edit Mode tests
- Data-oriented direction through `SinDefinition`
- Separate world-reaction layer
- Respawn flow rather than destroying the player permanently

## Known Technical Debt

- Locomotion and action animations are controlled by two different animation components.
- Legacy and replacement knockback components coexist.
- Gameplay uses hardcoded legacy input even though Unity Input System is installed.
- Tutorial tracking duplicates gameplay key checks.
- Some legacy global scene searches and singleton consumers remain.
- Some components assume required references exist and can fail during `Awake` if prefab wiring changes.
- Backstabber damage and knockback settings are currently unused; betrayal only changes hostility.
- Scene ordering and narrative transitions are not yet authored as a complete campaign flow.

## Immediate Priorities

1. Author a visible Greed temptation encounter using `GreedTemptation`.
2. Move Reflection from the scalar API to authoritative snapshots.
3. Add qualified encounter weighting and mastery evidence limits.
4. Consolidate player animation ownership.
5. Complete migration to one knockback system.
6. Introduce an input abstraction based on Unity Input System.
7. Define the intended order and transition rules for all scenes.
