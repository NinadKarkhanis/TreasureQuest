#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AssetUsageReport
{
    [MenuItem("Tools/Treasure Quest/Export Asset Usage Report")]
    public static void Export()
    {
        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        var used = new HashSet<string>(AssetDatabase.GetDependencies(scenes, true));
        // Dynamically loaded assets are not discoverable from scene references.
        string[] paths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/") && !p.EndsWith(".cs") && !p.EndsWith(".meta"))
            .OrderBy(p => p).ToArray();
        string filename = EditorUtility.SaveFilePanel("Save asset review CSV", "", "TreasureQuest-asset-audit.csv", "csv");
        if (string.IsNullOrEmpty(filename)) return;
        using (var writer = new StreamWriter(filename))
        {
            writer.WriteLine("Path,Status,Notes");
            foreach (string path in paths)
            {
                bool protectedAsset = path.Contains("/Resources/") || path.Contains("/Editor/") ||
                    path.Contains("/Photon/") || path.Contains("/Plugins/") || path.Contains("/StreamingAssets/");
                string status = used.Contains(path) ? "Referenced in enabled scenes" :
                    (protectedAsset ? "Needs manual inspection (code/package dependency)" : "Candidate for manual review");
                writer.WriteLine("\"" + path.Replace("\"", "\"\"") + "\",\"" + status + "\",\"Never auto-delete assets from this report\"");
            }
        }
        Debug.Log("Treasure Quest asset usage report saved to: " + filename);
        EditorUtility.RevealInFinder(filename);
    }
}
#endif
