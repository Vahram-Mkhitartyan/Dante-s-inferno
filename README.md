# Dante's Inferno

A 2D Unity action game inspired by *Dante's Inferno*, built around fast melee combat, reactive enemies, and a morality system where the player's dominant sin becomes a gameplay force instead of just a story label.

## Current Focus

The current update integrates the seven-axis sin architecture with the playable combat loop:

- Every gameplay action receives a stable run-scoped event identity.
- Contextual evaluators translate behavior into influence, pressure, and mastery evidence.
- Wrath reacts to deliberate attacks, repetition, blocking, damage, and unnecessary kills.
- Greed uses the same contracts for collecting, rejecting, and investing temptation rewards.
- Existing momentum, power, hostility, and Reflection behavior remains available through a compatibility layer.

## Core Mechanics

### Combat

The player currently has three main attacks:

- `J` - Top-down sword swing  
  A controlled grounded hit with moderate knockback.

- `K` - Upward launcher  
  Pops enemies upward and creates space.

- `L` - Forward pierce  
  A stronger straight-line thrust with high horizontal knockback.

The combat system supports buffered attacks, hit timing, knockback, enemy reactions, and anti-spam counter pressure.

### Sin System

The authoritative sin model tracks:

- fixed Origin Sin and dynamic Current Dominant Sin
- seven independent influence, pressure, and mastery axes
- normalized composition and overall instability
- run, schema, rules, event, and deterministic-seed state
- collapse history, Death corruption, and companion choice

The previous scalar API remains active while gameplay systems migrate to immutable snapshots. Versioned JSON save data preserves the authoritative model without serializing scene references.

### Enemy Reactions

Enemies react to player behavior through:

- hostility scaling
- backstabber activation
- spam counterattacks
- knockback response
- movement pauses during knockback

Enemy movement now respects knockback so hits feel more physical and readable.

### Respawn

The player now respawns instead of being destroyed on death. Respawn includes:

- death animation
- delayed revive
- velocity reset
- input delay after respawn
- camera return to the respawn framing over roughly two seconds

## Controls

- `A / D` - Move
- `W` - Jump
- `Left Shift` - Defend / block
- `J` - Top-down swing
- `K` - Upward launcher
- `L` - Forward pierce

## Project Setup

Unity version:

```text
6000.2.8f1
```

Open the project from the repository root in Unity:

```text
DantesInferno/
```

Important folders:

- `Assets/scripts/Combat` - attack execution, health, hit resolving, knockback bridge
- `Assets/scripts/Core` - sin system, camera, behavior tracking, world reactions
- `Assets/scripts/SinSystem` - authoritative state, action contracts, evaluators, and save schema
- `Assets/Tests/EditMode` - deterministic sin-system tests
- `Assets/scripts/player` - player input, movement, damage reaction, respawn
- `Assets/scripts/enemy` - enemy state, attacks, movement, counterattacks
- `Assets/Scenes` - current playable scenes
- `Assets/prefabs` - enemy prefabs
- `Assets/FantaziaCharacterEditor/Prefabs/main_hero.prefab` - main player prefab

## Major Update Notes

Current foundation work includes:

- automatic direct-scene bootstrap for `SinManager` and `SeedManager`
- independently persistent services that cannot carry scene-only siblings
- contextual action records with duplicate-event protection
- data-driven Wrath and Greed evaluators
- snapshot-driven world pressure with legacy compatibility
- versioned save/load support and deterministic RNG restoration
- an Edit Mode test command at `Tools > Dante's Inferno > Run Sin System Tests`

## Development Status

This is an active prototype. The current goal is to make the core combat loop feel good, then deepen the sin system into a full run-defining mechanic with unique behavior per dominant sin.

## Documentation

- [Current project state](Docs/PROJECT_STATE.md)
- [Sin system specification](Docs/SIN_SYSTEM_SPEC.md)
- [Sin system integration plan](Docs/SIN_SYSTEM_INTEGRATION.md)
- [Sin system decisions](Docs/SIN_SYSTEM_DECISIONS.md)
