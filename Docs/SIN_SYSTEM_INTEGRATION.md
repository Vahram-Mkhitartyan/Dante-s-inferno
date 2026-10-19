# Sin System Integration Plan

Status: Foundation integrated; Wrath feedback playable; Greed contract validated

## Implementation Progress

Completed on October 19, 2026:

- Seven-axis authoritative state model and immutable snapshots
- Direct-scene bootstrap and isolated persistent manager lifecycle
- Compatibility bridge preserving existing scalar gameplay behavior
- Contextual action records with run-scoped event identities
- Duplicate-event and repeated-Greed-temptation protection
- Versioned save data with rules-version validation
- Deterministic seed and random-draw restoration
- Data-driven Wrath evaluator connected to combat actions
- Data-driven Greed evaluator and reusable temptation choice component
- Snapshot-driven world pressure
- Automatic scene-local Wrath lighting and camera-color response
- Non-numerical Death hints on rising Wrath thresholds when dialogue is available
- `F8` editor/development snapshot diagnostics
- Spine track resynchronization after jumps and action locks
- Twenty passing deterministic Edit Mode tests

The next presentation task is to identify or author a real mirror target for snapshot-driven corruption. The next content task is a visible Greed temptation encounter, and the next architecture migration is Reflection consuming snapshots directly.

## Assessment

The proposed seven-sin system is strongly aligned with the project's existing direction, but it cannot be implemented by extending the current scalar `Sin` value alone.

Integration ratings:

| Area | Rating |
|---|---:|
| Conceptual fit | 9/10 |
| Existing architectural fit | 7/10 |
| Direct compatibility without refactoring | 4/10 |
| Incremental feasibility | 8/10 |
| Overall | 8/10 |

Approximately half of the current surrounding architecture can be retained. The authoritative sin-state model and mastery logic require redesign.

## Reusable Systems

### PlayerActionEvents

Retain the event-driven boundary, but replace narrowly typed notifications with richer contextual action records where necessary.

### PlayerBehaviorTracker

Retain its bounded history and pattern detection for the Wrath slice. It should become one source of evidence rather than the final moral classifier.

### SinDefinition

Evolve this concept into configurable per-sin profiles. Profiles should contain tuning and rules, not runtime state.

### AttackExecutor

Continue reporting completed gameplay actions. It should not calculate sin values or mastery.

### WorldReactionDirector

Retain the separation between authoritative sin state and world reactions. It should consume a stable state snapshot rather than directly reading several mutable manager properties.

### HostilityDirector and EnemyCounterAttack

Retain these as responders to world pressure and player patterns. They should not determine whether the player has mastered a sin.

### SeedManager

Retain deterministic random selection. Generation must include a rules-version identifier if reproducibility is expected across future balance changes.

### SinReflection

Retain its responsibility for visual translation, but replace scalar thresholds with a read-only authoritative sin snapshot.

## Systems Requiring Redesign

### SinManager

The manager currently owns one momentum value, one stability value, one derived integer, one state, and one threshold-based Pure Form flag.

It should instead coordinate a dedicated runtime model containing:

- Fixed Origin Sin and dynamic Current Dominant Sin
- Seven influence weights
- Normalized composition
- Intensity state
- Current instability state
- Mastery evidence and progress
- Run and schema version information

The manager should expose commands and snapshots rather than allowing unrelated components to mutate fields directly.

### Action Evaluation

Introduce an evaluation boundary between gameplay events and sin state.

An action record may include:

- Action type
- Source and target
- Encounter identity
- Timestamp
- Outcome
- Repetition history
- Current danger
- Available alternatives
- Reward or resource context

Evaluators convert these records into influence, intensity, control, and mastery evidence. Not every evaluator needs every field.

### Mastery Evaluation

Mastery should be accumulated from weighted evidence across meaningful encounters. It must account for encounter difficulty, repeated farming, recovery, adaptation, and restraint.

Temporary stability must not be treated as mastery.

### Save State

Introduce an explicit, versioned data object. Do not serialize references to scene objects or ScriptableObject runtime state.

The saved state should include the Origin Sin, Current Dominant Sin, raw influence weights, per-sin pressure, overall pressure, regulation evidence, per-sin mastery, collapse history, Death corruption, companion choice, seed state, processed-event identities, and generation-rules version.

## Proposed Runtime Flow

```text
Gameplay action
    -> contextual action record
    -> relevant sin evaluators
    -> state command/delta
    -> SinManager validates and applies
    -> immutable/read-only snapshot published
    -> buffs, Reflection, hostility, encounters, and UI respond
    -> mastery evaluator records qualified evidence
```

All normalization and invariant enforcement should occur at the authoritative state boundary.

## Compatibility Risks

### Passive Recovery

The current system restores stability after a delay. The new design requires demonstrated control. Waiting may stop escalation, but should not provide meaningful mastery or full recovery by itself.

### Animation and Feedback Ownership

Sin feedback must not deepen the existing split between `CharacterAnimator` and `PlayerSpineAnimationController`. Animation ownership should be consolidated before adding sin-specific animation states.

### Initialization Order

Current managers discover one another through singleton state and scene searches. The redesigned event graph requires a predictable bootstrap order and explicit ownership.

### Event Duplication

Several systems may observe the same hit, kill, or attack. Every contextual action must have a stable identity or processing rule so it cannot affect sin state twice.

### Content Availability

Wrath can be tested with existing combat. Greed and the remaining sins require new resource, reward, encounter, NPC, quest, or exploration contexts. Architecture alone cannot guarantee meaningful mastery opportunities.

### Reflection Synchronization

Reflection visuals must react to snapshots from the authoritative model. They must not independently interpret raw gameplay events.

## Incremental Plan

### Phase 0: Stabilize Foundations

- Define bootstrap and manager lifetimes.
- Consolidate player animation ownership.
- Finish migration to one knockback system.
- Add tests around existing behavior tracking and sin transitions.

### Phase 1: Establish the General Model

- Define seven-sin identifiers and influence storage.
- Define normalization and invariant rules.
- Define intensity, instability, mastery, and state snapshots.
- Define contextual action and evaluation contracts.
- Define versioned save data.

Maintain current gameplay through a compatibility adapter while this model is introduced.

### Phase 2: Complete Wrath

- Map combat actions and context to Wrath influence and intensity.
- Distinguish necessary aggression from uncontrolled repetition.
- Implement Wrath benefits and risks through data.
- Add regulation, recovery, and mastery evidence.
- Connect hostility, counters, UI, and Reflection to the new snapshot.

### Phase 3: Validate with Greed

- Add a small reward and temptation loop.
- Implement accumulation, risk, investment, rejection, and anti-farming signals.
- Confirm that Greed can use the same core contracts without Wrath-specific assumptions.

### Phase 4: Expand Carefully

- Add the remaining sins one at a time.
- Require complete gameplay opportunities before presenting a sin as a finished Origin Sin choice.
- Revisit abstractions whenever a new sin requires exceptions in the authoritative model.

## Implementation Preconditions

The architectural decisions are recorded in `SIN_SYSTEM_DECISIONS.md`. Before implementation begins:

1. Convert the decisions into concrete data contracts without embedding balance numbers in runtime logic.
2. Define save-schema and configuration-version identifiers.
3. Define the contextual action record and unique event identity.
4. Define deterministic normalization, dominance hysteresis, and Collapse scheduling rules.
5. Define the first Wrath evaluator tests before replacing the existing scalar state.
6. Preserve current playable combat through a compatibility layer during migration.
