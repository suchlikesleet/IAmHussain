# BOH Project Updates

This log tracks notable code and content changes. Keep appending new entries at the top with date and a concise summary of what changed and why.

## 2025-09-09

- Conversa: Added missing editor view and menu entry for `TimeBeforeNode`.
  - Files: `BOH/Scripts/Features/Dialogue/BOHConversa/Editor/TimeBeforeNodeView.cs`, updated `ErrandNodesMenuModifier.cs`.
- Conversa: Added new high‑impact custom nodes (+ editor views) and exposed them in the node menu.
  - Time: `TimeAfterNode`, `TimeBetweenNode` (+ `TimeAfterNodeView`, `TimeBetweenNodeView`).
  - Resources: `ResourceCheckNode`, `ResourceChangeNode` (+ views).
  - Journal: `JournalAddEntryNode` (+ view).
  - Inventory: `EquippedCheckNode` (+ view).
  - Errands: `ErrandStateBranchNode` (+ view).
  - Menu updated to include new nodes under logical sections (Time, Resources, Journal, Inventory, Errands).
- Unity 6 API: Replaced obsolete `FindObjectOfType<ErrandSystem>()` with `FindFirstObjectByType<ErrandSystem>()` in `ActiveErrandsUI` to remove CS0618 warnings.

Notes:
- Unity will generate `.meta` files for new `.cs` assets on next import; commit those `.meta` files.
- Ensure `GameServicesInstaller` exists in gameplay scenes so nodes can resolve services reliably at runtime.

