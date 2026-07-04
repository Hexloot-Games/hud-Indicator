using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Project-only helper (outside the shipped HUD Indicator folder) that creates a Universal Render
/// Pipeline asset + renderer and makes URP the active pipeline. Run once via
/// <c>-executeMethod URPSetup.Setup</c> or the Tools menu.
/// </summary>
public static class URPSetup {

    private const string SettingsFolder = "Assets/Settings";
    private const string RendererPath = SettingsFolder + "/UniversalRenderer.asset";
    private const string AssetPath = SettingsFolder + "/UniversalRenderPipelineAsset.asset";

    [MenuItem("Tools/HUD Indicator/Configure URP (project)")]
    public static void Setup() {
        if (!AssetDatabase.IsValidFolder(SettingsFolder)) {
            AssetDatabase.CreateFolder("Assets", "Settings");
        }

        var rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
        rendererData.name = "UniversalRenderer";
        AssetDatabase.CreateAsset(rendererData, RendererPath);

        UniversalRenderPipelineAsset urp = UniversalRenderPipelineAsset.Create(rendererData);
        urp.name = "UniversalRenderPipelineAsset";
        AssetDatabase.CreateAsset(urp, AssetPath);
        AssetDatabase.SaveAssets();

        GraphicsSettings.defaultRenderPipeline = urp;
        QualitySettings.renderPipeline = urp;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[HUD Indicator] URP configured and set as the active render pipeline ({AssetPath}).");
    }
}
