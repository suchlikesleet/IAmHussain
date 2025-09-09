using UnityEngine;
using UnityEngine.SceneManagement;

namespace BOH
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureGameServicesInstaller()
        {
            // If one already exists (or a persistent one from a previous scene), do nothing
            if (Object.FindFirstObjectByType<GameServicesInstaller>())
                return;

            var go = new GameObject("GameServicesInstaller");
            go.AddComponent<GameServicesInstaller>();
            Object.DontDestroyOnLoad(go);
        }
    }
}

