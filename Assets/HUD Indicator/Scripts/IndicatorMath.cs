using System.Globalization;
using UnityEngine;

namespace HUDIndicator {

    /// <summary>
    /// Pure, side-effect-free helpers for indicator projection, edge clamping, scaling and
    /// label composition. Kept separate from the MonoBehaviour code so the tricky math can be
    /// unit tested without a running scene.
    /// </summary>
    public static class IndicatorMath {

        private const float Epsilon = 1e-6f;

        /// <summary>
        /// Maps a camera viewport point (0..1 on each axis) to a point in a RectTransform's local
        /// space, given that RectTransform's <see cref="Rect"/>.
        /// </summary>
        public static Vector2 ViewportToLocal(Vector2 viewport, Rect localRect) {
            return new Vector2(
                Mathf.LerpUnclamped(localRect.xMin, localRect.xMax, viewport.x),
                Mathf.LerpUnclamped(localRect.yMin, localRect.yMax, viewport.y));
        }

        /// <summary>
        /// Insets a rect on all sides. Used to keep an icon of the given size fully inside the
        /// drawable area (margin plus half the icon size). Never returns a negative size.
        /// </summary>
        public static Rect Inset(Rect rect, float margin, Vector2 iconSize) {
            float insetX = margin + iconSize.x * 0.5f;
            float insetY = margin + iconSize.y * 0.5f;

            float width = Mathf.Max(0f, rect.width - insetX * 2f);
            float height = Mathf.Max(0f, rect.height - insetY * 2f);

            // Keep the inset rect centred on the original rect even if it collapsed to zero size.
            float centerX = rect.center.x;
            float centerY = rect.center.y;
            return new Rect(centerX - width * 0.5f, centerY - height * 0.5f, width, height);
        }

        /// <summary>
        /// True when the target projects in front of the camera and inside <paramref name="bounds"/>.
        /// </summary>
        public static bool IsOnScreen(float viewportZ, Vector2 localPoint, Rect bounds) {
            return viewportZ > 0f && bounds.Contains(localPoint);
        }

        /// <summary>
        /// Projects <paramref name="localPoint"/> onto the border of <paramref name="bounds"/> along
        /// the direction from the rect centre. Used to pin off-screen indicators to the HUD edge.
        /// </summary>
        /// <param name="behindCamera">
        /// When true the target is behind the camera, so the projected point is mirrored and the
        /// direction is inverted to point at the correct edge.
        /// </param>
        /// <param name="direction">The (un-normalised) direction from the centre to the edge point.</param>
        public static Vector2 ClampToEdge(Vector2 localPoint, Rect bounds, bool behindCamera, out Vector2 direction) {
            Vector2 center = bounds.center;
            Vector2 dir = localPoint - center;
            if (behindCamera) dir = -dir;

            // Degenerate case (target exactly at the centre or directly behind): pick a stable default.
            if (dir.sqrMagnitude < Epsilon) dir = Vector2.up;

            float halfWidth = bounds.width * 0.5f;
            float halfHeight = bounds.height * 0.5f;

            float scaleX = halfWidth / Mathf.Max(Mathf.Abs(dir.x), Epsilon);
            float scaleY = halfHeight / Mathf.Max(Mathf.Abs(dir.y), Epsilon);
            float scale = Mathf.Min(scaleX, scaleY);

            direction = dir;
            return center + dir * scale;
        }

        /// <summary>
        /// Signed Z rotation (degrees) for an arrow whose neutral orientation points down, so that
        /// it points along <paramref name="direction"/>.
        /// </summary>
        public static float ArrowAngle(Vector2 direction) {
            if (direction.sqrMagnitude < Epsilon) return 0f;
            return Vector2.SignedAngle(Vector2.down, direction);
        }

        /// <summary>
        /// Distance-based uniform scale. Returns <paramref name="maxScale"/> at or below
        /// <paramref name="nearDistance"/> and <paramref name="minScale"/> at or beyond
        /// <paramref name="farDistance"/>, interpolating in between.
        /// </summary>
        public static float DistanceScale(float distance, float nearDistance, float farDistance, float minScale, float maxScale) {
            if (farDistance <= nearDistance) return maxScale;
            float t = Mathf.Clamp01(Mathf.InverseLerp(nearDistance, farDistance, distance));
            return Mathf.Lerp(maxScale, minScale, t);
        }

        /// <summary>
        /// Builds the label string from the indicator text and, optionally, the distance:
        /// <list type="bullet">
        /// <item>text + distance  -&gt; the text with the distance on the line below;</item>
        /// <item>distance only     -&gt; the distance as the single line;</item>
        /// <item>text only         -&gt; the text;</item>
        /// <item>neither           -&gt; an empty string (the label is hidden).</item>
        /// </list>
        /// </summary>
        public static string ComposeLabel(string text, bool showDistance, float distance, string distanceFormat) {
            bool hasText = !string.IsNullOrEmpty(text);

            if (showDistance) {
                string format = string.IsNullOrEmpty(distanceFormat) ? "0 m" : distanceFormat;
                string distanceText = FormatDistance(distance, format);
                return hasText ? text + "\n" + distanceText : distanceText;
            }

            return hasText ? text : string.Empty;
        }

        /// <summary>Formats a distance with the given numeric format using invariant culture.</summary>
        public static string FormatDistance(float distance, string format) {
            // Guard against malformed user format strings so a bad format never throws at runtime.
            try {
                return distance.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (System.FormatException) {
                return distance.ToString("0 m", CultureInfo.InvariantCulture);
            }
        }
    }
}
