# Sin System Decisions

Status: Approved architectural direction; numeric tuning and content details remain subject to prototyping

This document records approved design decisions for the future sin-system architecture. Numeric tuning and authored content details remain prototype work and must not silently redefine these architectural rules.

## 1. Run Identity

### Opening Confession

The player's origin sin is established during an opening conversation with Death.

Death asks the player why they believe they are there. The available responses are indirect confessions rather than labels such as "Wrath" or "Pride."

Example structure:

- "I won every battle I fought, but lost to my faithfulness to my beloved." This response represents Lust.
- "I killed every enemy my country had, but eventually did it for personal pleasure." This response represents Pride or another configured interpretation depending on the final narrative taxonomy.

The player selects the statement that best describes their past. The selected confession determines the journey's origin sin.

The interface should not reveal the associated sin before the choice unless later playtesting shows that hidden interpretation feels unfair.

### Origin Sin and Current Dominant Sin

The design distinguishes two concepts that were previously both called the dominant sin.

#### Origin Sin

- The opening confession selects the player's origin sin.
- The origin sin explains why the player entered Hell and remains fixed for the journey.
- It receives a meaningful initial influence advantage.
- It anchors the opening narrative and the player's initial mastery work.

#### Current Dominant Sin

- The current dominant sin is the sin exerting the strongest present influence.
- It is derived from authoritative composition and pressure rather than selected directly.
- It can change during the journey.
- Reducing the origin sin does not guarantee mastery if another sin becomes dominant and uncontrolled.
- Hell can shift the player's struggle from the sin they understand to one they are unprepared to regulate.

The other six sins remain present at minimum starting influence.

### Initial Composition

Use the dominant-head-start model.

- The origin sin starts with a substantially higher influence weight.
- The remaining six sins begin at a shared minimum influence.
- Exact percentages remain configurable balance values.
- A provisional target may use approximately 34 percent dominant influence and 11 percent for each other sin.

## 2. Composition

### Storage and Normalization

- Store nonnegative raw influence weights.
- Derive displayed percentages by normalizing all seven weights.
- Displayed percentages must always total 100 percent after presentation-safe rounding.
- Increasing one sin redistributes relative composition proportionally across the others.

### Minimum Influence

- Every sin retains a small minimum raw influence.
- No sin is psychologically removed from the player.
- A displayed value may round to zero if presentation requires it, but its internal weight remains positive.

## 3. Intensity and Pressure

Use a hybrid model:

- Every sin tracks its own local intensity or pressure.
- Local pressure represents how actively that drive is currently influencing play and the world.
- Local pressures contribute to a derived overall pressure value.
- Overall pressure drives broad escalation and major world consequences.
- The most influential local pressure determines the dominant immediate visual language, even when it is not the journey's fixed origin sin.

## 4. Environmental Expression

Each sin has an assigned visual color and related environmental language.

When a sin becomes especially active:

- The world begins expressing that sin's color and atmosphere.
- The Reflection reveals the active influence through armor and appearance changes.
- Environmental changes communicate which drive is currently affecting the world.
- Secondary sins may contribute smaller visual details.
- Overall pressure determines the severity of the transformation.

The Reflection and environment must read from the same authoritative sin-state snapshot used by gameplay.

## 5. Inactivity

- Inactivity can stop additional escalation.
- Waiting alone does not restore meaningful control or grant mastery.
- Recovery requires deliberate gameplay behavior.
- Inactivity should count as Sloth only in contexts where the player has a clear reason and opportunity to act.

## 6. Pressure States

- Every sin has local pressure.
- Local pressures combine into overall pressure.
- Overall pressure controls broad states such as Controlled, Rising, Unstable, and Critical.
- Local pressure identifies which sin is primarily shaping the current test, visuals, and consequences.

## 7. Control and Interpretation

Use a hybrid control model:

- The game maintains internal per-sin regulation evidence and derived stability values.
- These values are not presented as visible control meters.
- The environment and Reflection communicate immediate pressure during gameplay.
- Death interprets the player's behavior between levels.
- Death gives a qualitative reality check rather than exposing formulas or numeric scores.
- Death's assessment should distinguish suppression, indulgence, unstable recovery, and genuine mastery.

Death is the primary narrative interpreter of the system, but Death does not own or calculate authoritative sin state. Death receives a read-only summary from the sin system and translates it into dialogue.

## 8. Repetition and Wrath

Repeated attacks are not currently treated as uncontrolled Wrath or directly punished.

- The system may record repetition for analytics and later evaluation.
- No fixed spam threshold should affect sin, control, or mastery until playtesting shows that real players exploit repetition enough to harm the intended experience.
- If a future anti-spam rule is introduced, it should use contextual warnings and opportunities to adapt rather than classifying repetition alone as immoral or uncontrolled.

## 9. Gameplay Warning

The player receives audiovisual warning before behavior reaches serious instability.

Possible channels include:

- Environmental color and lighting
- Reflection appearance
- Sound and music changes
- Enemy posture or behavior
- Environmental distortion

Warnings should communicate escalation without displaying formulas.

## 10. Combat Adaptation

The following behaviors can provide evidence of deliberate adaptation when appropriate to the encounter:

- Changing attack type
- Blocking or parrying
- Disengaging or repositioning
- Changing targets

These actions do not automatically grant control. Their meaning depends on the pressure, available alternatives, and outcome.

## 11. Multi-Sin Actions

- One action may influence multiple sins.
- An evaluated action has one primary sin influence and optional secondary influences.
- Influence strengths do not need to be equal.
- Evaluators should avoid assigning every action to many sins without meaningful contextual justification.

## 12. Mistakes and Mastery

- Mistakes can quickly damage current control and increase pressure.
- A single mistake does not erase accumulated mastery.
- Long-term mastery may decline slowly after repeated or substantial failures.
- Temporary instability and permanent mastery progress remain separate.

## 13. Anti-Farming

Mastery evidence uses all of the following safeguards:

- A specific encounter or decision awards the same evidence only once where appropriate.
- Repeatedly farming similar situations produces diminishing evidence.
- Evidence requires meaningful pressure, difficulty, temptation, or consequence.
- Encounter and event identity must survive save/load so rewards cannot be reset through reloading.

## 14. Player-Facing Information

Do not expose exact sin, intensity, control, or mastery numbers as the primary interface.

- The environment communicates which sin is active.
- The Reflection communicates composition and severity through appearance.
- Death gives a qualitative assessment between levels.
- Numeric values remain internal for deterministic calculation, balancing, reason records, and automated testing.
- Do not build an exact-value runtime debug panel. Diagnose the system through structured reason records, logs, tests, and the post-journey summary.

## 15. Death Between Levels

- The player meets Death between every major level.
- Death reviews the completed level and frames the next stage of the journey.
- The first meeting follows the player's opening confession.
- Death explicitly identifies the chosen origin sin during that first response, for example: "So, Lust will be your guide here."
- Later dialogue relies more on implication, metaphor, accusation, and reference to observed behavior.

## 16. Death's Reliability and Corruption

Death begins as a truthful interpreter of the player's condition.

As sin pressure changes the environment, Death also changes from a clean state into progressively corrupted states.

Corruption progresses gradually:

1. Death remains factually truthful but changes emphasis and wording.
2. Death begins framing accurate observations in manipulative ways.
3. Death starts omitting relevant truths and encouraging indulgence.
4. Death becomes meaningfully misleading and increasingly corrupted.
5. At a sufficiently advanced state, the player can challenge Death and initiate a fight.

Death's dialogue presentation may become unreliable, but Death must never mutate or replace the authoritative sin state. The environment, Reflection, consequences, reason records, and tests continue reading the actual state.

Death's corruption uses two layers:

- Current overall pressure affects immediate presentation and behavior.
- Accumulated indulgence and control failures create lasting corruption progression.

The precise weighting and stage thresholds remain configurable.

### Confronting Death

- The player recognizes Death's corruption and chooses whether to challenge him.
- Death's corruption does not substantially reverse before the confrontation.
- The longer the player takes to recognize or confront Death's corruption, the more corrupted and difficult the Death encounter becomes.
- Defeating Death causes him to confess that his desire to discover how far the player would go contributed to his corruption.
- After the confession, the player chooses whether to continue the journey with Death or without him.
- Continuing with Death grants a major one-time intervention during the Satan confrontation.
- Continuing without Death creates the harder, less-guided route and removes that intervention.

## 17. Critical Pressure and Collapse

- Entering Critical pressure begins a dangerous grace period.
- The player has an opportunity to regulate before Collapse occurs.
- Failure to regulate during that period causes Collapse and its consequences.
- Critical pressure must be strongly communicated through audiovisual and mechanical feedback.

### Combat Death

Combat death and sin Collapse are different systems.

- Combat death represents physical failure.
- Collapse represents loss of internal control.
- Combat death does not automatically count as Collapse.
- Collapse may influence combat or contribute to death, but the two states remain independently identifiable.

## 18. Pure Form

- Accumulated mastery permanently unlocks access to Pure Form for the current run.
- Pure Form can only activate while its corresponding sin is under meaningful pressure and the player remains in control.
- Reaching a threshold without demonstrated mastery cannot unlock Pure Form.

Pure Form currently provides:

- Protection from or meaningful resistance to the relevant sin's primary debuff.
- Different Death dialogue and access to ending conditions unavailable without mastery.

Pure Form is not currently defined as a new attack, general damage upgrade, or complete combat transformation.

## 19. Persistence

- Sin composition, local pressure, control state, and run mastery reset between runs.
- Narrative knowledge, discovered lore, and codex information may persist.
- Persistent knowledge must not automatically grant mechanical mastery in a later run.

## 20. Saving

Authoritative state is saved:

- At established checkpoints.
- After major irreversible decisions.
- During transitions into Death's between-level sequence.
- Before or after unique encounter resolution as required to prevent duplicated evidence.

The game does not need to perform a permanent save after every evaluated action.

## 21. Ending Structure

The current ending direction contains at least three outcomes.

### Conquered Ending

- The player reaches and defeats the final boss.
- The player has mastered the Origin Sin and has no other currently uncontrolled sin.
- The player is permitted to continue forward.

### Keeper After Victory

- The player reaches and defeats the final boss.
- The player has not mastered the Origin Sin or still has another uncontrolled sin.
- The door to Heaven closes or remains locked.
- The player becomes the new keeper.

### Reserved Throne

- The player reaches the ending with overwhelming sin pressure or corruption.
- No final fight occurs.
- A throne waits with a sign reading: "Reserved for [Player Name]."
- The player assumes the role of keeper without resistance.

### Final Opponent

- Satan is the existing keeper and principal final boss.
- Death watches the confrontation with Satan rather than serving as the standard final boss.
- Death may still be fought earlier through the corruption-confrontation route.

Exact numeric ending thresholds remain configurable and require playtesting.

## 22. World Presentation

Use layered visual identity:

- The fixed Origin Sin establishes the journey's underlying visual foundation.
- The most active current sin overlays that foundation through color, material, sound, and environmental effects.
- Secondary active sins may appear as smaller accents where readability permits.

Color must not be the only indicator. Sin pressure should also use:

- Reflection details
- Material and silhouette changes
- Symbols or environmental motifs
- Sound and music
- Animation and enemy behavior
- Death's language

This preserves readability for color-blind players and visually complex scenes.

## 23. Mechanical World Reactions

Environmental corruption is both visual and mechanical.

Initial approved examples include:

- Enemies trigger, become hostile, or react more frequently.
- Mirrors present a progressively damaged or obscured Reflection.
- Reflection details may include red eyes, a blurred face, corrupted armor, or related sin-specific changes.
- The severity of reactions follows overall pressure, while their form follows the most active local sin and the fixed Origin Sin identity.

Additional reactions should test adaptation rather than merely increasing enemy health or damage.

## 24. Level Structure

- Each level retains its own authored location, narrative purpose, and mechanical identity.
- The player's Origin Sin, Current Dominant Sin, active local pressures, mastery, and corruption modify encounters and presentation.
- The project does not require seven separate campaigns.
- Seeded variation may select temptations, enemy states, and conditional events within an authored level.

## 25. Sin Implementation Scope

- There is no deadline requiring a prematurely narrow public version.
- All seven sins should eventually receive meaningful mechanics.
- Development should be staged as an initial group of four sins followed by the remaining three.
- Wrath, Greed, Pride, and Sloth form the first beta-ready group.
- The underlying model must support all seven before sin-specific implementations are scaled.

## 26. Telemetry and Player Data

- Do not add gameplay analytics or player-behavior collection at this stage.
- Do not introduce a repetition penalty based on hypothetical player behavior.
- Evaluate repetition through internal playtesting and post-release player feedback.
- Any future telemetry proposal requires a separate privacy and consent decision.

Death asks for the player's name during the opening conversation. The name belongs to the save profile and may appear in dialogue and the Reserved Throne ending.

## 27. Campaign Model

- Each save file represents one authored journey or run.
- Levels are authored rather than procedurally generated wholesale.
- The run seed controls selected encounter states, temptations, event variants, and related conditional content.
- A new journey may produce different conditions without changing the narrative identity of each level.
- The origin sin is selected once at the beginning of a new journey and remains fixed for that save file.
- The current dominant sin may change throughout that journey.

## 28. Combat Death and Checkpoint Restoration

- Ordinary combat death restores the complete authoritative checkpoint snapshot.
- Position, health, encounters, sin state, pressure, control, mastery evidence, corruption, and processed-event identities must restore consistently.
- No subsystem may retain post-checkpoint changes unless it is explicitly designed as persistence outside the run snapshot.
- Death may comment on failure only if a separate design later approves remembering deaths outside checkpoint restoration.

## 29. Collapse Event Burst

Collapse immediately triggers a concentrated period in which the world turns against the player.

The intended experience is a burst of approximately 10 to 15 hostile events over a limited duration. The count and duration are configurable rather than fixed rules.

Potential events include:

- Screen blur, shake, distortion, or related sensory pressure
- Nearby neutral or inactive enemies becoming hostile
- Existing enemies attacking more aggressively
- Ground hazards activating beneath or near the player
- Lava or environmental splurges
- Traps, obstructions, deceptive cues, or other level-appropriate threats

Architectural requirements:

- Collapse events come from a configurable event pool.
- The run seed and authoritative state make selection reproducible.
- Levels declare which events are valid in their environment.
- Event scheduling is time-based and independent of frame rate.
- Events require fairness rules preventing impossible overlaps or unavoidable lethal combinations.
- Sensory pressure is part of Collapse and is not removed through a reduced-effects mode.
- Effects must still avoid unsafe flashing patterns and platform-prohibited presentation.

Surviving the burst lowers pressure enough to leave Critical but does not grant mastery by itself.

## 30. Difficulty and Judgment

- Combat difficulty does not change the moral interpretation of equivalent player behavior.
- Sin thresholds and mastery rules remain consistent across difficulty settings.
- Accessibility options may widen input or reaction windows where necessary without changing the meaning of decisions.

## 31. Save Profiles

- Support multiple independent save profiles or journey slots.
- Every profile stores its own Origin Sin, Current Dominant Sin, seed, authoritative state, choices, Death corruption, companion state, checkpoints, and ending eligibility.
- Narrative knowledge or codex persistence across profiles remains subject to the existing meta-progression decision.

## 32. Journey Without Death

- After defeating Death and hearing his confession, the player may continue without him.
- Continuing without Death creates a harder route with reduced guidance.
- The environment and Reflection remain available as truthful but indirect feedback.
- This route has unique narrative and ending consequences.
- A replacement guide should not provide equivalent clarity, or the choice loses its weight.

Forgiving Death provides his one-time intervention during the Satan confrontation and allows his redemption arc to continue.

## 33. Initial Sin Development Order

Implement the first four complete sin slices in this order unless prototyping reveals a dependency:

1. Wrath: combat intensity, escalation, restraint, and precision
2. Greed: rewards, resources, accumulation, investment, and exploitation
3. Pride: independence, assistance, confidence, failure, and adjustment
4. Sloth: avoidance, urgency, efficiency, and purposeful action

Implement Lust, Envy, and Gluttony after these four validate the shared architecture.

All seven remain represented in the underlying state model from the beginning.

## 34. Data-Driven Content

The following content must be configurable outside gameplay code:

- Sin identities, colors, symbols, and visual references
- Influence, pressure, control, and mastery tuning
- Buffs and debuffs
- Evaluation rules and evidence weights
- Collapse event definitions and level eligibility
- Death dialogue conditions and text
- Reflection stages
- Environmental reactions
- Ending thresholds and requirements

Runtime state must remain separate from these configuration assets. Dialogue and rules should support localization and versioning from the beginning.

## 35. Collapse Recovery

- Surviving a Collapse lowers pressure enough to leave the Critical state.
- Survival alone does not grant mastery.
- The player's decisions before, during, and after Collapse may independently provide qualified control or mastery evidence.
- Collapse triggers whenever authoritative pressure reaches the configured threshold.
- Skilled combat execution cannot allow the player to ignore the sin system indefinitely.
- Collapse frequency is therefore behavior-driven rather than limited to a target number per journey.
- Cooldowns may prevent immediate technical retrigger loops, but must not suppress a legitimately rebuilt Critical state.

## 36. Death's Final Assistance

- Forgiving Death and continuing with him provides assistance during the confrontation with Satan.
- Continuing without Death removes that assistance and remains the harder route.
- The form and strength of Death's assistance remain an encounter-design decision.
- Death's assistance must not determine victory automatically or invalidate the player's mastery outcome.

## 37. Mastery Durability

- Qualified mastery evidence persists during the journey.
- Repeated major failures can slowly reduce accumulated mastery.
- Ordinary passage of time does not erase mastery.
- A single mistake cannot remove a substantial body of demonstrated mastery.

## 38. Buff Ownership

- The origin sin provides the initial complete mechanical identity.
- The current dominant sin and strongly active secondary sins may provide smaller temporary effects.
- The system must avoid simultaneously applying seven full buff and debuff kits.
- The complete kit remains anchored to the Origin Sin when the Current Dominant Sin changes.

## 39. Pure Form Activation

- Pure Form is manually activated by the player.
- Activation is available only after the relevant mastery requirement has been unlocked.
- Activation also requires meaningful pressure and sufficient current control.
- Pure Form is an act of conscious direction, not an automatic threshold reward.

## 40. Contextual Evaluation

Use a hybrid authoring model:

- Explicit gameplay tags and evaluator rules are the normal source of contextual judgment.
- Encounters report facts such as urgency, surrender, temptation, available assistance, scarcity, danger, and outcome.
- Unique narrative decisions may provide explicitly authored sin meaning where general rules are insufficient.
- Raw input repetition alone does not determine moral meaning.

## 41. Reflection Truthfulness

- The Reflection is a presentation of authoritative state, not an independent character or judge.
- It never intentionally lies.
- It may become blurred, damaged, incomplete, frightening, or difficult to interpret when the true state calls for that appearance.
- Death's corruption cannot falsify the underlying Reflection state.

## 42. Release Staging

- Internal and beta testing begins after Wrath, Greed, Pride, and Sloth are complete enough to validate their full loops.
- Incomplete sins must not be presented as equivalent finished choices during beta.
- Lust, Envy, and Gluttony are completed after feedback from the first four.
- The intended full release contains meaningful implementations for all seven sins.

## 43. Independent Mastery Scope

Every sin has independent mastery progress and can eventually unlock its own Pure Form.

### Anti-Hopping Rules

- The Origin Sin remains the player's complete core mechanical kit.
- A newly dominant sin does not replace the complete build with a more advantageous kit.
- Current Dominant and strongly active secondary sins provide pressure, risks, and smaller temporary effects.
- Pure Form activation requires individual mastery, relevant current pressure, and current control.
- Players cannot freely equip a mastered sin solely to obtain its strongest perk.
- Mastery evidence remains subject to meaningful-pressure, encounter-identity, and anti-farming rules.

This preserves independent mastery without encouraging players to manipulate composition merely to chase the mathematically strongest kit.

## 44. Drift and the Journey Without Death

- Leaving Death makes changes in sin dominance less clearly explained.
- The player must interpret environmental and Reflection feedback without an explicit guide.
- A player approaching mastery in one sin becomes more susceptible to drifting into another sin as Hell creates mixed and confusing situations.
- A player without established mastery has greater difficulty understanding and regulating a newly dominant sin.
- The system should create ambiguous mixed-pressure situations without making the authoritative calculations random or incoherent.
- Mastering a newly dominant sin without Death remains possible, but significantly harder to understand and pursue.

Exact drift modifiers remain balance decisions requiring prototypes.

## 45. Pure Forms

- Every sin has its own Pure Form.
- Each Pure Form requires independent mastery of that sin.
- Pure Form is manually activated under relevant pressure and control conditions.
- Unlocking several Pure Forms does not permit unrestricted perk swapping outside those conditions.

## 46. Dominance Stabilization

- Current Dominant Sin does not change the instant another influence becomes marginally higher.
- A challenger sin must exceed the current dominant influence by a configurable margin.
- It must remain above that margin for a configurable duration or qualified event window.
- This hysteresis prevents visual, mechanical, and dialogue flicker.

## 47. Death and Changing Dominance

- When Death remains with the player, he initially hints that another sin is becoming dominant.
- If the player continues drifting, Death becomes explicit about the new dominant sin and its danger.
- Death's reliability still depends on his current corruption stage.

## 48. Conquered Ending Requirements

- The player must master the Origin Sin.
- No other sin may remain currently uncontrolled at the ending.
- The player does not need to master all seven sins in one journey.
- A newly dominant but uncontrolled sin can prevent the conquered outcome even when the Origin Sin is mastered.

## 49. Heaven Without Death

- Leaving Death does not permanently close Heaven.
- The player can still achieve the conquered ending without Death.
- This route is harder because the player loses direct interpretation and Satan-fight assistance.
- Success without Death may support distinct dialogue or a stronger self-mastery variation of the ending.

## 50. Save Compatibility

- Save data uses an explicit schema version.
- Game updates attempt to migrate older saves whenever practical.
- Migration is deterministic and must preserve authoritative run identity and processed-event history.
- If a save cannot be migrated safely, the game clearly explains the incompatibility rather than silently resetting or corrupting state.
- Before any destructive migration, preserve a recoverable backup of the original save.

## 51. Deterministic Save State

Every journey save stores enough information to reproduce authoritative outcomes after loading, including:

- Original run seed
- Current random-generator state or equivalent deterministic stream positions
- Consumed and resolved event identities
- Configuration and generation-rules versions
- Collapse event history and active scheduled events
- Checkpoint identity and authoritative sin snapshot

Reloading must not allow the player to reroll an already-determined temptation, Collapse burst, encounter assignment, or mastery outcome.

## 52. Rules Versions During Updates

- Existing journeys retain the balance and evaluation rules version with which they began whenever practical.
- Patches may migrate data structures without silently changing the moral meaning of actions already taken.
- New journeys use the latest approved rules and balance configuration.
- If an urgent defect requires changing an active journey's rules, the migration must be explicit, deterministic, documented, and tested.

## 53. Collapse Presentation

- Blur, shake, distortion, and related sensory effects remain part of the intended Collapse challenge.
- The normal design does not provide a reduced-presentation mode that preserves an otherwise identical Collapse.
- The implementation must nevertheless avoid unsafe flashing frequencies and respect mandatory platform safety requirements.
- Mechanical scheduling remains independent from presentation so visual effects cannot alter authoritative outcomes.

## 54. Diagnostics Without a Runtime Debug Panel

- Do not create a player-accessible or developer runtime panel exposing all exact sin values.
- Automated tests may inspect exact state directly.
- Structured local reason records explain important state changes.
- Development logs may expose exact values when explicitly enabled.
- Normal builds should avoid unnecessary verbose logging.

## 55. Evaluation Reason Records

Important state changes create compact structured records containing:

- Event identity
- Gameplay context and source
- Primary and secondary sin influences
- Pressure, control, or mastery effects
- Rule or authored decision that produced the result
- Level, encounter, and checkpoint identity
- Authoritative timestamp or sequence number

Example human-readable rendering:

`Wrath pressure increased: attacked a surrendering enemy.`

Reason records remain local. They support deterministic debugging, Death's summaries, save validation, and the post-journey ending sequence. They are not analytics.

## 56. Death Encounter Scaling

- Death does not fully recover from corruption before being confronted.
- Delay allows corruption to deepen.
- Greater corruption makes the fight mechanically harder and changes Death's appearance, dialogue, and behavior.
- Difficulty scaling must use explicit corruption stages rather than frame time or hidden arbitrary multipliers.
- Defeating Death resolves the confrontation and leads to his confession and the companion choice.

## 57. Death's Intervention Against Satan

- Redeemed and retained Death performs one major intervention during the Satan confrontation.
- The intervention should feel narratively significant and express Death's attempted redemption.
- It does not fight the entire battle for the player.
- It does not override mastery or ending requirements.
- The exact intervention is authored as part of the Satan encounter.

## 58. Player Identity

- Death asks the player's name during the opening conversation.
- The name is stored with the save profile.
- Dialogue and UI use a safely formatted version of the name.
- The Reserved Throne ending displays: `Reserved for [Player Name]`.
- A sensible fallback is used if the player leaves the name empty.

## 59. Post-Journey Judgment

- Finishing a journey unlocks an optional behavioral summary.
- The presentation should resemble a reflective ending poem rather than an analytics dashboard, inspired by the emotional role of Minecraft's ending poem without copying its text or structure.
- It may reveal meaningful statistics, patterns, decisions, collapses, recoveries, mastery evidence, Death's corruption, and changing dominance.
- The summary explains the journey without reducing it to a scorecard.
- Exact raw formulas and arbitrary internal coefficients do not need to be exposed.
- The ending sequence uses the locally stored evaluation reason records.

## Decision History

- October 18, 2026: Established opening confession, fixed Origin Sin identity, origin-head-start composition, proportional redistribution, minimum influence floors, local and overall pressure, environmental color feedback, and non-restorative inactivity.
- October 18, 2026: Established hidden hybrid control, Death's between-level qualitative judgment, non-punitive repetition tracking, audiovisual warnings, recognized combat adaptations, multi-sin actions, resilient mastery, anti-farming safeguards, and primarily diegetic feedback.
- October 18, 2026: Established mandatory between-level meetings with Death, progressive corruption of Death, a possible Death confrontation, Critical grace periods, separation of combat death and Collapse, conditional Pure Form, run-reset mastery, checkpoint saving, and three initial ending outcomes.
- October 18, 2026: Established pressure-plus-history Death corruption, player-initiated confrontation, Death's confession and companion choice, Satan as keeper and final boss, layered sin visuals, mechanically reactive environments, authored levels with sin-driven variation, staged seven-sin development, no current analytics, and multi-channel accessibility.
- October 18, 2026: Established authored seeded journeys, one Origin Sin per save, complete checkpoint restoration, Collapse event bursts, difficulty-independent judgment, multiple save profiles, a harder route without Death, the first four sin slices, and data-driven rules and dialogue.
- October 18, 2026: Split fixed origin sin from dynamic current dominant sin; established Collapse survival behavior, Death's Satan-fight assistance, durable but reducible mastery, limited secondary buffs, manual Pure Form, contextual tagged evaluation, truthful Reflection state, and four-sin beta staging.
- October 18, 2026: Established independent mastery and Pure Forms for all sins, anti-perk-hopping constraints, confusing drift without Death, stabilized dominance switching, Death's escalating warnings, conquered-ending requirements, and a viable Heaven route without Death.
- October 18, 2026: Established versioned save migration and complete deterministic event persistence beyond the original seed.
- October 18, 2026: Established rules-version retention, full Collapse presentation, diagnostics through reason records instead of a debug panel, corruption-scaled Death combat, a one-time Satan intervention, opening player naming, and a reflective post-journey statistical poem.
