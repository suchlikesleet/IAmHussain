using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class AnimationRetargeter : EditorWindow
{
    [System.Serializable]
    public class PathMapping
    {
        public string oldPath;
        public string newPath;
        public bool isEnabled = true;
        public bool isAutoDetected = false;
    }

    [System.Serializable]
    public class AnimationInfo
    {
        public AnimationClip clip;
        public List<string> brokenPaths = new List<string>();
        public List<string> validPaths = new List<string>();
        public bool isSelected = true;
    }

    private List<AnimationInfo> animationClips = new List<AnimationInfo>();
    private List<PathMapping> pathMappings = new List<PathMapping>();
    private GameObject targetGameObject;
    private Vector2 scrollPosition;
    private Vector2 mappingScrollPosition;
    private bool showValidPaths = false;
    private bool autoDetectEnabled = true;
    private int selectedTab = 0;
    private string[] tabNames = { "Scan Animations", "Path Mappings", "Settings" };

    [MenuItem("Tools/Animation Retargeter")]
    public static void ShowWindow()
    {
        var window = GetWindow<AnimationRetargeter>("Animation Retargeter");
        window.minSize = new Vector2(600, 400);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // Header
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Retargeting Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Tabs
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
        EditorGUILayout.Space();

        switch (selectedTab)
        {
            case 0:
                DrawScanTab();
                break;
            case 1:
                DrawMappingTab();
                break;
            case 2:
                DrawSettingsTab();
                break;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawScanTab()
    {
        // Target GameObject
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Target GameObject:", GUILayout.Width(120));
        targetGameObject = (GameObject)EditorGUILayout.ObjectField(targetGameObject, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Selected Animation Clips"))
        {
            ScanSelectedAnimationClips();
        }
        if (GUILayout.Button("Scan All Animation Clips in Project"))
        {
            ScanAllAnimationClips();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (animationClips.Count > 0)
        {
            // Summary
            int totalClips = animationClips.Count;
            int clipsWithIssues = animationClips.Count(a => a.brokenPaths.Count > 0);
            
            EditorGUILayout.LabelField($"Found {totalClips} clips, {clipsWithIssues} have broken references", EditorStyles.helpBox);
            
            EditorGUILayout.Space();

            // Controls
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select All"))
            {
                animationClips.ForEach(a => a.isSelected = true);
            }
            if (GUILayout.Button("Select None"))
            {
                animationClips.ForEach(a => a.isSelected = false);
            }
            if (GUILayout.Button("Select Broken Only"))
            {
                animationClips.ForEach(a => a.isSelected = a.brokenPaths.Count > 0);
            }
            showValidPaths = EditorGUILayout.Toggle("Show Valid Paths", showValidPaths);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Animation list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (var animInfo in animationClips)
            {
                DrawAnimationInfo(animInfo);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Action buttons
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = pathMappings.Count > 0;
            if (GUILayout.Button("Apply Retargeting to Selected Clips"))
            {
                ApplyRetargeting();
            }
            GUI.enabled = true;
            
            if (GUILayout.Button("Generate Path Mappings"))
            {
                GeneratePathMappings();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawMappingTab()
    {
        EditorGUILayout.LabelField("Path Mappings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Manual Mapping"))
        {
            pathMappings.Add(new PathMapping());
        }
        if (GUILayout.Button("Clear All Mappings"))
        {
            if (EditorUtility.DisplayDialog("Clear Mappings", "Are you sure you want to clear all path mappings?", "Yes", "No"))
            {
                pathMappings.Clear();
            }
        }
        if (GUILayout.Button("Auto-Detect Mappings"))
        {
            AutoDetectMappings();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (pathMappings.Count == 0)
        {
            EditorGUILayout.HelpBox("No path mappings defined. Use 'Generate Path Mappings' in the Scan tab or add manual mappings.", MessageType.Info);
            return;
        }

        // Mappings list
        mappingScrollPosition = EditorGUILayout.BeginScrollView(mappingScrollPosition);

        for (int i = pathMappings.Count - 1; i >= 0; i--)
        {
            DrawPathMapping(pathMappings[i], i);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSettingsTab()
    {
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        autoDetectEnabled = EditorGUILayout.Toggle("Auto-detect similar paths", autoDetectEnabled);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Backup & Safety", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("This tool modifies animation clips directly. Make sure to backup your project before using.", MessageType.Warning);
        
        if (GUILayout.Button("Create Backup of Selected Clips"))
        {
            CreateBackup();
        }
    }

    private void DrawAnimationInfo(AnimationInfo animInfo)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        animInfo.isSelected = EditorGUILayout.Toggle(animInfo.isSelected, GUILayout.Width(20));
        EditorGUILayout.ObjectField(animInfo.clip, typeof(AnimationClip), false);
        
        // Status indicator
        if (animInfo.brokenPaths.Count > 0)
        {
            GUI.color = Color.red;
            EditorGUILayout.LabelField($"⚠ {animInfo.brokenPaths.Count} broken", GUILayout.Width(100));
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = Color.green;
            EditorGUILayout.LabelField("✓ OK", GUILayout.Width(100));
            GUI.color = Color.white;
        }
        EditorGUILayout.EndHorizontal();

        // Show broken paths
        if (animInfo.brokenPaths.Count > 0)
        {
            EditorGUILayout.LabelField("Broken Paths:", EditorStyles.boldLabel);
            foreach (var path in animInfo.brokenPaths)
            {
                EditorGUILayout.LabelField($"  • {path}", EditorStyles.miniLabel);
            }
        }

        // Show valid paths if requested
        if (showValidPaths && animInfo.validPaths.Count > 0)
        {
            EditorGUILayout.LabelField("Valid Paths:", EditorStyles.boldLabel);
            foreach (var path in animInfo.validPaths.Take(5)) // Show first 5
            {
                EditorGUILayout.LabelField($"  • {path}", EditorStyles.miniLabel);
            }
            if (animInfo.validPaths.Count > 5)
            {
                EditorGUILayout.LabelField($"  ... and {animInfo.validPaths.Count - 5} more", EditorStyles.miniLabel);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawPathMapping(PathMapping mapping, int index)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        mapping.isEnabled = EditorGUILayout.Toggle(mapping.isEnabled, GUILayout.Width(20));
        
        if (mapping.isAutoDetected)
        {
            GUI.color = Color.cyan;
            EditorGUILayout.LabelField("AUTO", GUILayout.Width(40));
            GUI.color = Color.white;
        }

        EditorGUILayout.LabelField("From:", GUILayout.Width(40));
        mapping.oldPath = EditorGUILayout.TextField(mapping.oldPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(60, false);
        EditorGUILayout.LabelField("To:", GUILayout.Width(40));
        mapping.newPath = EditorGUILayout.TextField(mapping.newPath);
        
        if (GUILayout.Button("×", GUILayout.Width(20)))
        {
            pathMappings.RemoveAt(index);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void ScanSelectedAnimationClips()
    {
        var clips = Selection.objects.OfType<AnimationClip>().ToArray();
        if (clips.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select animation clips to scan.", "OK");
            return;
        }

        ScanAnimationClips(clips);
    }

    private void ScanAllAnimationClips()
    {
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip");
        var clips = guids.Select(guid => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        
        if (clips.Length == 0)
        {
            EditorUtility.DisplayDialog("No Clips Found", "No animation clips found in the project.", "OK");
            return;
        }

        ScanAnimationClips(clips);
    }

    private void ScanAnimationClips(AnimationClip[] clips)
    {
        animationClips.Clear();
        
        for (int i = 0; i < clips.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Scanning Animation Clips", $"Processing {clips[i].name}", (float)i / clips.Length);
            
            var animInfo = AnalyzeAnimationClip(clips[i]);
            animationClips.Add(animInfo);
        }
        
        EditorUtility.ClearProgressBar();
        
        Debug.Log($"Scanned {clips.Length} animation clips. Found {animationClips.Count(a => a.brokenPaths.Count > 0)} clips with broken references.");
    }

    private AnimationInfo AnalyzeAnimationClip(AnimationClip clip)
    {
        var animInfo = new AnimationInfo { clip = clip };
        
        var bindings = AnimationUtility.GetCurveBindings(clip);
        var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        
        foreach (var binding in bindings.Concat(objectBindings))
        {
            if (string.IsNullOrEmpty(binding.path))
            {
                animInfo.validPaths.Add("(Root)");
                continue;
            }

            if (targetGameObject != null)
            {
                var transform = targetGameObject.transform.Find(binding.path);
                if (transform == null)
                {
                    animInfo.brokenPaths.Add(binding.path);
                }
                else
                {
                    animInfo.validPaths.Add(binding.path);
                }
            }
            else
            {
                // Without target object, we can't validate paths
                animInfo.validPaths.Add(binding.path);
            }
        }

        // Remove duplicates
        animInfo.brokenPaths = animInfo.brokenPaths.Distinct().ToList();
        animInfo.validPaths = animInfo.validPaths.Distinct().ToList();
        
        return animInfo;
    }

    private void GeneratePathMappings()
    {
        if (targetGameObject == null)
        {
            EditorUtility.DisplayDialog("No Target", "Please assign a target GameObject to generate mappings.", "OK");
            return;
        }

        var allBrokenPaths = animationClips
            .Where(a => a.isSelected)
            .SelectMany(a => a.brokenPaths)
            .Distinct()
            .ToList();

        foreach (var brokenPath in allBrokenPaths)
        {
            // Check if mapping already exists
            if (pathMappings.Any(m => m.oldPath == brokenPath))
                continue;

            var mapping = new PathMapping { oldPath = brokenPath };

            if (autoDetectEnabled)
            {
                var suggestedPath = FindSimilarPath(brokenPath, targetGameObject.transform);
                if (!string.IsNullOrEmpty(suggestedPath))
                {
                    mapping.newPath = suggestedPath;
                    mapping.isAutoDetected = true;
                }
            }

            pathMappings.Add(mapping);
        }

        Debug.Log($"Generated {pathMappings.Count} path mappings.");
    }

    private string FindSimilarPath(string brokenPath, Transform root)
    {
        var pathParts = brokenPath.Split('/');
        var lastName = pathParts[pathParts.Length - 1];

        // Try to find objects with the same name
        var candidates = GetAllChildTransforms(root)
            .Where(t => t.name == lastName)
            .ToList();

        if (candidates.Count == 1)
        {
            return GetTransformPath(candidates[0], root);
        }

        // If multiple candidates, try to find the best match based on partial path
        if (candidates.Count > 1)
        {
            foreach (var candidate in candidates)
            {
                var candidatePath = GetTransformPath(candidate, root);
                var candidateParts = candidatePath.Split('/');
                
                // Check if paths have similar structure
                if (candidateParts.Length == pathParts.Length)
                {
                    int matches = 0;
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        if (pathParts[i] == candidateParts[i])
                            matches++;
                    }
                    
                    if (matches >= pathParts.Length * 0.7f) // 70% similarity
                    {
                        return candidatePath;
                    }
                }
            }
        }

        return "";
    }

    private List<Transform> GetAllChildTransforms(Transform parent)
    {
        var result = new List<Transform>();
        GetAllChildTransformsRecursive(parent, result);
        return result;
    }

    private void GetAllChildTransformsRecursive(Transform parent, List<Transform> result)
    {
        foreach (Transform child in parent)
        {
            result.Add(child);
            GetAllChildTransformsRecursive(child, result);
        }
    }

    private string GetTransformPath(Transform transform, Transform root)
    {
        if (transform == root)
            return "";

        var path = transform.name;
        var current = transform.parent;

        while (current != null && current != root)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private void AutoDetectMappings()
    {
        if (targetGameObject == null)
        {
            EditorUtility.DisplayDialog("No Target", "Please assign a target GameObject for auto-detection.", "OK");
            return;
        }

        int detected = 0;
        foreach (var mapping in pathMappings.Where(m => string.IsNullOrEmpty(m.newPath)))
        {
            var suggestedPath = FindSimilarPath(mapping.oldPath, targetGameObject.transform);
            if (!string.IsNullOrEmpty(suggestedPath))
            {
                mapping.newPath = suggestedPath;
                mapping.isAutoDetected = true;
                detected++;
            }
        }

        Debug.Log($"Auto-detected {detected} path mappings.");
    }

    private void ApplyRetargeting()
    {
        var selectedClips = animationClips.Where(a => a.isSelected).ToList();
        var enabledMappings = pathMappings.Where(m => m.isEnabled && !string.IsNullOrEmpty(m.newPath)).ToList();

        if (selectedClips.Count == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select animation clips to retarget.", "OK");
            return;
        }

        if (enabledMappings.Count == 0)
        {
            EditorUtility.DisplayDialog("No Mappings", "Please define path mappings before retargeting.", "OK");
            return;
        }

        bool confirmed = EditorUtility.DisplayDialog("Confirm Retargeting", 
            $"This will modify {selectedClips.Count} animation clips using {enabledMappings.Count} path mappings.\n\nThis action cannot be undone. Continue?", 
            "Yes", "Cancel");

        if (!confirmed)
            return;

        int processedClips = 0;
        int totalModifications = 0;

        foreach (var animInfo in selectedClips)
        {
            EditorUtility.DisplayProgressBar("Retargeting Animations", $"Processing {animInfo.clip.name}", (float)processedClips / selectedClips.Count);
            
            int modifications = RetargetAnimationClip(animInfo.clip, enabledMappings);
            totalModifications += modifications;
            processedClips++;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();

        Debug.Log($"Retargeting complete! Modified {totalModifications} bindings across {processedClips} clips.");
        
        // Rescan to update the display
        ScanAnimationClips(selectedClips.Select(a => a.clip).ToArray());
    }

    private int RetargetAnimationClip(AnimationClip clip, List<PathMapping> mappings)
    {
        int modifications = 0;
        
        // Handle curve bindings
        var bindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var binding in bindings)
        {
            var mapping = mappings.FirstOrDefault(m => m.oldPath == binding.path);
            if (mapping != null)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                var newBinding = binding;
                newBinding.path = mapping.newPath;
                
                AnimationUtility.SetEditorCurve(clip, binding, null); // Remove old
                AnimationUtility.SetEditorCurve(clip, newBinding, curve); // Add new
                modifications++;
            }
        }

        // Handle object reference bindings
        var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
        foreach (var binding in objectBindings)
        {
            var mapping = mappings.FirstOrDefault(m => m.oldPath == binding.path);
            if (mapping != null)
            {
                var curve = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                var newBinding = binding;
                newBinding.path = mapping.newPath;
                
                AnimationUtility.SetObjectReferenceCurve(clip, binding, null); // Remove old
                AnimationUtility.SetObjectReferenceCurve(clip, newBinding, curve); // Add new
                modifications++;
            }
        }

        if (modifications > 0)
        {
            EditorUtility.SetDirty(clip);
        }

        return modifications;
    }

    private void CreateBackup()
    {
        var selectedClips = animationClips.Where(a => a.isSelected).Select(a => a.clip).ToArray();
        
        if (selectedClips.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select animation clips to backup.", "OK");
            return;
        }

        string backupFolder = $"Assets/AnimationBackups_{System.DateTime.Now:yyyyMMdd_HHmmss}";
        AssetDatabase.CreateFolder("Assets", Path.GetFileName(backupFolder));

        foreach (var clip in selectedClips)
        {
            string originalPath = AssetDatabase.GetAssetPath(clip);
            string backupPath = Path.Combine(backupFolder, Path.GetFileName(originalPath));
            AssetDatabase.CopyAsset(originalPath, backupPath);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Created backup of {selectedClips.Length} clips in {backupFolder}");
    }
}