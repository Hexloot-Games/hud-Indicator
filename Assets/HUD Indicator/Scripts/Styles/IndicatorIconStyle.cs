using UnityEngine;

namespace HUDIndicator {

    /// <summary>
    /// Visual configuration for the icon of an indicator (the image drawn on the HUD).
    /// </summary>
    [System.Serializable]
    public class IndicatorIconStyle {

        [Tooltip("Texture drawn as the indicator icon. Any Texture (Texture2D, RenderTexture, ...) is supported.")]
        public Texture texture = null;

        [Tooltip("Tint applied to the icon texture.")]
        public Color color = Color.white;

        [Tooltip("Icon width, in canvas units (pixels for a Screen Space canvas).")]
        [Min(0f)] public float width = 48f;

        [Tooltip("Icon height, in canvas units (pixels for a Screen Space canvas).")]
        [Min(0f)] public float height = 48f;

        /// <summary>Icon size as a vector, in canvas units.</summary>
        public Vector2 Size => new Vector2(width, height);
    }
}
