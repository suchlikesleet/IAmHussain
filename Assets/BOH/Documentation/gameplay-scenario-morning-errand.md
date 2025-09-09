# Gameplay Scenario: Morning Errand — Deliver Medicine

## Overview
- Scene: `SCN_Vinodnagar`
- Primary systems: `GameStateManager`, `TimeSystem`, `ErrandSystem`, `InventorySystem`, `ResourceSystem`, `OfferPrompt`
- NPCs: Aunty Zainab (recipient), Corner Chemist (vendor)
- Goal: Buy medicine from the chemist and deliver it to Aunty before 09:00.

## Narrative Hook
Hussain receives a morning request from a neighbor to pick up her medicine. It is a simple, time-bound errand that introduces core loops: accept → gather → deliver → reward.

## Preconditions
- Day starts at 06:00 (per `TimeConfigSO`).
- Player has at least 2 Energy (`ResourceConfigSO.startEnergy >= 2`).
- Inventory empty or with enough space for 1 stackable item.

## Acceptance
1) Trigger zone near Aunty shows an `OfferPrompt`:
   - Source: Aunty Zainab
   - Offer: “Please pick up my medicine from the Corner Chemist.”
   - Accept: “I’ll get it.” Decline: “Not now.”
2) On Accept:
   - `ErrandSystem.AddErrand` with id `ERR_Meds_Aunty`.
   - Energy cost on completion: `2`.
   - Rewards on time: `+5 Money`, `+1 Blessing`, `+Trust` (optional if Contacts exist).
   - Late delivery (after 09:00): reduced rewards `+2 Money`, `0 Blessings`.

## Task Flow
1) Navigate to Chemist stall (map marker optional).
2) Interact to purchase “Aunty’s Medicine”:
   - Requires `Money >= 5`; if not, prompt shows “Need 5 Money”.
   - On success, `Inventory.AddItem(Medicine, 1)` and deduct `5 Money`.
3) Return to Aunty before 09:00.
4) Interact to deliver:
   - `ErrandSystem.TryCompleteErrand("ERR_Meds_Aunty")` checks:
     - Has `Medicine x1` → consumes it.
     - Has `Energy >= 2` → spends it.
     - Late check via `TimeSystem.GetTotalMinutes()` against 09:00.
   - Apply rewards based on punctuality and show a small toast.

## Failure/Edge Cases
- Decline at start: Errand not added; prompt can be re-triggered.
- Out of money at Chemist: guidance hint appears; player can do a micro-task (future) or come back later.
- Out of energy on delivery: delivery blocked with hint “You’re too tired.”
- After 09:00: completion marked Late; reduced rewards.
- End of day while active: `onDayEnd` clears late/failed errands as per `ErrandSystem` rules.

## Tuning Parameters
- Time window: 06:00–09:00 (end-minute inclusive).
- Costs/Rewards (default): Buy cost 5 Money; Energy cost 2; On-time rewards +5 Money, +1 Blessing; Late rewards +2 Money.

## Telemetry (optional)
- Track accept/decline, time to completion, on-time vs late rate, and player route heatmap between Aunty and Chemist.

## Test Notes
- EditMode: Validate `ErrandSystem` requirement check, item consume, reward application.
- PlayMode: Simulate time advancing; confirm punctual and late branches; validate `OfferPrompt` pause/unpause does not stall timers (uses unscaled time).

## Content Hooks
- Items: `Medicine` (stackable: true, persistent: true).
- Vendors: `Corner Chemist` interaction grants `Medicine` for 5 Money.
- UI: Map marker for vendor; toast on completion; small portrait for Aunty in prompt.

## Story & Dialogues

### Cast
- Aunty Zainab — warm but firm, cares for the lane’s kids.
- Corner Chemist — practical, slightly sardonic, knows everyone’s tab.

### Story Synopsis
- Hussain starts the day with a neighborly responsibility: fetch medicine for Aunty Zainab. The player chooses to help, learns to budget money and time, and earns trust and blessings for punctuality. Being late softens rewards and nudges the player about time management.

### Personality Profiles
- Aunty Zainab
  - Voice: Gentle, teasing, sprinkles advice with humor.
  - Tells: Calls people “beta”, keeps a small prayer bead pouch, hums old radio jingles.
  - Inner life: Once ran a small stitching business; misses the bustle. Keeps an eye on neighborhood kids so they “don’t skip steps” in life.
  - Edge: Dislikes false promises. Warmth turns clipped when people overextend and fail quietly.
- Corner Chemist
  - Voice: Dry wit, unflappable. Speaks in short, precise sentences; notes details.
  - Tells: Counts currency twice, taps pen three times before making a difficult decision.
  - Inner life: Trying to modernize the shop; hates keeping tabs but does when he trusts.
  - Edge: Resents suppliers who delay deliveries. Keeps a steel box ledger.

### Variables, Flags, IDs
- Errand Id: `ERR_Meds_Aunty`
- Item Id: `MEDS_AUNTY` (display: “Aunty’s Medicine”)
- Flags:
  - `FLAG_ERR_MEDS_ACCEPTED`
  - `FLAG_ERR_MEDS_BOUGHT` (set when purchase succeeds)
  - `FLAG_ERR_MEDS_DELIVERED_ON_TIME`
  - `FLAG_ERR_MEDS_DELIVERED_LATE`
- Suggested Trust Keys (if Contacts used): `trust_aunty`

---

### Dialogue: Aunty Zainab

Conversation: `Aunty_Zainab_Dialogue`

- Entry A — First Greeting (No active errand, before 09:00)
  - Conditions: NOT `HasActiveErrand(ERR_Meds_Aunty)`, Time < 09:00
  - Line: “Hussain beta, could you fetch my medicine from the corner chemist?”
  - Choices:
    - Accept → Effects: `SetFlag(FLAG_ERR_MEDS_ACCEPTED, true)`, Add Errand `ERR_Meds_Aunty`, Hint: “Chemist opens early. It costs 5 Money.”
    - Decline → Line: “It’s alright, dear. Maybe later.” End.

- Entry B — Reminder (Accepted, not bought yet)
  - Conditions: `HasActiveErrand(ERR_Meds_Aunty)`, NOT `FLAG_ERR_MEDS_BOUGHT`
  - Line: “The chemist should have it ready. Be safe crossing the road.”
  - Choices:
    - “On my way.” → End.
    - “I’m short on cash.” → Line: “Ask at home or try a small job; I can wait a bit.” End.

- Entry C — Delivery (Holding medicine; checks punctuality and energy via completion)
  - Conditions: `HasActiveErrand(ERR_Meds_Aunty)`, Player has item `MEDS_AUNTY`
  - Line: “You found it! May I?”
  - Choices:
    - “Here you go.” → Effects:
      - Attempt `ErrandSystem.TryCompleteErrand(ERR_Meds_Aunty)`
      - If success AND Time < 09:00:
        - Line: “Right on time! Allah bless you.”
        - Effects: `SetFlag(FLAG_ERR_MEDS_DELIVERED_ON_TIME, true)`, `TrustChange(Aunty, +2)`
      - If success AND Time ≥ 09:00:
        - Line: “Thank you, beta. Try to be earlier next time.”
        - Effects: `SetFlag(FLAG_ERR_MEDS_DELIVERED_LATE, true)`, `TrustChange(Aunty, +1)`
      - If failure (not enough Energy):
        - Line: “You look exhausted; rest a moment.” End.
    - “I’ll be back.” → End.

- Entry D — Post-Delivery (Same day, after completion)
  - Conditions: NOT `HasActiveErrand(ERR_Meds_Aunty)`, any time
  - Branch:
    - If `FLAG_ERR_MEDS_DELIVERED_ON_TIME`:
      - Line: “You were a lifesaver this morning. Come have tea later.” End.
    - Else if `FLAG_ERR_MEDS_DELIVERED_LATE`:
      - Line: “Thank you again. Next time, we’ll beat the sun together.” End.
    - Else (no flags):
      - Line: “Lovely weather, isn’t it?” End.

- Entry E — After Day + Follow-up (Next day idle)
  - Conditions: New day, neither flag set (or auto-cleared)
  - Line: “How are your studies? I’ll call if I need you again.” End.

Implementation Hooks
- Accept choice: Call `ErrandSystem.AddErrand(ErrandSO)` and/or set `GameServices.Flags`.
- Delivery path: Completion handled by `TryCompleteErrand`, which consumes item and energy and grants rewards.
- Trust changes can be applied with `TrustChangeNode` if Contacts exist, else omit.

#### Aunty Zainab — Enriched Conversation Variants
- First Greeting (warmer color)
  - “Hussain beta, your shadow is long and the kettle is louder than the sparrows today. Could you fetch my medicine from the corner chemist?”
  - On Accept (teasing): “And don’t let that chemist sell you cough drops as a bonus. He tried that with me once.”
- Reminder (history)
  - “When my stitching ran slow, your mother fronted me thread. Ask at home or try a small job; I can wait a bit, but don’t wait on yourself.”
- Delivery On-Time (memory)
  - “Before the kettle whistles twice! Your father used to race the rain like this.”
- Delivery Late (gentle nudge)
  - “The day is long, but the body keeps shorter hours. Next time, we’ll beat the sun together.”
- Idle (higher trust)
  - “Your shoulders have grown, but don’t let them carry silence.”
  - “I still have my old sewing frame. It creaks—but it remembers weddings.”

---

### Dialogue: Corner Chemist

Conversation: `Corner_Chemist_Dialogue`

- Entry A — Greeting (No acceptance yet)
  - Conditions: NOT `FLAG_ERR_MEDS_ACCEPTED`
  - Line: “Morning. Need anything?”
  - Choices:
    - “Just looking.” → End.
    - “About Aunty Zainab’s medicine…” → Branch to Entry B.

- Entry B — Aunty’s Medicine (Accepted, not purchased)
  - Conditions: `FLAG_ERR_MEDS_ACCEPTED`, NOT `FLAG_ERR_MEDS_BOUGHT`
  - Line: “Ah yes, ready at the counter. It’s 5 Money.”
  - Choices:
    - “Pay 5 Money.” → Conditional:
      - If `Resources.GetMoney() >= 5`:
        - Effects: Deduct 5 Money, `GiveItem(MEDS_AUNTY, x1)`, `SetFlag(FLAG_ERR_MEDS_BOUGHT, true)`
        - Line: “Here you go. Greet Aunty for me.” End.
      - Else:
        - Line: “Short on cash? I can’t do tabs anymore. Sorry.” End.
    - “Maybe later.” → End.

- Entry C — Already Holding Medicine
  - Conditions: Player has `MEDS_AUNTY`
  - Line: “You already have it. Don’t keep Aunty waiting.” End.

- Entry D — Late Window (Optional)
  - Conditions: Time ≥ 10:00
  - Line: “Stockroom’s busy; if you need anything else, come earlier.” End.

Implementation Hooks
- Payment can be handled by a custom Conversa action or a small mediator script that checks `ResourceSystem` and grants the item via `GiveItemNode` when successful.
- The `FLAG_ERR_MEDS_BOUGHT` avoids repurchasing and lets Aunty recognize progress.

#### Corner Chemist — Enriched Conversation Variants
- Greeting (neutral)
  - “Morning. Inventory’s thin on cough syrups. Need anything?”
- Aunty’s Medicine (precise)
  - “Yes. Ready at the counter. Five Money. Checked twice, seal intact.”
- On Payment (dry)
  - “Here. Tell Aunty the supplier finally discovered clocks.”
- Short on Cash (firm)
  - “No tabs today. Ledger’s already heavier than my counter.”
- Optional Probing
  - Why no tabs? → “Because my father kept them for twenty years, and the ledger outlived him.”
  - You seem tired? → “Counting coins is easy. Counting errands isn’t. Still—people first.”

---

### Branch Summary (Quick Reference)
- Aunty → Offer → Accept sets flag + adds errand | Decline ends.
- Chemist → Buy path checks money → gives `MEDS_AUNTY` and sets bought flag.
- Aunty → Delivery path checks item + energy → completes errand.
- Rewards differ: on-time vs late; flags note which outcome occurred.

### Future Seeds (Hints without certainty)
- Aunty Zainab
  - “The tailor down the lane is short on hands this week…” → Potential future errand: deliver stitched veils before rain.
  - “Clinic’s line is long—forms and stamps…” → Potential future: help queue management or form filling for elders.
  - “Captain, the one-eared stray…” → Potential future: find lost stray before evening azaan.
- Corner Chemist
  - “Suppliers and their clocks…” → Potential future: intercept a delayed delivery; choose between community fairness vs quick profit buyer.
  - “Ledger outlived him.” → Potential future: resolve an old family tab ethically; decide write-off vs collection.

### Subtext & Small Choices
- Respectful vs dismissive responses adjust subtle trust deltas (`±1`) and unlock extra ambient lines, not overtly rewarded.
- Asking about their day reveals personal history that slightly advances hidden familiarity flags (e.g., `FLAG_KNOWS_CHEMIST_FATHER`, `FLAG_KNOWS_AUNTY_STITCHING`).
- Bringing chai (if player has it) yields a one-time thank-you line and tiny blessing bonus, foreshadowing gifting mechanics.

## Conversa Node Outlines (Implementation Guide)

Below are practical node lists you can recreate in Conversa. Node type names align with your BOHConversa Runtime nodes where possible.

### Graph: Aunty_Zainab_Meds_Morning
- A1 AvatarMessage: “Hussain beta, your shadow is long…”
  - Gate upstream by: NOT HasActiveErrand(ERR_Meds_Aunty)
- A1C AvatarChoice: [Accept] / [Decline]
  - Accept → N2 AcceptErrandNode(errand=ERR_Meds_Aunty)
    - N2 → N3 AvatarMessage: “Don’t let that chemist sell you cough drops…” → End
  - Decline → A1D AvatarMessage: “Maybe later—don’t promise what your feet can’t carry.” → End

- R1 Reminder AvatarMessage: “Be safe near the milk cans—slippery as lies.”
  - Gate: HasActiveErrand(ERR_Meds_Aunty) AND NOT HasFlag(FLAG_ERR_MEDS_BOUGHT)
  - Choice optional: “I’m short on cash.” → Aunty gives supportive history line.

- D0 InventoryCheckNode(item=MEDS_AUNTY, count=1)
  - Gate: HasActiveErrand(ERR_Meds_Aunty)
  - If false → D0F AvatarMessage: “Find it at the chemist, beta.” → End
  - If true → D1 AvatarMessage: “You found it! Let me see…”
    - Optional: D2 Energy check is handled by ErrandSystem on completion.
    - TCHK Time check (pseudo): Time < 09:00?
      - If true → D3 ErrandCompleteNode(errand=ERR_Meds_Aunty)
        - Then S1 SetFlagNode(flag=FLAG_ERR_MEDS_DELIVERED_ON_TIME)
        - → D3A AvatarMessage: On-time delighted line → End
      - If false → D4 ErrandCompleteNode(errand=ERR_Meds_Aunty)
        - Then S2 SetFlagNode(flag=FLAG_ERR_MEDS_DELIVERED_LATE)
        - → D4A AvatarMessage: Late but grateful line → End

- PD Post-Delivery small talk
  - Gate: NOT HasActiveErrand(ERR_Meds_Aunty)
  - If HasFlag(FLAG_ERR_MEDS_DELIVERED_ON_TIME) → PD1 AvatarMessage: Tea + monsoon story + tailor hint
  - Else if HasFlag(FLAG_ERR_MEDS_DELIVERED_LATE) → PD2 AvatarMessage: Encouragement + clinic hint
  - Else → PD3 AvatarMessage: Weather idle

Note: Use `TimeBeforeNode(hour=9, minute=0, inclusive=false)` to branch on punctual vs late.

### Graph: Corner_Chemist_Meds_Morning
- C1 AvatarMessage: “Morning. Inventory’s thin on cough syrups. Need anything?”
  - Branch to C2 only if HasFlag(FLAG_ERR_MEDS_ACCEPTED)

- C2 AvatarMessage: “Yes. Ready at the counter. Five Money. Checked twice, seal intact.”
  - C2C AvatarChoice: [Pay 5 Money] / [Maybe later]
    - Pay → C3 PurchaseItemNode(item=MEDS_AUNTY, count=1, price=5, flagToSet=FLAG_ERR_MEDS_BOUGHT)
      - If false → C3F AvatarMessage: “No tabs today. Ledger’s heavier than my counter.” → End
      - If true  → C3S AvatarMessage: “Tell Aunty the supplier discovered clocks.” → End
    - Later → End

- C4 Already holding check
  - Gate: InventoryCheckNode(item=MEDS_AUNTY, count=1)
  - True → C4A AvatarMessage: “You already have it. Don’t keep Aunty waiting.” → End

Optional Probing from any chemist entry (small choices)
- “Why no tabs?” → C5 AvatarMessage: “Because my father kept them twenty years; the ledger outlived him.”
- “You seem tired.” → C6 AvatarMessage: “Counting coins is easy. Counting errands isn’t. People first.”

Hookup Tips
- Use HasActiveErrandNode and InventoryCheckNode to route between offer/reminder/delivery.
- TrustChangeNode can follow Aunty’s on-time/late outcome if you’re gating content by trust.

---

### Here’s a clear, end-to-end checklist to set up the Morning Errand scenario in your project.

Scene & Packages

- Open or create SCN_Vinodnagar.
- Ensure packages present: TextMeshPro, Unity UI, Conversa (already in Assets), Obvious.Soap (already in Assets).

Core Systems (in scene)

- Create an empty Systems object and add:
  - GameStateManager
  - TimeSystem
  - ResourceSystem
  - InventorySystem
  - ErrandSystem
- Optional (if you use them): JournalSystem, GiftingSystem, ContactSystem.

Configs & Events

- Create ScriptableObjects in Assets/BOH/Scriptables/Configs/:
  - TimeConfigSO (e.g., secondsPerMinute=0.5, startHour=6, startMinute=0, endHour=9, endMinute=0).
  - ResourceConfigSO (e.g., startMoney=10, startEnergy=5, maxMoney=999, maxEnergy=10).
- Assign to components:
  - Drag TimeConfigSO onto TimeSystem.config.
  - Drag ResourceConfigSO onto ResourceSystem.config.
- Create ScriptableEvent assets in Assets/BOH/Scriptables/Events/ (Obvious.Soap):
  - onDayStart, onDayEnd, onPauseToggle (ScriptableEventNoParam).
  - Assign them to GameStateManager and to any listeners (e.g., TimeSystem.onDayStart).

Generate Scenario Assets

- Menu: BOH → Scenarios → Create Morning Errand Placeholders
  - Creates Assets/BOH/Scriptables/Items/MEDS_AUNTY.asset
  - Creates Assets/BOH/Scriptables/ERR_Meds_Aunty.asset (requires MEDS_AUNTY x1, energy cost 2, rewards money/
    blessing).

Game Services Wiring

- Menu: BOH → Create GameServices Installer (Scene)
  - Verifies/auto-assigns found systems to GameServices.
  - Confirm in Inspector that Installer fields point to the systems in your scene.

Dialogue UI (Conversa)

- Create a Canvas (Screen Space - Overlay), add a panel with:
  - Message window: Image/Panel + TextMeshProUGUI for actorName and message, and a Button for next.
  - Choice window: a container with two Buttons for choices.
- Add MyUIController to the Canvas and assign:
  - messageWindow, choiceWindow, avatarImage, actorNameText, messageText, nextMessageButton, choiceButtonA,
    choiceButtonB.
- Add MyConversaController (anywhere, e.g., on Canvas). Assign uiController to the MyUIController on the Canvas.

Conversa Graphs

- Create two Conversation assets (e.g., in Assets/BOH/Scriptables/Conversations/):
  - Aunty_Zainab_Meds_Morning
  - Corner_Chemist_Meds_Morning
- Build graphs per the outlines in:
  - Assets/BOH/Documentation/gameplay-scenario-morning-errand.md (Conversa Node Outlines section)
  - Use these nodes (provided in code):
  - `AcceptErrandNode` (assign `ERR_Meds_Aunty`)
  - `HasActiveErrandNode` / `InventoryCheckNode`
  - `ErrandCompleteNode` (assign `ERR_Meds_Aunty`)
  - `TimeBeforeNode` (set `hour=9`, `minute=0`, `inclusive=false`) for on-time/late branch
  - `PurchaseItemNode` (set `item=MEDS_AUNTY`, `count=1`, `price=5`, `flagToSet=FLAG_ERR_MEDS_BOUGHT`)
  - `SetFlagNode` for accept and outcome flags
- Gate reminder/delivery by HasActiveErrandNode, and chemist purchase by FLAG_ERR_MEDS_ACCEPTED.

NPCs & Triggers

- Create NPC_AuntyZainab:
  - Add ConversationStarter or ConversationZoneStarter, assign Aunty_Zainab_Meds_Morning.
  - Set up a trigger collider or an interact key to call StartConversation().
- Create NPC_CornerChemist:
  - Add ConversationStarter/ZoneStarter, assign Corner_Chemist_Meds_Morning.
- Ensure something calls MyConversaController.StartConversation(conversation) from the starter/interaction script (use
  your existing ConversationStarter/ConversationZoneStarter components).

OfferPrompt (optional)

- If you want a phone-like prompt elsewhere, create a separate UI Canvas:
  - Add OfferPrompt and assign its serialized fields (promptPanel, sourceText, offerText, acceptButton,
    acceptButtonText, declineButton, declineButtonText).
  - Not required for this scenario since Conversa handles acceptance.

Flags & IDs

- Use these consistently in graphs:
  - `ERR_Meds_Aunty`, `MEDS_AUNTY`
  - Flags: `FLAG_ERR_MEDS_ACCEPTED`, `FLAG_ERR_MEDS_BOUGHT`, `FLAG_ERR_MEDS_DELIVERED_ON_TIME`,
    `FLAG_ERR_MEDS_DELIVERED_LATE`

Playtest Path

- Press Play at 06:00. Talk to Aunty → Accept.
- Go to Chemist → “Pay 5 Money” (uses PurchaseItemNode) → gain MEDS_AUNTY.
- Return to Aunty before 09:00 → on-time branch; after 09:00 → late branch.
- Verify energy cost (2) deducted and rewards applied.

Tuning & Polish

- Adjust TimeConfigSO.secondsPerMinute for comfortable testing speed.
- Adjust ResourceConfigSO.startMoney if you want a “short on cash” branch early.
- Add portraits to MyUIController.avatarImage and set actor names in message nodes.
- Optional: add trust changes using TrustChangeNode (if Contacts/Trust used).
- Optional: add map markers and SFX for acceptance/completion.