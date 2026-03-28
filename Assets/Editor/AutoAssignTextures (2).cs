using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class AutoAssignTextures : EditorWindow
{
    private Vector2 scrollPos;
    private string logText = "Click ASSIGN TEXTURES to start.";

    [MenuItem("Tools/Auto Assign Textures")]
    public static void ShowWindow()
    {
        GetWindow<AutoAssignTextures>("Auto Assign Textures");
    }

    void OnGUI()
    {
        GUILayout.Label("Auto Assign Textures to Materials", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("Searches ENTIRE project for rx-480 materials\nand assigns your 8K textures automatically.", EditorStyles.helpBox);
        GUILayout.Space(10);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("ASSIGN TEXTURES NOW", GUILayout.Height(45)))
            AssignTextures();

        GUI.backgroundColor = Color.white;
        GUILayout.Space(10);
        GUILayout.Label("Log:");
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(350));
        GUILayout.TextArea(logText, GUILayout.ExpandHeight(true));
        GUILayout.EndScrollView();
    }

    void AssignTextures()
    {
        logText = "=== STARTING ===\n\n";

        // ---- Find textures anywhere in project ----
        string[] texGUIDs = AssetDatabase.FindAssets("t:Texture2D");
        List<Texture2D> allTex = texGUIDs
            .Select(g => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(t => t != null).ToList();

        Texture2D diffuseTex   = Find(allTex, "Diffuse_Map");
        Texture2D normalTex    = Find(allTex, "Normal_Map");
        Texture2D roughnessTex = Find(allTex, "Roughness_Map");
        Texture2D glossyTex    = Find(allTex, "Glossy_Map");

        logText += $"Textures located:\n";
        logText += $"  Diffuse:   {(diffuseTex   != null ? diffuseTex.name   : "❌ NOT FOUND")}\n";
        logText += $"  Normal:    {(normalTex    != null ? normalTex.name    : "❌ NOT FOUND")}\n";
        logText += $"  Roughness: {(roughnessTex != null ? roughnessTex.name : "❌ NOT FOUND")}\n";
        logText += $"  Glossy:    {(glossyTex    != null ? glossyTex.name    : "❌ NOT FOUND")}\n\n";

        // Fix normal map import
        if (normalTex != null)
        {
            string np = AssetDatabase.GetAssetPath(normalTex);
            TextureImporter ti = AssetImporter.GetAtPath(np) as TextureImporter;
            if (ti != null && ti.textureType != TextureImporterType.NormalMap)
            {
                ti.textureType = TextureImporterType.NormalMap;
                ti.SaveAndReimport();
                logText += "✅ Normal map import fixed.\n\n";
            }
        }

        // ---- Find ALL materials in project ----
        string[] matGUIDs = AssetDatabase.FindAssets("t:Material");
        logText += $"Total materials in project: {matGUIDs.Length}\n\n";

        int updated = 0;
        int skipped = 0;

        foreach (var g in matGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);

            // Only target rx-480 related materials
            if (!path.ToLower().Contains("rx-480") &&
                !path.ToLower().Contains("rx480") &&
                !path.ToLower().Contains("polysurface") &&
                !path.ToLower().Contains("pcube") &&
                !path.ToLower().Contains("pplane"))
            {
                skipped++;
                continue;
            }

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            if (diffuseTex != null)
            {
                mat.SetTexture("_BaseMap", diffuseTex);
                mat.SetTexture("_MainTex",  diffuseTex);
            }
            if (normalTex != null)
            {
                mat.SetTexture("_BumpMap", normalTex);
                mat.EnableKeyword("_NORMALMAP");
                mat.SetFloat("_BumpScale", 1f);
            }
            if (roughnessTex != null)
                mat.SetTexture("_MetallicGlossMap", roughnessTex);
            else if (glossyTex != null)
                mat.SetTexture("_MetallicGlossMap", glossyTex);

            mat.SetFloat("_Metallic",   0.8f);
            mat.SetFloat("_Smoothness", 0.5f);

            EditorUtility.SetDirty(mat);
            logText += $"✅ {mat.name}  [{path}]\n";
            updated++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        logText += $"\n=== DONE: {updated} materials updated, {skipped} skipped ===\n";

        if (updated == 0)
        {
            logText += "\n❌ Still 0 updated!\n";
            logText += "Your materials are still EMBEDDED inside the .obj file.\n\n";
            logText += "DO THIS FIRST:\n";
            logText += "1. Project panel → click rx_480_texture (.obj)\n";
            logText += "2. Inspector → Materials tab\n";
            logText += "3. Click 'Extract Materials...' → save to:\n";
            logText += "   Assets/Prefabs/rx-480-gpu/Materials\n";
            logText += "4. Click Apply\n";
            logText += "5. Run this tool again\n";
        }
    }

    Texture2D Find(List<Texture2D> list, params string[] keys)
    {
        foreach (string k in keys)
        {
            var m = list.FirstOrDefault(t => t.name.ToLower().Contains(k.ToLower()));
            if (m != null) return m;
        }
        return null;
    }
}
