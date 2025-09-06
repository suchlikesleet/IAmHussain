Overview

  - Purpose: 2D URP project with errand/gifting gameplay, time-of-day, event-driven UI, and Conversa-powered dialogues.
  - Organization: Feature-based under Assets/BOH with Core systems, Scriptables, and UI separated. Scenes and URP
  Settings present and consistent.

  Structure

  - Code: BOH/Scripts by feature:
      - Core: GameStateManager, TimeSystem, ResourceSystem, ContactSystem, StoryService, FlagService.
      - Data: ItemSO, ErrandSO, TriggerSO, ContactSO, ResourceConfigSO, TimeConfigSO.
      - Features: Inventory, Errands, Triggers, Dialogue (Conversa), Gifting, Journal, NPC, Player, Shop.
      - UI: AppFlowManager, TransitionFader, MainMenu, HUDController, PauseMenu, JournalPanel.
  - Data: BOH/Scriptables with Configs (Time/Resource/Input), Events (Soap-based), Items, Conversations.
  - Settings: Assets/Settings (URP pipeline, 2D renderer, scene template).
  - Scenes: Assets/Scenes/SCN_Menu.unity, SCN_Chawl.unity (matches SCN_* convention).

  Core Systems

  - Game flow: GameStateManager drives phases (Boot/InDay/Journal/Paused) and pause via Time.timeScale. Emits
  onDayStart, onDayEnd, onPauseToggle.
  - Time: TimeSystem simulates minutes from TimeConfigSO, raises onMinuteTick, stops at end window and raises day end.
  - Resources: ResourceSystem manages money/energy/blessings; resets daily on onDayStart.
  - Inventory: Stackable items, equip one special item; daily clear for non-persistent items; events for UI.
  - Errands: Tracks active/completed, checks/consumes requirements, determines Late/Completed, supports follow-ups.
  - Contacts/Story/Flags: Lightweight services for trust, chapter gates, and world flags (used by dialogue gating).

  Dialogue & Triggers

  - Conversa integration: MyConversaController + MyUIController and custom nodes (AcceptErrand, GiveItem,
  InventoryCheck/Consume, Trust/Flag, Chapter).
  - ConversationSelector: Picks a conversation by flag/chapter/trust/priority.
  - Triggers: TriggerSystem for NPC/Phone/Ambient; time windows; starts Conversa for phone triggers; legacy OfferPrompt
  as fallback; grants items and adds errands.

  UI

  - HUD: Shows time/money/energy/blessings via Soap events.
  - Menus: MainMenu and PauseMenu wired to AppFlowManager/GameStateManager.
  - Journal: Opens on day end with stub content; closes to start next day.
  - Lists: ActiveErrandsUI, InventoryUIExtended reflect current state; equip/unequip controls.

  Assets & Settings

  - Events: Many ScriptableEvent* assets exist for decoupling.
  - Configs: TimeConfig.asset, ResourceConfig.asset, MyControls.inputactions present.
  - URP: UniversalRP.asset, Renderer2D.asset configured.
  - Scenes: SCN_Menu, SCN_Chawl included; AppFlowManager references names centrally.

  Build & Tests

  - Build scripts: No custom build C# entry found; build via Editor or CI with your own method.
  - Tests: No Assets/Tests detected. Recommend adding focused EditMode tests for ErrandSystem, InventorySystem,
  TimeSystem.

  Style & Conventions

  - Namespaces: Mostly BOH. Could optionally align to folder roots (BOH.Core, BOH.Features.*) for clarity.
  - Fields: Private [SerializeField] camelCase and class/prop PascalCase followed well.
  - Events: Good ScriptableObject event usage; minimal scene hard refs.

  Notable Issues

  - GameStateManager.ReturnToMenu: Calls AppFlowManager.Instance?.LoadMainMenu() twice; remove duplicate.
  - HUD money text: HUDController.UpdateMoney shows garbled  ,1{value}; replace with plain numeric or locale-safe format
  (e.g., moneyText.text = $"₹{value:N0}" or moneyText.text = value.ToString("N0")).
  - Encoding artifacts: Several files and docs contain bad characters (e.g., bullets/arrows in UI text, documentation
  headings). Save as UTF-8 and normalize strings in UI building code.
  - Time parsing: Multiple places parse hour via GetTimeString() substring. Prefer exposing numeric hour/minute getters
  from TimeSystem to avoid fragile string parsing.
  - Pause semantics: OfferPrompt sets Time.timeScale=0. Ensure consistency with GameStateManager pause flow to avoid
  conflicts (e.g., resuming state).
  - Input actions: .inputactions present; no generated wrapper in repo (normal). After editing, regenerate wrappers
  in Unity.

  Recommendations

  - Fix quick bugs: duplicate LoadMainMenu call and HUD money format.
  - Add numeric time API in TimeSystem (e.g., GetHour(), GetMinute()), update ErrandSystem/TriggerSystem.
  - Normalize text/encoding: fix UI bullet icons and docs encoding to UTF-8; replace special chars with standard glyphs.
  - Journal improvements: summarize completed/late errands and rewards pulled from ErrandSystem.
  - Scene bootstrapping: Ensure GameServicesInstaller is present and wired in gameplay scenes; verify
  MyConversaController exists.
  - Tests: Add Assets/Tests/EditMode targets for:
      - Errand requirement checks, status transitions, follow-up chaining.
      - Inventory add/consume/equip/clear-day logic.
      - TimeSystem minute tick and end-of-day behavior.
  - Optional: Introduce sub-namespaces (BOH.Features.*) incrementally alongside asmdef reintroduction once stable.