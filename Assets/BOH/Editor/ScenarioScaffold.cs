#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace BOH.Editor
{
    public static class ScenarioScaffold
    {
        private const string ScriptablesRoot = "Assets/BOH/Scriptables";
        private const string ItemsFolder = ScriptablesRoot + "/Items";

        [MenuItem("BOH/Scenarios/Create Morning Errand Placeholders")] 
        public static void CreateMorningErrandPlaceholders()
        {
            EnsureFolder(ItemsFolder);

            // Create or load ItemSO: MEDS_AUNTY
            var itemPath = ItemsFolder + "/MEDS_AUNTY.asset";
            var item = AssetDatabase.LoadAssetAtPath<ItemSO>(itemPath);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<ItemSO>();
                item.itemId = "MEDS_AUNTY";
                item.displayName = "Aunty's Medicine";
                item.description = "Prescription prepared by the Corner Chemist for Aunty Zainab.";
                item.category = ItemSO.ItemCategory.Consumable;
                item.isPersistent = true;
                item.isStackable = true;
                item.isEquippable = false;
                item.buyPrice = 5;
                item.sellPrice = 0;
                AssetDatabase.CreateAsset(item, itemPath);
            }

            // Create or load ErrandSO: ERR_Meds_Aunty
            var errandPath = ScriptablesRoot + "/ERR_Meds_Aunty.asset";
            var errand = AssetDatabase.LoadAssetAtPath<ErrandSO>(errandPath);
            if (errand == null)
            {
                errand = ScriptableObject.CreateInstance<ErrandSO>();
                errand.errandId = "ERR_Meds_Aunty";
                errand.errandTitle = "Morning Errand — Aunty's Medicine";
                errand.description = "Buy medicine from the Corner Chemist and deliver it to Aunty before 09:00.";
                errand.type = ErrandSO.ErrandType.Strict;
                errand.startDay = 1;
                errand.expiryDay = 1;
                errand.startHour = 6;
                errand.endHour = 9; // punctual window
                errand.energyCost = 2;
                errand.blessingsReward = 1;
                errand.moneyReward = 5;
                errand.offeredByNpcId = "NPC_AuntyZainab";
                errand.priority = 10;
                AssetDatabase.CreateAsset(errand, errandPath);
            }

            // Wire requirement to created item (ensure one entry exists)
            if (item != null && errand != null)
            {
                if (errand.itemsRequired == null)
                    errand.itemsRequired = new System.Collections.Generic.List<ErrandSO.ItemRequirement>();

                if (errand.itemsRequired.Count == 0)
                    errand.itemsRequired.Add(new ErrandSO.ItemRequirement { item = item, count = 1 });
                else
                {
                    errand.itemsRequired[0].item = item;
                    errand.itemsRequired[0].count = 1;
                }

                EditorUtility.SetDirty(errand);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.objects = new Object[] { errand, item };
            Debug.Log("Created/updated placeholders: ERR_Meds_Aunty and MEDS_AUNTY");
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
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
    }
}
#endif

