using UnityEngine;
using UnityEngine.Serialization;

namespace HUDIndicator {

    /// <summary>
    /// A HUD viewport: the RectTransform area (under a Canvas) that indicators are drawn inside,
    /// together with the camera used to project world targets onto it. Add one per screen region —
    /// for split-screen, add one renderer per player camera.
    /// </summary>
    [AddComponentMenu("HUD Indicator/Indicator Renderer")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class IndicatorRenderer : MonoBehaviour {

        [Tooltip("When off, every indicator drawn on this renderer is hidden.")]
        [SerializeField] private bool visible = true;

        [Tooltip("Inset from the edges of the rect, in canvas units. Off-screen indicators clamp to " +
                 "this inset border; on-screen indicators switch to off-screen within it.")]
        [Min(0f)] [SerializeField] private float margin = 32f;

        [Tooltip("Camera used to project world targets. Falls back to Camera.main when unset.")]
        [SerializeField] private new Camera camera;

        [Tooltip("Colour of the editor-only gizmo that previews the drawable area.")]
        [FormerlySerializedAs("canvasColor")]
        [SerializeField] private Color gizmoColor = new Color(0f, 207f / 255f, 1f, 27f / 255f);

        private RectTransform _rectTransform;

        public bool Visible {
            get => visible;
            set => visible = value;
        }

        public float Margin {
            get => margin;
            set => margin = Mathf.Max(0f, value);
        }

        public Color GizmoColor {
            get => gizmoColor;
            set => gizmoColor = value;
        }

        /// <summary>The RectTransform indicators are parented to. Cached.</summary>
        public RectTransform RectTransform {
            get {
                if (_rectTransform == null) _rectTransform = (RectTransform)transform;
                return _rectTransform;
            }
        }

        /// <summary>The projection camera. Resolves to <see cref="Camera.main"/> when unassigned.</summary>
        public Camera Camera {
            get {
                if (camera == null) camera = Camera.main;
                return camera;
            }
            set => camera = value;
        }

        /// <summary>The drawable rect (renderer rect inset by <see cref="Margin"/>) in local space.</summary>
        public Rect GetRect() {
            Rect rect = RectTransform.rect;
            rect.x += margin;
            rect.y += margin;
            rect.width -= margin * 2f;
            rect.height -= margin * 2f;
            return rect;
        }
    }
}
