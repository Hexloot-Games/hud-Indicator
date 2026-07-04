using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HUDIndicator {

    /// <summary>
    /// Wire-up helpers so users can get a working HUD in one click: create a full-screen
    /// Indicator Renderer (adding a Canvas if the scene has none) with the main camera assigned.
    /// </summary>
    public static class HUDIndicatorMenu {

        [MenuItem("GameObject/UI/HUD Indicator Renderer", false, 2100)]
        [MenuItem("Tools/HUD Indicator/Create Indicator Renderer", false, 0)]
        public static void CreateIndicatorRenderer() {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            GameObject createdCanvas = null;

            if (canvas == null) {
                createdCanvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = createdCanvas.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = createdCanvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                Undo.RegisterCreatedObjectUndo(createdCanvas, "Create HUD Indicator Renderer");
            }

            var go = new GameObject("Indicator Renderer", typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(canvas.transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var renderer = go.AddComponent<IndicatorRenderer>();
            renderer.Camera = Camera.main;

            Undo.RegisterCreatedObjectUndo(go, "Create HUD Indicator Renderer");
            Selection.activeGameObject = go;

            if (Camera.main == null) {
                Debug.LogWarning("[HUD Indicator] Created an Indicator Renderer but no 'MainCamera' was found. " +
                                 "Assign a Camera on the Indicator Renderer.", renderer);
            }
        }
    }
}
