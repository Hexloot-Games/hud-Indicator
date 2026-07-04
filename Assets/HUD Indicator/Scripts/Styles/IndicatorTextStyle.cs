using TMPro;
using UnityEngine;

namespace HUDIndicator {

    /// <summary>Which side of the icon the label prefers to sit on.</summary>
    public enum IndicatorTextPlacement {
        Right,
        Left,
        Above,
        Below,
        Center
    }

    /// <summary>
    /// Visual configuration for the optional text label of an indicator (rendered with TextMeshPro).
    /// The label shows the indicator's text, the distance to the target, or both. It hugs a side of
    /// the icon, stays vertically centred on it, and (when <see cref="autoFlip"/> is on) flips to the
    /// side facing the screen interior so it never overflows the edge.
    /// </summary>
    [System.Serializable]
    public class IndicatorTextStyle {

        [Tooltip("Font used by the label. Leave empty to use the project's default TextMeshPro font.")]
        public TMP_FontAsset font = null;

        [Tooltip("Font size of the label, in points.")]
        [Min(1f)] public float fontSize = 24f;

        [Tooltip("Color of the label text.")]
        public Color color = Color.white;

        [Tooltip("Font styling flags (bold, italic, ...).")]
        public FontStyles fontStyle = FontStyles.Normal;

        [Tooltip("Preferred side of the icon for the label. Used as the axis; the actual side may flip.")]
        public IndicatorTextPlacement placement = IndicatorTextPlacement.Right;

        [Tooltip("Flip the label towards the screen centre when the icon nears an edge, so text never " +
                 "runs off screen. Turn off to always keep the preferred side.")]
        public bool autoFlip = true;

        [Tooltip("Gap between the icon edge and the label, in canvas units.")]
        public float spacing = 8f;

        [Tooltip("Additional offset applied to the label position, in canvas units.")]
        public Vector2 offset = Vector2.zero;

        [Header("Distance")]
        [Tooltip("Numeric format used when the distance is displayed. Example: \"0 m\", \"0.0 units\", \"0\".")]
        public string distanceFormat = "0 m";

        [Tooltip("Multiplier applied to the world-space distance before formatting (e.g. to convert units).")]
        public float distanceScale = 1f;

        /// <summary>
        /// The side the label should actually use for an icon of <paramref name="iconSize"/> at
        /// <paramref name="iconLocal"/>, given the measured <paramref name="labelSize"/> and the
        /// drawable <paramref name="area"/>. When <see cref="autoFlip"/> is on, the preferred side
        /// flips to the opposite one only if it would push the label off the edge — so it never
        /// overflows, without snapping sides while there is room.
        /// </summary>
        public IndicatorTextPlacement ResolvePlacement(Vector2 iconLocal, Vector2 iconSize, Vector2 labelSize, Rect area) {
            if (!autoFlip) return placement;

            float halfW = iconSize.x * 0.5f;
            float halfH = iconSize.y * 0.5f;

            switch (placement) {
                case IndicatorTextPlacement.Right:
                    return iconLocal.x + halfW + spacing + labelSize.x > area.xMax
                        ? IndicatorTextPlacement.Left : IndicatorTextPlacement.Right;
                case IndicatorTextPlacement.Left:
                    return iconLocal.x - halfW - spacing - labelSize.x < area.xMin
                        ? IndicatorTextPlacement.Right : IndicatorTextPlacement.Left;
                case IndicatorTextPlacement.Above:
                    return iconLocal.y + halfH + spacing + labelSize.y > area.yMax
                        ? IndicatorTextPlacement.Below : IndicatorTextPlacement.Above;
                case IndicatorTextPlacement.Below:
                    return iconLocal.y - halfH - spacing - labelSize.y < area.yMin
                        ? IndicatorTextPlacement.Above : IndicatorTextPlacement.Below;
                default:
                    return IndicatorTextPlacement.Center;
            }
        }

        /// <summary>TextMeshPro alignment that makes the label hug the icon and stay centred on it.</summary>
        public static TextAlignmentOptions ResolveAlignment(IndicatorTextPlacement placement) {
            switch (placement) {
                case IndicatorTextPlacement.Right: return TextAlignmentOptions.Left;   // hug icon's right side
                case IndicatorTextPlacement.Left: return TextAlignmentOptions.Right;   // hug icon's left side
                case IndicatorTextPlacement.Above: return TextAlignmentOptions.Bottom;
                case IndicatorTextPlacement.Below: return TextAlignmentOptions.Top;
                default: return TextAlignmentOptions.Center;
            }
        }

        /// <summary>Pivot for the label rect so it grows away from the icon from the hugging edge.</summary>
        public static Vector2 ResolvePivot(IndicatorTextPlacement placement) {
            switch (placement) {
                case IndicatorTextPlacement.Right: return new Vector2(0f, 0.5f);
                case IndicatorTextPlacement.Left: return new Vector2(1f, 0.5f);
                case IndicatorTextPlacement.Above: return new Vector2(0.5f, 0f);
                case IndicatorTextPlacement.Below: return new Vector2(0.5f, 1f);
                default: return new Vector2(0.5f, 0.5f);
            }
        }

        /// <summary>Local offset of the label relative to the icon centre, given the icon size.</summary>
        public Vector2 ResolveLocalPosition(IndicatorTextPlacement placement, Vector2 iconSize) {
            float halfW = iconSize.x * 0.5f;
            float halfH = iconSize.y * 0.5f;
            Vector2 basePosition;
            switch (placement) {
                case IndicatorTextPlacement.Right: basePosition = new Vector2(halfW + spacing, 0f); break;
                case IndicatorTextPlacement.Left: basePosition = new Vector2(-(halfW + spacing), 0f); break;
                case IndicatorTextPlacement.Above: basePosition = new Vector2(0f, halfH + spacing); break;
                case IndicatorTextPlacement.Below: basePosition = new Vector2(0f, -(halfH + spacing)); break;
                default: basePosition = Vector2.zero; break;
            }

            // Mirror the manual offset on the axis that flipped, so it keeps pointing "outward".
            Vector2 tuned = offset;
            if (placement == IndicatorTextPlacement.Left) tuned.x = -tuned.x;
            if (placement == IndicatorTextPlacement.Above) tuned.y = -tuned.y;
            return basePosition + tuned;
        }
    }
}
