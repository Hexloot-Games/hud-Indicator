using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HUDIndicator {

    /// <summary>
    /// Runtime visual for a single (<see cref="Indicator"/>, <see cref="IndicatorRenderer"/>) pair.
    /// Owns the UI hierarchy (icon + optional arrow + optional label) and, every frame, decides
    /// whether the target is on or off screen and draws the appropriate representation. This is a
    /// plain class, not a MonoBehaviour: the indicator drives its whole life cycle explicitly.
    /// </summary>
    public class IndicatorView {

        private Indicator _indicator;
        private IndicatorRenderer _renderer;

        private GameObject _root;
        private RectTransform _rootRect;

        private RectTransform _iconRect;
        private RawImage _iconImage;

        private GameObject _arrowObject;
        private RectTransform _arrowRect;
        private RawImage _arrowImage;

        private GameObject _labelObject;
        private RectTransform _labelRect;
        private TextMeshProUGUI _label;

        private bool _created;
        private bool _rootActive;

        public void Create(Indicator indicator, IndicatorRenderer renderer) {
            _indicator = indicator;
            _renderer = renderer;

            _root = new GameObject($"Indicator:{indicator.name}");
            _rootRect = _root.AddComponent<RectTransform>();
            ConfigureRect(_rootRect);
            _rootRect.SetParent(renderer.RectTransform, false);

            var iconObject = new GameObject("Icon");
            _iconRect = iconObject.AddComponent<RectTransform>();
            ConfigureRect(_iconRect);
            _iconRect.SetParent(_rootRect, false);
            _iconImage = iconObject.AddComponent<RawImage>();
            _iconImage.raycastTarget = false;

            _arrowObject = new GameObject("Arrow");
            _arrowRect = _arrowObject.AddComponent<RectTransform>();
            ConfigureRect(_arrowRect);
            _arrowRect.SetParent(_rootRect, false);
            _arrowImage = _arrowObject.AddComponent<RawImage>();
            _arrowImage.raycastTarget = false;

            _created = true;

            // A new GameObject is active by default; hide it until the first Tick positions it.
            _rootActive = true;
            SetRootActive(false);
        }

        /// <summary>Advances the view one frame. Called by the indicator from LateUpdate.</summary>
        public void Tick() {
            if (!_created) return;

            if (_renderer == null || _renderer.Camera == null || !IsVisible()) {
                SetRootActive(false);
                return;
            }

            UpdateIconStyle();

            Camera camera = _renderer.Camera;
            Vector3 viewport = camera.WorldToViewportPoint(_indicator.transform.position);
            bool behindCamera = viewport.z <= 0f;

            Rect rendererRect = _renderer.RectTransform.rect;
            Vector2 local = IndicatorMath.ViewportToLocal(viewport, rendererRect);

            // Distance-based scale only affects the on-screen icon.
            float scale = 1f;
            if (_indicator.ScaleWithDistance) {
                scale = IndicatorMath.DistanceScale(
                    GetCameraDistance(), _indicator.NearDistance, _indicator.FarDistance,
                    _indicator.MinScale, _indicator.MaxScale);
            }

            Vector2 onScreenIconSize = _iconRect.sizeDelta * scale;
            Rect onScreenBounds = IndicatorMath.Inset(rendererRect, _renderer.Margin, onScreenIconSize);
            bool onScreen = IndicatorMath.IsOnScreen(viewport.z, local, onScreenBounds);

            if (onScreen) {
                DrawOnScreen(local, scale, rendererRect);
            }
            else {
                DrawOffScreen(local, rendererRect, behindCamera);
            }
        }

        public void Hide() => SetRootActive(false);

        public void Destroy() {
            if (_root != null) {
                Object.Destroy(_root);
                _root = null;
            }
            _created = false;
        }

        // --- State drawing ----------------------------------------------------------------------

        private void DrawOnScreen(Vector2 local, float scale, Rect rendererRect) {
            if (!_indicator.ShowOnScreen) {
                SetRootActive(false);
                return;
            }

            SetRootActive(true);
            SetArrowActive(false);

            _iconRect.localScale = new Vector3(scale, scale, 1f);
            _iconRect.localRotation = Quaternion.identity;
            _rootRect.localPosition = new Vector3(local.x, local.y, 0f);

            UpdateLabel(_indicator.ShowTextOnScreen, _iconRect.sizeDelta * scale, local, rendererRect);
        }

        private void DrawOffScreen(Vector2 local, Rect rendererRect, bool behindCamera) {
            if (!_indicator.ShowOffScreen) {
                SetRootActive(false);
                return;
            }

            SetRootActive(true);

            Vector2 iconSize = _iconRect.sizeDelta;
            _iconRect.localScale = Vector3.one;

            Rect bounds = IndicatorMath.Inset(rendererRect, _renderer.Margin, iconSize);
            Vector2 edge = IndicatorMath.ClampToEdge(local, bounds, behindCamera, out Vector2 direction);
            _rootRect.localPosition = new Vector3(edge.x, edge.y, 0f);

            float angle = IndicatorMath.ArrowAngle(direction);
            UpdateArrow(iconSize, angle);
            _iconRect.localRotation = _indicator.RotateIcon
                ? Quaternion.Euler(0f, 0f, angle)
                : Quaternion.identity;

            UpdateLabel(_indicator.ShowTextOffScreen, iconSize, edge, rendererRect);
        }

        // --- Icon / arrow -----------------------------------------------------------------------

        private void UpdateIconStyle() {
            IndicatorIconStyle style = _indicator.Icon;
            Vector2 size = style?.Size ?? Vector2.zero;

            _iconRect.sizeDelta = size;
            _iconImage.texture = style?.texture;
            _iconImage.color = style?.color ?? Color.white;
            // Hide the quad when no texture is assigned so it does not draw a white box.
            _iconImage.enabled = style != null && style.texture != null;
        }

        private void UpdateArrow(Vector2 iconSize, float angle) {
            if (!_indicator.ShowArrow) {
                SetArrowActive(false);
                return;
            }

            SetArrowActive(true);

            IndicatorArrowStyle style = _indicator.ArrowStyle;
            _arrowRect.sizeDelta = style.Size;

            // Offset the pivot beyond the icon so the arrow orbits the icon centre as it rotates.
            float safeHeight = Mathf.Max(style.height, 0.0001f);
            float pivotY = 1f + ((iconSize.y + iconSize.x) * 0.25f + style.margin) / safeHeight;
            _arrowRect.pivot = new Vector2(0.5f, pivotY);
            _arrowRect.localRotation = Quaternion.Euler(0f, 0f, angle);

            _arrowImage.texture = style.texture;
            _arrowImage.color = style.color;
            _arrowImage.enabled = style.texture != null;
        }

        // --- Label ------------------------------------------------------------------------------

        private void UpdateLabel(bool show, Vector2 iconSize, Vector2 iconLocal, Rect area) {
            if (!show) {
                if (_label != null && _labelObject.activeSelf) _labelObject.SetActive(false);
                return;
            }

            float distance = _indicator.ShowDistance ? GetCameraDistance() : 0f;
            string content = IndicatorMath.ComposeLabel(
                _indicator.Label, _indicator.ShowDistance, distance, _indicator.TextStyle.distanceFormat);

            if (string.IsNullOrEmpty(content)) {
                if (_label != null && _labelObject.activeSelf) _labelObject.SetActive(false);
                return;
            }

            EnsureLabel();
            if (!_labelObject.activeSelf) _labelObject.SetActive(true);

            IndicatorTextStyle style = _indicator.TextStyle;
            if (style.font != null && _label.font != style.font) _label.font = style.font;
            _label.fontSize = style.fontSize;
            _label.color = style.color;
            _label.fontStyle = style.fontStyle;
            _label.text = content;

            // Flip the label to the opposite side only if the preferred side would run off screen.
            Vector2 labelSize = _label.GetPreferredValues();
            IndicatorTextPlacement placement = style.ResolvePlacement(iconLocal, iconSize, labelSize, area);
            _label.alignment = IndicatorTextStyle.ResolveAlignment(placement);
            _labelRect.pivot = IndicatorTextStyle.ResolvePivot(placement);
            _labelRect.localPosition = style.ResolveLocalPosition(placement, iconSize);
        }

        private void EnsureLabel() {
            if (_label != null) return;

            _labelObject = new GameObject("Label");
            _labelRect = _labelObject.AddComponent<RectTransform>();
            ConfigureRect(_labelRect);
            _labelRect.SetParent(_rootRect, false);
            _labelRect.sizeDelta = new Vector2(512f, 128f);

            _label = _labelObject.AddComponent<TextMeshProUGUI>();
            _label.raycastTarget = false;
            _label.textWrappingMode = TextWrappingModes.NoWrap;
            _label.overflowMode = TextOverflowModes.Overflow;
        }

        // --- Helpers ----------------------------------------------------------------------------

        private bool IsVisible() => _indicator.Visible && _renderer.Visible;

        private float GetCameraDistance() {
            Camera camera = _renderer.Camera;
            if (camera == null) return 0f;
            float distance = Vector3.Distance(camera.transform.position, _indicator.transform.position);
            return distance * _indicator.TextStyle.distanceScale;
        }

        private void SetRootActive(bool value) {
            if (_rootActive == value) return;
            _rootActive = value;
            if (_root != null) _root.SetActive(value);
        }

        private void SetArrowActive(bool value) {
            if (_arrowObject != null && _arrowObject.activeSelf != value) _arrowObject.SetActive(value);
        }

        private static void ConfigureRect(RectTransform rect) {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
