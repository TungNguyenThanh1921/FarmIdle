#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TDevTools : EditorWindow
{
    [MenuItem("TDev/Open Toolbox")]
    public static void OpenToolbox()
    {
        GetWindow<TDevTools>("TDev Toolbox");
    }

    #region Editor Window GUI

    private void OnGUI()
    {
        GUILayout.Label("Sprite Texture Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("🟢 Apply Android Sprite Settings"))
        {
            if (EditorUtility.DisplayDialog("Xác nhận",
                "Bạn có chắc muốn áp dụng cài đặt sprite cho Android (FilterMode: Point, RGBA32)?",
                "OK", "Huỷ"))
            {
                ApplySpriteSettings(BuildTarget.Android);
            }
        }

        if (GUILayout.Button("🔵 Apply iOS Sprite Settings"))
        {
            if (EditorUtility.DisplayDialog("Xác nhận",
                "Bạn có chắc muốn áp dụng cài đặt sprite cho iOS (FilterMode: Point, RGBA32)?",
                "OK", "Huỷ"))
            {
                ApplySpriteSettings(BuildTarget.iOS);
            }
        }
    }

    #endregion

    #region Sprite Setting Logic

    private static void ApplySpriteSettings(BuildTarget target)
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            bool changed = false;

            // Nếu chưa là Sprite thì đổi sang Sprite
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }

            importer.mipmapEnabled = false;

            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(target.ToString());
            if (!settings.overridden || settings.format != TextureImporterFormat.RGBA32)
            {
                settings.overridden = true;
                settings.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(settings);
                changed = true;
            }

            if (changed)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                count++;
            }
        }

        EditorUtility.DisplayDialog("Hoàn tất", $"Đã chỉnh {count} texture(s) thành sprite và áp dụng cho {target}.", "OK");
    }

    #endregion
}
#endif