# Gameplay Scenarios

Expanded scenarios for design, authoring, and QA. Each scenario lists prerequisites, step‑by‑step flow, Conversa graph hints, involved systems, and success/failure conditions.

## 1) Morning Medicine Run (Phone → Delivery)
- Summary: Nurse calls at 08:00; accept, receive Medicine, deliver before 10:00.
- Prerequisites: `TriggerSO` (type Phone) with `conversation` set; `Medicine` `ItemSO`; delivery zone has Conversation.
- Steps:
  1) At 08:00, phone trigger fires; Conversa starts with offer.
  2) Player accepts → `AcceptErrandNode` adds errand; `GiveItemNode(Medicine,1)` grants item.
  3) Player reaches delivery zone; zone conversation checks/consumes item; completes errand.
  4) Journal shows result at day end.
- Conversa Graph: Offer → [Accept] → AcceptErrand + GiveItem → Hint; Zone: InventoryCheck/Consume → ErrandComplete → TrustChange.
- Systems: TriggerSystem (Phone), ErrandSystem, InventorySystem, TimeSystem, MyConversaController, Journal.
- Success: Errand marked Completed before 10:00; item consumed; trust updated.
- Failure: Arrive after 10:00 (strict) → Late/Failed; no item → branch explains.
- QA: Toggle time to 07:59/08:00; verify accept adds errand, inventory shows Medicine, completion consumes it.

## 2) Parcel Delivery Sprint (Strict Window)
- Summary: Deliver a Parcel by 12:00; late marks Late/Failed.
- Prerequisites: `Parcel` `ItemSO`; strict `ErrandSO` with `endHour=12`.
- Steps: Accept via phone/NPC → receive Parcel → navigate → interact at recipient → complete.
- Conversa: AcceptErrand + GiveItem(Parcel); Delivery: InventoryCheck/Consume → ErrandComplete; late branch shows apology.
- Systems: ErrandSystem(strict), TimeSystem, InventorySystem, HUD.
- Success: Completed before noon; Journal logs on-time.
- Failure: After noon → Late/Failed; ensure status change and no double completion.
- QA: Simulate 11:59 vs 12:01; verify state transitions and Journal text.

## 3) Lost Necklace Recovery (Ambient Find + Gift)
- Summary: Ambient trigger grants Necklace; gifting to correct NPC yields trust.
- Prerequisites: Ambient `TriggerSO` near alley; `Necklace` `ItemSO`; target NPC Conversation supports gift.
- Steps: Enter alley → trigger → item added; talk to NPC → gift option appears → consume item → trust++.
- Conversa: Ambient: short message; NPC: InventoryCheck(Necklace) → InventoryConsume → TrustChange(+X) → Thank‑you.
- Systems: Triggers(Ambient), Inventory, Dialogue, ContactSystem.
- Success: Necklace removed; trust increased.
- Failure: No item → branch without gift.
- QA: Enter/exit area repeatedly (non-repeatable); verify indicator shows when in range with item equipped if applicable.

## 4) Grocer Barter Deal (NPC Proximity)
- Summary: Trade Notebook for money or keep it.
- Prerequisites: ConversationStarter on Grocer; `Notebook` `ItemSO`.
- Steps: Approach Grocer → options: [Sell], [Keep]; choose Sell → consume Notebook → add money.
- Conversa: Choice → [Sell] path: InventoryCheck/Consume(Notebook) → SetFlag("sold_notebook") → UserEvent/Money+.
- Systems: Dialogue, Inventory, ResourceSystem.
- Success: Money increases; item removed; flag set.
- Failure: No Notebook → disable Sell option via InventoryCheck.
- QA: Repeat without item; ensure option hidden/disabled and no negative money.

## 5) Night Charity (Ambient → Gifting)
- Summary: After 20:00, chance to give Food; blessings rewarded.
- Prerequisites: Time gate in trigger/conversation; `Food` item available.
- Steps: At night, ambient trigger invites; accept → consume Food → add Blessings.
- Conversa: Conditional(Time>=20) → InventoryConsume(Food) → UserEvent/Blessings+ → Message.
- Systems: TimeSystem, Triggers, Inventory, ResourceSystem.
- Success: Blessings increased; item consumed.
- Failure: Before 20:00 → no trigger; without Food → branch explains.
- QA: Verify no daytime activation; log correctness.

## 6) Nurse Follow‑Up (Phone → Chapter)
- Summary: Post‑completion call that advances chapter and unlocks new errands.
- Prerequisites: Flag set on previous completion; phone `TriggerSO` filtered by flag.
- Steps: Phone rings → conversation acknowledges progress → `AdvanceChapter(+1)` → `SetFlag("chapter2_unlocked")`.
- Conversa: HasFlag(prev_complete) → AdvanceChapter → SetFlag → optional AcceptErrand.
- Systems: FlagService, StoryService, TriggerSystem.
- Success: Chapter increments; new content available.
- Failure: Missing prerequisite flag → trigger not eligible.
- QA: Ensure repeated calls are prevented (non‑repeatable or flag guard).

## 7) Multi‑Step Errand Chain
- Summary: A → B sequence triggered on A completion.
- Prerequisites: Errand A with `followUpErrand=B` in `ErrandSO`.
- Steps: Accept/complete A → system auto‑adds B → B appears in Active list.
- Conversa: Completion node path sets flags/messages; no manual accept needed for B.
- Systems: ErrandSystem (follow‑up), Journal.
- Success: B added on A completion; no duplication.
- Failure: B already active/completed → guard against re‑adding.
- QA: Complete A twice (cheat) → confirm only one B active.

## 8) Shop Sell‑Off (Inventory Management)
- Summary: Player sells surplus items for money.
- Prerequisites: SimpleShop UI; price data on items.
- Steps: Open shop → select item → confirm sell multiple → inventory decreases; money increases.
- Conversa/UI: Not required; direct UI event or a simple Conversation with InventoryConsume + UserEvent(Money+).
- Systems: InventorySystem, ResourceSystem, UI.
- Success: Accurate counts/money; events fire (onInventoryChanged).
- Failure: Selling equipped/non‑sellable items blocked; clamp negative.
- QA: Sell stack edges (0, 1, max); verify persistence across scenes.

## 9) Time Pressure + Pause
- Summary: Two overlapping errands; pause to plan route.
- Prerequisites: Two strict errands with close deadlines.
- Steps: Accept both → pause (GameStateManager.TogglePause) → plan → resume → complete in order.
- Conversa: None required; errands and HUD communicate status.
- Systems: GameStateManager, TimeSystem, ErrandSystem, HUD.
- Success: TimeScale toggles; timers unaffected during pause; errands complete correctly.
- Failure: Pause in Journal phase blocked; ensure UI reflects.
- QA: Verify OnPauseToggle events and HUD updates freeze/resume correctly.

## 10) Zone Intro Cutscene (Auto Conversation)
- Summary: Enter area → short scene plays → sets flag to unlock NPC dialogue.
- Prerequisites: ConversationZoneStarter on trigger collider; gate NPC dialogue by flag.
- Steps: Enter zone → Conversa scene (Message/Actor nodes) → SetFlag("gate_opened"); NPC conversations now branch open.
- Conversa: Intro sequence → SetFlag → EndEvent.
- Systems: Dialogue, FlagService.
- Success: Flag present; NPC new branch visible.
- Failure: onlyOnce=true prevents repeat; ensure it’s respected.
- QA: Enter twice; second time should not replay if onlyOnce.
