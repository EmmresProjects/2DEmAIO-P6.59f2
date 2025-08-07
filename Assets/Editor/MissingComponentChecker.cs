using UnityEngine;
using UnityEditor;
using System.IO;

public class MissingComponentScanner : EditorWindow
{
    [MenuItem("Tools/Scan for Missing Components")]
    public static void Scan()
    {
        Debug.Log("🔍 Starting scan for missing components...");

        // Scan scene objects
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;

            var components = obj.GetComponents<Component>();
            foreach (var comp in components)
            {
                if (comp == null)
                {
                    Debug.LogWarning($"Missing component on scene object: '{obj.name}' in scene '{obj.scene.name}'", obj);
                }
            }
        }

        // Scan prefabs
        string[] prefabPaths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var components = prefab.GetComponentsInChildren<Component>(true);
            foreach (var comp in components)
            {
                if (comp == null)
                {
                    Debug.LogWarning($"Missing component in prefab: '{prefab.name}' at path '{path}'", prefab);
                }
            }
        }

        Debug.Log("✅ Scan complete.");
    }
}
