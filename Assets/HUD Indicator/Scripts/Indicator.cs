using System.Collections.Generic;
using UnityEngine;

namespace HUDIndicator {

    /// <summary>
    /// Attach to any world object you want to point at on the HUD. A single indicator can show an
    /// icon over the target while it is on screen, pin an icon + arrow to the screen edge while it is
    /// off screen, or both — each with its own optional text/distance label. One view is created per
    /// <see cref="IndicatorRenderer"/>, which is what makes split-screen work with no extra code.
    /// </summary>
    [AddComponentMenu("HUD Indicator/Indicator")]
    [DisallowMultipleComponent]
    public class Indicator : MonoBehaviour {

        [Tooltip("Master visibility switch for this indicator across every renderer.")]
        [SerializeField] private bool visible = true;

        [Tooltip("Renderers this indicator is drawn on. Leave empty to use every renderer in the scene.")]
        [SerializeField] private List<IndicatorRenderer> renderers = new List<IndicatorRenderer>();

        [Tooltip("Icon drawn for this indicator, on screen and off screen.")]
        [SerializeField] private IndicatorIconStyle icon = new IndicatorIconStyle();

        [Header("On Screen")]
        [Tooltip("Show an icon over the target while it is visible on screen.")]
        [SerializeField] private bool showOnScreen = true;

        [Tooltip("Shrink the on-screen icon as the target gets further from the camera.")]
        [SerializeField] private bool scaleWithDistance = false;

        [Min(0f)] [SerializeField] private float nearDistance = 5f;
        [Min(0f)] [SerializeField] private float farDistance = 50f;
        [Min(0f)] [SerializeField] private float minScale = 0.5f;
        [Min(0f)] [SerializeField] private float maxScale = 1f;

        [Header("Off Screen")]
        [Tooltip("Pin the icon to the screen edge while the target is off screen.")]
        [SerializeField] private bool showOffScreen = true;

        [Tooltip("Draw a directional arrow that points from the screen centre towards the target.")]
        [SerializeField] private bool showArrow = true;

        [SerializeField] private IndicatorArrowStyle arrowStyle = new IndicatorArrowStyle();

        [Tooltip("Rotate the icon itself to face the target while off screen (the label stays upright).")]
        [SerializeField] private bool rotateIcon = false;

        [Header("Label")]
        [Tooltip("Optional text shown next to the icon. Leave empty for no text.")]
        [SerializeField] private string label = string.Empty;

        [Tooltip("Show the distance to the target. Displayed below the text, or on its own when no text is set.")]
        [SerializeField] private bool showDistance = false;

        [Tooltip("Show the label while the target is on screen.")]
        [SerializeField] private bool showTextOnScreen = true;

        [Tooltip("Show the label while the target is off screen (next to the edge icon).")]
        [SerializeField] private bool showTextOffScreen = false;

        [SerializeField] private IndicatorTextStyle textStyle = new IndicatorTextStyle();

        private readonly List<IndicatorView> _views = new List<IndicatorView>();
        private bool _viewsBuilt;

        // --- Public API -----------------------------------------------------------------------

        public bool Visible { get => visible; set => visible = value; }
        public IndicatorIconStyle Icon => icon;

        public bool ShowOnScreen { get => showOnScreen; set => showOnScreen = value; }
        public bool ScaleWithDistance { get => scaleWithDistance; set => scaleWithDistance = value; }
        public float NearDistance { get => nearDistance; set => nearDistance = Mathf.Max(0f, value); }
        public float FarDistance { get => farDistance; set => farDistance = Mathf.Max(0f, value); }
        public float MinScale { get => minScale; set => minScale = Mathf.Max(0f, value); }
        public float MaxScale { get => maxScale; set => maxScale = Mathf.Max(0f, value); }

        public bool ShowOffScreen { get => showOffScreen; set => showOffScreen = value; }
        public bool ShowArrow { get => showArrow; set => showArrow = value; }
        public IndicatorArrowStyle ArrowStyle => arrowStyle;
        public bool RotateIcon { get => rotateIcon; set => rotateIcon = value; }

        public string Label { get => label; set => label = value ?? string.Empty; }
        public bool ShowDistance { get => showDistance; set => showDistance = value; }
        public bool ShowTextOnScreen { get => showTextOnScreen; set => showTextOnScreen = value; }
        public bool ShowTextOffScreen { get => showTextOffScreen; set => showTextOffScreen = value; }
        public IndicatorTextStyle TextStyle => textStyle;

        /// <summary>Sets the label text (convenience wrapper around <see cref="Label"/>).</summary>
        public void SetLabel(string text) => Label = text;

        /// <summary>Renderers this indicator is currently drawn on.</summary>
        public IReadOnlyList<IndicatorRenderer> GetRenderers() => renderers;

        /// <summary>Replaces the renderer list with a single renderer and rebuilds the views.</summary>
        public void SetRenderer(IndicatorRenderer renderer) {
            renderers.Clear();
            if (renderer != null) renderers.Add(renderer);
            Rebuild();
        }

        /// <summary>Replaces the renderer list and rebuilds the views.</summary>
        public void SetRenderers(IEnumerable<IndicatorRenderer> value) {
            renderers.Clear();
            if (value != null) renderers.AddRange(value);
            Rebuild();
        }

        /// <summary>Adds a renderer this indicator should draw on and rebuilds the views.</summary>
        public void AddRenderer(IndicatorRenderer renderer) {
            if (renderer == null || renderers.Contains(renderer)) return;
            renderers.Add(renderer);
            Rebuild();
        }

        /// <summary>Removes a renderer this indicator draws on and rebuilds the views.</summary>
        public void RemoveRenderer(IndicatorRenderer renderer) {
            if (renderer == null || !renderers.Remove(renderer)) return;
            Rebuild();
        }

        /// <summary>Destroys and recreates every view. Call after changing the renderer set at runtime.</summary>
        public void Rebuild() {
            if (!_viewsBuilt) return;
            DestroyViews();
            BuildViews();
        }

        // --- Unity life cycle -----------------------------------------------------------------

        private void OnEnable() {
            if (!_viewsBuilt) BuildViews();
        }

        private void LateUpdate() {
            // LateUpdate so positions use the camera's final transform for this frame (less jitter).
            for (int i = 0; i < _views.Count; i++) {
                _views[i].Tick();
            }
        }

        private void OnDisable() {
            for (int i = 0; i < _views.Count; i++) {
                _views[i].Hide();
            }
        }

        private void OnDestroy() {
            DestroyViews();
        }

        // --- Internals ------------------------------------------------------------------------

        private void BuildViews() {
            ResolveRenderers();

            for (int i = 0; i < renderers.Count; i++) {
                IndicatorRenderer renderer = renderers[i];
                if (renderer == null) continue;

                var view = new IndicatorView();
                view.Create(this, renderer);
                _views.Add(view);
            }

            _viewsBuilt = true;
        }

        private void DestroyViews() {
            for (int i = 0; i < _views.Count; i++) {
                _views[i].Destroy();
            }
            _views.Clear();
            _viewsBuilt = false;
        }

        private void ResolveRenderers() {
            renderers.RemoveAll(r => r == null);
            if (renderers.Count > 0) return;

            IndicatorRenderer[] found = FindObjectsByType<IndicatorRenderer>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (found.Length > 0) {
                renderers.AddRange(found);
            }
            else {
                Debug.LogWarning(
                    $"[HUD Indicator] '{name}' has no renderers assigned and none were found in the scene. " +
                    "Add an Indicator Renderer (Tools ▸ HUD Indicator ▸ Create Indicator Renderer).", this);
            }
        }
    }
}
