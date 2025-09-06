# Game Design Document (GDD) – Interaction & Systems (I Am Hussain)

This GDD section captures the current player interaction model, core systems, and authoring rules after the NPC interaction refactor.

## Core Loop
- Explore the chawl during the day while time advances.
- Receive opportunities (phone/ambient), talk to NPCs, accept errands.
- Collect/gift items, complete errands, earn money/blessings/trust.
- End of day: journal reflection; next day begins.

## Interaction Model
- Movement: Input System via `SimplePlayerMovement` (WASD/Stick), Rigidbody2D locomotion.
- Conversation start:
  - NPCs: `ConversationStarter` on the NPC (trigger collider). Optional `ConversationZoneStarter` for automatic/cutscene entry.
  - Phone/Ambient: via `TriggerSystem` (see Triggers section).
- Special-item hints: `SpecialItemIndicator` on NPCs toggles a subtle cue when the currently equipped item is valid for this NPC (gifting or dialogue gate).

## Dialogue System (Conversa)
- Controller/UI: `MyConversaController` + `MyUIController` display messages and choices (2-button layout).
- Services: `GameServicesInstaller` assigns `ErrandSystem`, `InventorySystem`, `ContactSystem`, and optional `FlagService`, `StoryService` for node access.
- Custom Nodes (examples):
  - Inventory: `InventoryCheckNode`, `InventoryConsumeNode`, `GiveItemNode`.
  - Errands: `AcceptErrandNode`, `ErrandCheckNode`, `ErrandCompleteNode`.
  - Gates: `HasFlagNode`, `TrustAtLeastNode`, `ChapterAtLeastNode`, `SetFlagNode`, `AdvanceChapterNode`.
- Authoring Rules:
  - Gate optional branches with nodes rather than UI hints; rely on `SpecialItemIndicator` only as a soft cue.
  - Use `AcceptErrandNode` in conversations that propose errands; `GiveItemNode` for rewards; `InventoryConsumeNode` for turn-ins.

## Triggers
- Managed by `TriggerSystem` for Phone/Ambient only.
  - Phone: time-gated; if `TriggerSO.conversation` is set, starts Conversa immediately.
  - Ambient: time-gated; shows `OfferPrompt` (configurable text/buttons) to accept/decline.
- NPC triggers are deprecated. Use `ConversationStarter` or `ConversationZoneStarter` on the NPC GameObject.
- `TriggerSO` deprecations: `TriggerType.NPC`, `npcName`, `triggerRadius` are `[Obsolete]` and ignored by the runtime.

## Errands
- Types: Strict (same-day), MultiDay, Persistent, FollowUp, PlayerPlanned.
- Data: requirements (items/energy), time window, rewards (items/money/blessings), outcomes (on-time/late/fail), optional follow-up.
- Flow:
  1) Offered via Phone/Ambient (TriggerSystem) or via Conversa (nodes).
  2) Upon completion: consume requirements, set status to Completed or Late, grant rewards, emit events.
  3) Day end: Strict uncompleted errands fail; MultiDay persist.
- Time: Prefer comparing minutes (`GetTotalMinutes()`) against window endpoints for precision.

## Inventory & Items
- `ItemSO`: category, persistence, stackable, equippable, `equipTag` for special items.
- Equip: Inventory holds at most one “equipped special item”; day end clears non-persistent items and unequips when needed.
- Gifting: `GiftingSystem` applies outcome rules per item+recipient (trust, blessings, money, journal).

## Special Item Indicator (Per-NPC)
- Purpose: Nudge discovery of optional gifting/equip-gated branches without explicit prompts.
- Logic:
  - Visible only when: player in range (configurable), AND the equipped item is valid for this NPC.
  - Validity sources:
    - `DialogueGateSO`: `requiredEquipTag` must match; optional filters by `npcFilter` or `npcTags`.
    - `GiftingSystem`: `GetValidRecipients(itemId)` contains this NPC’s `contactId`.
  - Events: reacts to `onItemEquipped`, `onInventoryChanged`, hides on `onDayEnd`.
  - UX: keep subtle and non-blocking; treat as a hint, not instruction.

## Resources & Time
- Resources: money, energy, blessings; starting values/limits via `ResourceConfigSO`. Energy resets on day start.
- Time: `TimeSystem` simulates minutes; `onDayStart` and `onDayEnd` drive daily cycles.
- Game Phases: `GameStateManager` transitions Boot → InDay → Journal; Pause toggles `Time.timeScale`.

## UI
- OfferPrompt: used by Ambient triggers (and Phone if no conversation asset); pauses time while active.
- Inventory UI: lists, equips/unequips; Active Errands UI shows current tasks; Journal logs events.

## Authoring Guidelines
- NPC setup:
  - Add `ConversationStarter` (or `ConversationZoneStarter`). Assign Conversation and optional prompt.
  - Add `SpecialItemIndicator` if this NPC has equip- or gift-gated content. Assign `ContactSO`, indicator GameObject, optional `DialogueGateSO`, and hook inventory events.
- Phone content:
  - Create `TriggerSO` (Phone). Assign time window and `conversation` for Conversa-first flows.
- Ambient content:
  - Create `TriggerSO` (Ambient). Use `OfferPrompt` text/buttons; optionally chain to errands or items via acceptance logic.
- Conversa graphs:
  - Use node gates and outcomes to effect game state. Prefer nodes over hand-coded scene logic.

## Migration Notes (from pre-refactor NPC triggers)
- Remove `NPCInteraction` from NPCs.
- Convert any TriggerSO with `NPC` type to either:
  - Phone/Ambient, or
  - Move the interaction into the NPC’s `ConversationStarter` flow.
- Assign `SpecialItemIndicator` on NPCs where a special item can be gifted or unlocks dialogue.
- Ensure `GameServicesInstaller` is present in scenes.

## Testing Guidelines (high-level)
- TimeSystem: minute advance cadence; stop at day end.
- ResourceSystem: clamped add/spend; daily energy reset.
- ErrandSystem: requirements checks, late vs on-time, day end failures.
- Conversa Nodes: stub `GameServices` and validate side effects (inventory, errands, flags, chapter).

## Future Considerations
- Expand `GiftingSystem` condition checks (trust/chapter/equip) for more expressive outcomes.
- Add identifier-rich events (e.g., `TrustChanged(contactId, value)`) for UI targeting.
- Save/load services: flags, story chapter, inventory, errands, and resources.

