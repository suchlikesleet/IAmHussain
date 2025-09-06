using UnityEngine;

namespace BOH
{
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "BOH/Config/ResourceConfig")]
    public class ResourceConfigSO : ScriptableObject
    {
        [Header("Starting Values")]
        public int startMoney = 50;
        public int startEnergy = 5;
        public int startBlessings = 0;
        
        [Header("Limits")]
        public int maxEnergy = 10;
        public int maxMoney = 999;
    }

    // ScriptRole: Resource system configuration
    // RelatedScripts: ResourceSystem
}