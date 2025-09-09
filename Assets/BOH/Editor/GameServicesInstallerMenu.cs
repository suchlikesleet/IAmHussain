// Editor utility to create and wire a GameServicesInstaller in the scene or save as a prefab
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BOH.Editor
{
    public static class GameServicesInstallerMenu
    {
        [MenuItem("BOH/Create GameServices Installer (Scene)")]
        public static void CreateInScene()
        {
            var go = new GameObject("GameServicesInstaller");
            var installer = go.AddComponent<GameServicesInstaller>();
            AutoAssign(installer);
            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create GameServicesInstaller");
        }

        [MenuItem("BOH/Create GameServices Installer Prefab...")]
        public static void CreatePrefab()
        {
            var go = new GameObject("GameServicesInstaller");
            var installer = go.AddComponent<GameServicesInstaller>();
            AutoAssign(installer);

            var path = EditorUtility.SaveFilePanelInProject(
                "Save GameServicesInstaller Prefab",
                "GameServicesInstaller",
                "prefab",
                "Choose location for the installer prefab.");

            if (!string.IsNullOrEmpty(path))
            {
                PrefabUtility.SaveAsPrefabAsset(go, path, out bool success);
                if (!success)
                    Debug.LogError("Failed to save GameServicesInstaller prefab");
                else
                    Debug.Log($"Saved GameServicesInstaller prefab at {path}");
            }

            Object.DestroyImmediate(go);
        }

        [MenuItem("BOH/Generate Prewired Installer Prefab (Default Path)")]
        public static void CreatePrefabDefault()
        {
            const string folder = "Assets/BOH/Prefabs";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                var parts = folder.Split('/');
                var path = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = path + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(path, parts[i]);
                    path = next;
                }
            }

            var go = new GameObject("GameServicesInstaller");
            var installer = go.AddComponent<GameServicesInstaller>();
            AutoAssign(installer);

            var prefabPath = folder + "/GameServicesInstaller.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath, out bool success);
            if (!success)
                Debug.LogError("Failed to save GameServicesInstaller prefab");
            else
                Debug.Log($"Saved GameServicesInstaller prefab at {prefabPath}");

            Object.DestroyImmediate(go);
        }

        [MenuItem("BOH/Create GameServices Installer From Selection")]
        public static void CreateFromSelection()
        {
            var go = new GameObject("GameServicesInstaller");
            var installer = go.AddComponent<GameServicesInstaller>();

            foreach (var obj in Selection.gameObjects)
            {
                AssignFrom(obj, installer);
            }

            Selection.activeObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create GameServicesInstaller (From Selection)");
        }

        private static void AutoAssign(GameServicesInstaller installer)
        {
            // Try to pick first found instance for each referenced type
            installer.GetType().GetField("errandSystem",  System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<ErrandSystem>());
            installer.GetType().GetField("inventorySystem",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<InventorySystem>());
            installer.GetType().GetField("contactSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<ContactSystem>());
            installer.GetType().GetField("resourceSystem",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<ResourceSystem>());
            installer.GetType().GetField("timeSystem",    System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<TimeSystem>());

            installer.GetType().GetField("flagService",   System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<FlagService>());
            installer.GetType().GetField("storyService",  System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<StoryService>());
            installer.GetType().GetField("giftingSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<GiftingSystem>());
            installer.GetType().GetField("journalSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, Object.FindFirstObjectByType<JournalSystem>());
        }

        private static void AssignFrom(GameObject source, GameServicesInstaller installer)
        {
            if (source.TryGetComponent<ErrandSystem>(out var errands))
                installer.GetType().GetField("errandSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, errands);
            if (source.TryGetComponent<InventorySystem>(out var inventory))
                installer.GetType().GetField("inventorySystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, inventory);
            if (source.TryGetComponent<ContactSystem>(out var contacts))
                installer.GetType().GetField("contactSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, contacts);
            if (source.TryGetComponent<ResourceSystem>(out var resources))
                installer.GetType().GetField("resourceSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, resources);
            if (source.TryGetComponent<TimeSystem>(out var time))
                installer.GetType().GetField("timeSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, time);

            if (source.TryGetComponent<FlagService>(out var flags))
                installer.GetType().GetField("flagService", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, flags);
            if (source.TryGetComponent<StoryService>(out var story))
                installer.GetType().GetField("storyService", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, story);
            if (source.TryGetComponent<GiftingSystem>(out var gifting))
                installer.GetType().GetField("giftingSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, gifting);
            if (source.TryGetComponent<JournalSystem>(out var journal))
                installer.GetType().GetField("journalSystem", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(installer, journal);
        }
    }
}
#endif
