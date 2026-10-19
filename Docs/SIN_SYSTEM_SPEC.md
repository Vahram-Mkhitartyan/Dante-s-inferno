# Sin System Specification

Status: Proposed design direction

## Core Thesis

A sin is not a resource the player must eliminate. It is a powerful motivational force that must be brought under conscious control.

The system should distinguish three approaches:

- **Suppression:** Avoiding expression of the sin and therefore avoiding both its danger and its useful power.
- **Indulgence:** Repeatedly exploiting the sin's benefits until the player loses control.
- **Mastery:** Using the sin's advantages deliberately while adapting, exercising restraint, and accepting its risks.

The game should reward mastery rather than suppression or indulgence.

## Seven Simultaneous Sins

Every player possesses all seven sins:

- Pride
- Greed
- Lust
- Envy
- Gluttony
- Wrath
- Sloth

Each sin has a relative influence percentage. The seven percentages must always total 100 percent.

Percentages describe composition, not absolute intensity. Raising one sin's influence redistributes the relative composition, while a separate intensity model records how strongly the player is currently expressing each drive.

Internally, the implementation should store nonnegative influence weights and calculate percentages through normalization. This avoids negative values, invalid totals, and accumulated floating-point drift.

## Origin and Current Dominant Sin

The Origin Sin is selected through the opening confession with Death and remains fixed for the journey.

It defines:

- The player's primary mechanical identity
- Principal benefits and risks
- The central mastery challenge
- The Reflection's primary visual identity
- Environmental temptations and pressures

The Current Dominant Sin is derived from present influence and pressure. It may change during the journey without rewriting the player's Origin Sin.

The Origin Sin is the fixed narrative and build identity. The Current Dominant Sin describes the drive presently exerting the strongest control.

## Core Measurements

### Composition

The normalized relative influence of all seven sins.

### Intensity

How strongly a particular drive is currently being expressed. Intensity rises through relevant actions and can change independently from composition.

High intensity is not automatically failure, and low intensity is not automatically success.

### Control

How effectively the player regulates active impulses. Control must be demonstrated through meaningful behavior rather than awarded automatically for waiting.

Examples include:

- Interrupting an escalating pattern
- Changing strategy after repetition becomes unsafe
- Rejecting an exploitative reward
- Defending successfully under pressure
- Recovering from instability through deliberate play

### Mastery

Long-term evidence that the player can express a sin's useful qualities without becoming controlled by it.

Mastery is distinct from temporary stability. One successful action should not grant mastery, and one mistake should not invalidate the run.

## Gameplay Loop

1. The player performs an action.
2. The action and its gameplay context are recorded.
3. An evaluator determines which sins the action influences.
4. Composition, intensity, and control are updated.
5. The player receives the relevant mechanical benefit.
6. Repetition or inappropriate escalation creates instability.
7. The world responds with pressure, temptation, or changed behavior.
8. The player suppresses, indulges, or adapts.
9. The mastery system evaluates evidence of regulation.
10. Repeated successful regulation advances mastery of the relevant sin.

Actions must be evaluated contextually. Repeating an attack is not automatically Wrath if repetition is tactically necessary.

## Instability States

The provisional state sequence is:

`Controlled -> Rising -> Unstable -> Critical`

Recovery can move the player back toward Controlled.

These states represent current instability, not permanent mastery progress. Temporary instability, repeated uncontrolled behavior, and long-term mastery must remain separate concepts.

## Pure Forms

Each sin has a destructive expression and a mastered expression.

### Wrath

- Uncontrolled: unnecessary escalation, retaliation, and aggression without judgment
- Pure form: decisive and precise force exercised with restraint

### Greed

- Uncontrolled: accumulation, exploitation, and excessive risk without purpose
- Pure form: productive ambition and intelligent value creation without destructive overreach

### Pride

- Uncontrolled: overconfidence, superiority, and refusal to acknowledge mistakes
- Pure form: confidence grounded in accurate self-awareness

### Envy

- Uncontrolled: blind imitation, destructive comparison, and sabotage
- Pure form: learning from others while maintaining independent judgment

### Gluttony

- Uncontrolled: excessive consumption, waste, and depletion
- Pure form: meaningful enjoyment and sustainable resource use

### Sloth

- Uncontrolled: avoidance and refusal to act when action is necessary
- Pure form: efficient, purposeful action without wasted effort

### Lust

- Uncontrolled: fixation and impulsive pursuit that overrides judgment
- Pure form: passion and focused desire governed by conscious choice

## Buffs and Risks

Every sin must provide a meaningful advantage that encourages engagement and a meaningful risk that emerges from excessive or inappropriate expression.

The system must avoid two dominant strategies:

- Always maximizing sin
- Always minimizing sin

The desired strategy is controlled expression under pressure.

Buffs, risks, thresholds, and evaluation rules should be configurable data rather than hardcoded across unrelated gameplay scripts.

## Pure Form Requirements

Pure Form is the highest expression of mastery for an individual sin. Every sin may be mastered independently and unlock its own Pure Form. It must not be unlocked by reaching a single percentage or intensity threshold.

Potential evidence includes:

- Regulation during high-pressure encounters
- Recovery from unstable states
- Adaptation after a behavior becomes ineffective
- Appropriate restraint when offered a powerful temptation
- Consistency across multiple encounters

Exact thresholds and formulas remain design decisions and must be configurable.

## Reflection

The physical player character remains visually consistent. The Reflection represents the internal sin profile.

Its appearance responds to:

- Origin Sin and Current Dominant Sin
- Seven-sin composition
- Current intensity and instability
- Mastery progress

The Reflection must consume the same authoritative state used by gameplay. It must not maintain a separate visual-only sin counter.

## Environmental Adaptation

The world should respond to the Origin Sin, Current Dominant Sin, instability, behavioral patterns, and mastery without requiring a separate campaign for each sin.

Responses should test adaptation instead of merely increasing enemy statistics. Existing hostility, counterattack, encounter, and temptation systems should consume sin-state notifications without owning sin calculations.

## Seed Rules

The run seed controls reproducible initial conditions such as encounter selection, temptation distribution, and environmental variation.

The same seed, Origin Sin, generation version, and configuration should produce the same initial conditions. Player choices determine which conditional events activate, which sin becomes dominant, and whether mastery is achieved.

## Architectural Boundaries

- **SinManager:** authoritative state and state-change notifications
- **Sin profiles:** configurable identity, benefits, risks, and thresholds
- **Action evaluation:** converts contextual gameplay actions into sin influence
- **Mastery evaluation:** records evidence of regulation and progress
- **Reflection system:** converts authoritative state into visuals
- **World responders:** react to sin state without calculating it
- **Save system:** persists and restores a versioned sin-state snapshot

`SinManager` must not directly own combat, input, quests, visuals, and enemy AI.

## Required Edge Cases

- Composition always totals 100 percent.
- No influence value becomes negative.
- Origin Sin remains fixed even when another sin becomes currently dominant.
- Intensity remains independent from composition.
- Avoidance of meaningful encounters cannot produce mastery.
- Easy encounters cannot be farmed indefinitely for mastery.
- Alternating two actions cannot trivially bypass repetition detection.
- Legitimate repetition is not automatically classified as uncontrolled Wrath.
- Temporary instability does not erase long-term mastery.
- Every journey provides viable mastery opportunities for its Origin Sin and meaningful regulation opportunities for changing dominant sins.
- Gameplay events are processed once where required.
- Save/load cannot desynchronize gameplay and Reflection state.
- Behavior scoring is independent of frame rate.

## Implementation Principle

Design the general model for all seven sins, then implement only one complete vertical slice. Wrath should validate combat behavior. Greed should follow as a mechanically different resource and reward slice.

If Greed requires extensive changes to the core model, revise the abstraction before implementing the other five sins.

## Final Direction

The approved architecture uses:

- Fixed Origin Sin and dynamic Current Dominant Sin
- Per-sin local pressure with derived overall pressure
- Per-sin mastery with manually activated conditional Pure Forms
- Hidden regulation evidence interpreted through the world, Reflection, and Death
- Authored contextual tags with explicit narrative overrides
- Multi-sin action effects with primary and secondary influence
- Anti-farming through event identity, diminishing repetition, and meaningful-pressure requirements
- Checkpointed, versioned, deterministic journey saves

Numeric thresholds, dialogue content, and individual perk tuning remain prototype and balance work rather than unresolved architecture.
