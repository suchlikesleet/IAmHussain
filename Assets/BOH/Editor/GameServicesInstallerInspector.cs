// Custom inspector for GameServicesInstaller with status colors and quick actions
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BOH.Editor
{
    [CustomEditor(typeof(GameServicesInstaller))]
    public class GameServicesInstallerInspector : UnityEditor.Editor
    {
        private SerializedProperty errandSystemProp;
        private SerializedProperty inventorySystemProp;
        private SerializedProperty contactSystemProp;
        private SerializedProperty resourceSystemProp;
        private SerializedProperty timeSystemProp;

        private SerializedProperty flagServiceProp;
        private SerializedProperty storyServiceProp;
        private SerializedProperty giftingSystemProp;
        private SerializedProperty journalSystemProp;

        private SerializedProperty dontDestroyProp;

        private GUIStyle headerStyle;

        private void OnEnable()
        {
            errandSystemProp   = serializedObject.FindProperty("errandSystem");
            inventorySystemProp= serializedObject.FindProperty("inventorySystem");
            contactSystemProp  = serializedObject.FindProperty("contactSystem");
            resourceSystemProp = serializedObject.FindProperty("resourceSystem");
            timeSystemProp     = serializedObject.FindProperty("timeSystem");

            flagServiceProp    = serializedObject.FindProperty("flagService");
            storyServiceProp   = serializedObject.FindProperty("storyService");
            giftingSystemProp  = serializedObject.FindProperty("giftingSystem");
            journalSystemProp  = serializedObject.FindProperty("journalSystem");

            dontDestroyProp    = serializedObject.FindProperty("dontDestroyOnLoad");

            headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Game Services Installer", headerStyle);
            EditorGUILayout.HelpBox("Assign scene references for core and optional services. Use Auto-Assign to pick the first scene instance.", MessageType.Info);

            DrawSettings();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Core Systems", EditorStyles.boldLabel);
            DrawObjectFieldWithStatus(errandSystemProp,   "Errand System");
            DrawObjectFieldWithStatus(inventorySystemProp,"Inventory System");
            DrawObjectFieldWithStatus(contactSystemProp,  "Contact System");
            DrawObjectFieldWithStatus(resourceSystemProp, "Resource System");
            DrawObjectFieldWithStatus(timeSystemProp,     "Time System");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Optional Services", EditorStyles.boldLabel);
            DrawObjectFieldWithStatus(flagServiceProp,    "Flag Service (IFlagService)", optional:true);
            DrawObjectFieldWithStatus(storyServiceProp,   "Story Service (IStoryService)", optional:true);
            DrawObjectFieldWithStatus(giftingSystemProp,  "Gifting System", optional:true);
            DrawObjectFieldWithStatus(journalSystemProp,  "Journal System", optional:true);

            EditorGUILayout.Space(8);
            DrawActions();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettings()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(dontDestroyProp, new GUIContent("Dont Destroy On Load"));
            }
        }

        private static readonly Color okColor = new Color(0.55f, 0.86f, 0.62f);
        private static readonly Color warnColor = new Color(0.98f, 0.72f, 0.23f);
        private static readonly Color errColor = new Color(0.94f, 0.42f, 0.42f);

        private void DrawObjectFieldWithStatus(SerializedProperty prop, string label, bool optional = false)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(prop, new GUIContent(label));

                var assigned = prop.objectReferenceValue != null;
                var rect = GUILayoutUtility.GetRect(18, 18, GUILayout.Width(18));
                var c = assigned ? okColor : (optional ? warnColor : errColor);
                using (new GUIColorScope(c))
                {
                    GUI.Box(rect, GUIContent.none);
                }

                if (assigned)
                {
                    if (GUILayout.Button("Ping", GUILayout.Width(44)))
                        EditorGUIUtility.PingObject(prop.objectReferenceValue);
                }
                else
                {
                    if (GUILayout.Button("Find", GUILayout.Width(44)))
                    {
                        var t = prop.serializedObject.targetObject as GameServicesInstaller;
                        var type = GetFieldType(t, prop.propertyPath);
                        var found = FindFirstOfType(type);
                        if (found)
                        {
                            prop.objectReferenceValue = found;
                            EditorUtility.SetDirty(target);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Not Found", $"No object of type {type.Name} found in the scene.", "OK");
                        }
                    }
                }
            }
        }

        private void DrawActions()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Auto-Assign Missing"))
                {
                    AutoAssignMissing();
                }
                if (GUILayout.Button("Validate Now"))
                {
                    InvokeValidateAssignments();
                }
            }
        }

        private void AutoAssignMissing()
        {
            AutoAssignIfNull(errandSystemProp);
            AutoAssignIfNull(inventorySystemProp);
            AutoAssignIfNull(contactSystemProp);
            AutoAssignIfNull(resourceSystemProp);
            AutoAssignIfNull(timeSystemProp);

            AutoAssignIfNull(flagServiceProp);
            AutoAssignIfNull(storyServiceProp);
            AutoAssignIfNull(giftingSystemProp);
            AutoAssignIfNull(journalSystemProp);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void AutoAssignIfNull(SerializedProperty prop)
        {
            if (prop == null || prop.objectReferenceValue != null) return;
            var t = target as GameServicesInstaller;
            var type = GetFieldType(t, prop.propertyPath);
            var found = FindFirstOfType(type);
            if (found) prop.objectReferenceValue = found;
        }

        private static Object FindFirstOfType(System.Type type)
        {
            var method = typeof(Object).GetMethod("FindFirstObjectByType", new System.Type[] { }) ?? typeof(Object).GetMethod("FindObjectOfType", new[] { typeof(System.Type) });
            if (method != null)
            {
                if (method.GetParameters().Length == 0)
                    return method.MakeGenericMethod(type).Invoke(null, null) as Object;
                else
                    return Object.FindFirstObjectByType(type);
            }
            return Object.FindFirstObjectByType(type);
        }

        private static System.Type GetFieldType(object obj, string fieldName)
        {
            var f = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return f != null ? f.FieldType : typeof(Object);
        }

        private void InvokeValidateAssignments()
        {
            var mi = typeof(GameServicesInstaller).GetMethod("ValidateAssignments", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            mi?.Invoke(target, null);
        }

        private readonly struct GUIColorScope : System.IDisposable
        {
            private readonly Color prev;
            public GUIColorScope(Color c)
            {
                prev = GUI.color;
                GUI.color = c;
            }
            public void Dispose() { GUI.color = prev; }
        }
    }
}
#endif

