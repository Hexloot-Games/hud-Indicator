using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUDIndicator.Demo {

    /// <summary>
    /// Builds the demo scene from scratch so it always matches the current API. Creates an orbiting
    /// camera, a few labelled targets (some moving) with distance readouts, and a HUD canvas +
    /// Indicator Renderer that the indicators auto-discover at runtime.
    /// </summary>
    public static class DemoSceneBuilder {

        private const string TexturesFolder = "Assets/HUD Indicator/Demo/Textures";
        private const string MaterialsFolder = "Assets/HUD Indicator/Demo/Materials";
        private const string ScenePath = "Assets/HUD Indicator/Demo/Demo.unity";

        // Accent colors.
        private static readonly Color Gold = new Color(1f, 0.82f, 0.25f);
        private static readonly Color Red = new Color(0.92f, 0.28f, 0.28f);
        private static readonly Color Green = new Color(0.38f, 0.85f, 0.45f);
        private static readonly Color Cyan = new Color(0.35f, 0.8f, 0.95f);

        [MenuItem("Tools/HUD Indicator/Rebuild Demo Scene")]
        public static void Build() {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            SetupCamera();
            CreateGround();

            Texture arrow = LoadTexture("arrow");

            // Static objective, far away, shows its distance on and off screen.
            CreateTarget("Objective", PrimitiveType.Cube, new Vector3(0f, 1f, 18f), 1.6f, Gold,
                LoadTexture("unity_logo_icon_249311"), arrow, "Objective",
                showDistance: true, textOnScreen: true, textOffScreen: true);

            // Enemy orbits the arena, so it repeatedly leaves the view and shows the edge arrow.
            GameObject enemy = CreateTarget("Enemy", PrimitiveType.Capsule, new Vector3(12f, 1f, 0f), 1f, Red,
                LoadTexture("diablo-skull"), arrow, "Enemy",
                showDistance: true, textOnScreen: true, textOffScreen: true);
            var orbit = enemy.AddComponent<Orbit>();
            orbit.center = Vector3.zero;
            orbit.radius = 12f;
            orbit.height = 1f;
            orbit.degreesPerSecond = 22f;

            // Ally: label only while on screen.
            CreateTarget("Ally", PrimitiveType.Sphere, new Vector3(-9f, 1f, 4f), 1.2f, Green,
                LoadTexture("P1-On"), arrow, "Ally",
                showDistance: true, textOnScreen: true, textOffScreen: false);

            // Loot: no text, just its distance while on screen (distance becomes the single line).
            CreateTarget("Loot", PrimitiveType.Sphere, new Vector3(8f, 0.5f, -5f), 0.7f, Cyan,
                LoadTexture("anvil-impact"), arrow, string.Empty,
                showDistance: true, textOnScreen: true, textOffScreen: false);

            CreateHud();

            EditorSceneManager.MarkSceneDirty(scene);
            bool saved = EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();

            Debug.Log(saved
                ? $"[HUD Indicator] Rebuilt demo scene at {ScenePath}."
                : $"[HUD Indicator] Failed to save demo scene at {ScenePath}.");
        }

        private static void SetupCamera() {
            Camera camera = Object.FindFirstObjectByType<Camera>();
            if (camera == null) {
                var go = new GameObject("Main Camera", typeof(Camera));
                go.tag = "MainCamera";
                camera = go.GetComponent<Camera>();
            }

            camera.transform.position = new Vector3(0f, 9f, -14f);
            camera.transform.rotation = Quaternion.Euler(26f, 0f, 0f);
            var orbit = camera.gameObject.AddComponent<CameraOrbit>();
            orbit.focus = Vector3.zero;
            orbit.degreesPerSecond = 12f;
        }

        private static void CreateGround() {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(6f, 1f, 6f);
            AssignMaterial(ground, "Ground", new Color(0.22f, 0.24f, 0.28f));
        }

        private static GameObject CreateTarget(string name, PrimitiveType shape, Vector3 position, float scale,
            Color accent, Texture icon, Texture arrow, string label,
            bool showDistance, bool textOnScreen, bool textOffScreen) {

            var go = GameObject.CreatePrimitive(shape);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = Vector3.one * scale;
            AssignMaterial(go, name, accent);

            var indicator = go.AddComponent<Indicator>();

            indicator.Icon.texture = icon;
            indicator.Icon.color = accent;
            indicator.Icon.width = 48f;
            indicator.Icon.height = 48f;

            indicator.ArrowStyle.texture = arrow;
            indicator.ArrowStyle.color = accent;
            indicator.ArrowStyle.width = 26f;
            indicator.ArrowStyle.height = 26f;
            indicator.ArrowStyle.margin = 8f;

            indicator.Label = label;
            indicator.ShowDistance = showDistance;
            indicator.ShowTextOnScreen = textOnScreen;
            indicator.ShowTextOffScreen = textOffScreen;
            indicator.TextStyle.fontSize = 22f;
            indicator.TextStyle.color = Color.white;

            return go;
        }

        private static void CreateHud() {
            var canvasGO = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var rendererGO = new GameObject("Indicator Renderer", typeof(RectTransform));
            var rect = (RectTransform)rendererGO.transform;
            rect.SetParent(canvas.transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var renderer = rendererGO.AddComponent<IndicatorRenderer>();
            renderer.Camera = Camera.main;
            renderer.Margin = 56f;
            // Indicators leave their renderer list empty, so they auto-discover this one at runtime.
        }

        private static void AssignMaterial(GameObject go, string name, Color color) {
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null) renderer.sharedMaterial = CreateMaterial(name, color);
        }

        private static Material CreateMaterial(string name, Color color) {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            var material = new Material(shader) { name = name, color = color };
            string path = $"{MaterialsFolder}/{name}.mat";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Texture LoadTexture(string fileName) {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{TexturesFolder}/{fileName}.png");
            if (texture == null) {
                Debug.LogWarning($"[HUD Indicator] Demo texture not found: {TexturesFolder}/{fileName}.png");
            }
            return texture;
        }
    }
}
