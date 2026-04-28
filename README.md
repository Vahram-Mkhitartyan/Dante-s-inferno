# Dante's Inferno

A 2D Unity action game inspired by *Dante's Inferno*, built around fast melee combat, reactive enemies, and a morality system where the player's dominant sin becomes a gameplay force instead of just a story label.

## Current Focus

This update restructures the game around a clearer combat and sin-system foundation:

- Player actions generate sin momentum.
- High momentum increases power, but also raises instability.
- Repeated or careless behavior makes enemies respond more aggressively.
- Success is about controlled intensity: using power without losing control.

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

The sin system tracks:

- `DominantSin`
- `Momentum`
- `Stability`
- `SinState`
- `CollapseCount`
- `Pure Form`

Player actions push momentum upward. Momentum increases damage potential, but unstable play lowers control. The system is designed to reward high-risk controlled play rather than safe play or runaway spamming.

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
- `Assets/scripts/player` - player input, movement, damage reaction, respawn
- `Assets/scripts/enemy` - enemy state, attacks, movement, counterattacks
- `Assets/Scenes` - current playable scenes
- `Assets/prefabs` - enemy prefabs
- `Assets/FantaziaCharacterEditor/Prefabs/main_hero.prefab` - main player prefab

## Major Update Notes

This push includes:

- A new event layer for player actions.
- A data-driven `SinDefinition` structure for future sin tuning.
- A `PlayerBehaviorTracker` for repeated patterns and overcommitment.
- A `WorldReactionDirector` for connecting sin state to enemy/world pressure.
- Refactored attack types: `TopDownSwing`, `UpwardLauncher`, `ForwardPierce`.
- Reworked player and enemy knockback.
- Player death animation and respawn handling.
- Improved camera behavior around respawn.
- More reliable enemy counterattack behavior.

## Development Status

This is an active prototype. The current goal is to make the core combat loop feel good, then deepen the sin system into a full run-defining mechanic with unique behavior per dominant sin.
