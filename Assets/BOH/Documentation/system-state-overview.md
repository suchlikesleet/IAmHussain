# Current System State – Overview & Handoff

This document summarizes the current architecture, what’s working, what changed during refactor, known gaps, and clear next steps to resume work.

## Project Layout (Post‑Refactor)
- Core: `BOH/Core` — FlagService, StoryService, ServiceAbstractions (IFlagService/IStoryService/ITrustService)
- Data: `BOH/Data` — ScriptableObjects (`ItemSO`, `ErrandSO`, `TriggerSO`, `ContactSO`, configs)
- Features:
  - Inventory: `BOH/Features/Inventory` (+ `UI/InventoryUIExtended.cs`)
  - Errands: `BOH/Features/Errands` (ErrandSystem, `StoryContext`)
  - Triggers: `BOH/Features/Triggers` (TriggerSystem – Phone & Ambient only, `UI/OfferPrompt.cs`)
  - Dialogue: `BOH/Features/Dialogue` (Conversa controllers + `BOHConversa/Runtime|Editor` nodes)
  - NPC: `BOH/Features/NPC` (ConversationStarter/ConversationZoneStarter, SpecialItemIndicator)
  - Journal: `BOH/Features/Journal` (JournalSystem)
  - Gifting: `BOH/Features/Gifting` (GiftingSystem)
  - Shop: `BOH/Features/Shop` (SimpleShop)
  - Player: `BOH/Features/Player` (InputHandler, SimplePlayerMovement)
- UI Shared: `BOH/UI` — HUDController, JournalPanel, MainMenu, PauseMenu, TransitionFader, AppFlowManager
- Documentation: `BOH/documentation` — gameplay scenarios, this overview
- Legacy: Removed (`DialogueSystem` eliminated from scenes)

Note: Custom BOH asmdefs were removed to stabilize compile (single Assembly‑CSharp build). Third‑party asmdefs remain (Conversa, Soap, TMPro, InputSystem).

## Systems – Status Snapshot
- Game Flow
  - AppFlowManager: scene switching with TransitionFader; now in `BOH/UI`.
  - GameStateManager: phases (Boot, InDay, Journal, Paused); pause toggles `Time.timeScale`.
  - TimeSystem: minute ticks, day start/end events; UI listens via Soap events.
- World Data & Events
  - ResourceSystem: money/energy/blessings with ScriptableEventInt broadcasting.
  - FlagService / StoryService: lightweight services; interfaces in Core (`ServiceAbstractions`).
  - ContactSystem: trust and contacts registry (used by Conversa gates).
- Inventory & Errands
  - InventorySystem: add/consume/check, equip special items; events for UI.
  - ErrandSystem: active/completed lists, strict windows, follow‑up errands.
  - StoryContext: moved to Errands; helper for flag/errand/story checks.
- Triggers & Dialogue
  - TriggerSystem: Ambient & Phone scanning; for Phone triggers, starts Conversa if `TriggerSO.conversation` is assigned. NPC triggers are deprecated and ignored.
  - NPC Conversations: use `ConversationStarter` and/or `ConversationZoneStarter`; `ConversationSelector` supports gating/priority. Use `SpecialItemIndicator` for equip/gift hints.
  - Conversa: controllers + custom nodes (AcceptErrand, ErrandCheck/Complete, InventoryCheck/Consume, GiveItem, Trust/Flag, Chapter).
- UI
  - HUDController: time/resources; JournalPanel: opens on day end (stub text); OfferPrompt: legacy path (non‑Conversa).

## Conversa Integration – Current
- Phone triggers: use Conversa when `TriggerSO.conversation` is set; otherwise fall back to OfferPrompt.
- NPC: `ConversationStarter`/`ConversationZoneStarter` drive graphs by proximity or input; `ConversationSelector` supports gating/priority; `SpecialItemIndicator` provides contextual cues.
- Custom Nodes added: `GiveItemNode` (+ Editor view) under “Errands & Inventory”.
- GameServices (for nodes): in `BOH/Features/Dialogue/BOHConversa/Runtime/`; use the included `GameServicesInstaller` in scene to assign Errands, Inventory, Contacts, Flags, Story.

## Scenes – Current
- `Scenes/SCN_Chawl.unity`: Removed legacy `DialogueSystem`; keep `MyConversaController` present. `Systems_Root` hosts core systems (Time/Resource/Errand/Inventory/Journal/Contacts, etc.).

## Known Issues / Cleanups
- HUD currency format bug: `HUDController.UpdateMoney` shows a garbled format; replace with standard currency/text.
- Journal content is a placeholder; integrate real summaries (completed/late errands, rewards, trust changes).
- Ensure `GameServicesInstaller` exists in each gameplay scene (assigns services accessible by Conversa nodes).
- Trigger time parsing: TriggerSystem uses hour extracted from `TimeSystem.GetTimeString()`; consider exposing numeric time API on `TimeSystem` to avoid string parsing fragility.
 - NPC triggers: `TriggerSO.TriggerType.NPC`, `npcName`, and `triggerRadius` are marked Obsolete; migrate NPC interactions to ConversationStarter/Zone.
- Namespace alignment: classes still under `BOH`; optional: align to folders (`BOH.Features.*`).
- Tests: no `Assets/Tests`; consider adding EditMode tests for ErrandSystem & InventorySystem.
- CI/build: no project build scripts included; manual build via Unity Editor or CLI.

## Content & Authoring – Ready Paths
- Phone Conversations: author graphs with AcceptErrand + GiveItem; link to `TriggerSO.conversation` (type Phone, time windows).
- Delivery/Zone Conversations: place `ConversationZoneStarter` at destinations; use InventoryCheck→Consume→ErrandComplete.
- NPC Conversations: use `ConversationStarter` with `ConversationSelector` to branch by Flags/Chapter/Trust.
- Gifting: Inventory checks in NPC graphs; consume on success; update trust with `TrustChangeNode`.

## Next Steps (Recommended)
1) Place `GameServicesInstaller` in `SCN_Chawl` and wire references (or allow auto‑find) to Errands/Inventory/Contacts/Flags/Story.
2) Fix HUD money formatting; replace with `$"₹{value:N0}"` or similar per locale.
3) Flesh out JournalPanel content using data from ErrandSystem (active/completed today, late, rewards).
4) Author and link the 10 gameplay scenarios (see `gameplay-scenarios.md`).
5) Re‑introduce asmdefs incrementally once stable (Core → Data → one Feature at a time), verifying compile after each.
6) Add lightweight bootstrapping for scene service setup and a minimal test plan (EditMode for Errand/Inventory).
7) Content pass: verify TriggerSO (Phone/Ambient) windows and conversations; ensure no `TriggerSO` of type NPC remain. NPCs should use ConversationStarter/Zone and SpecialItemIndicator as needed; ensure `MyConversaController` exists in scenes.

## Quick Verification Checklist
- Scene has: MyConversaController, GameServicesInstaller, Systems_Root (Time/Resource/Errand/Inventory/Journal/Contact).
- Phone trigger with conversation starts graph; Accept adds errand; GiveItem grants item.
- Delivery zone consumes item and completes errand; Journal opens at day end.
- NPC interactions route through Conversa; selector gating works (Flags/Chapter/Trust).
- HUD updates on events; pause toggles time.

This captures the current state for immediate continuation in your next session.
