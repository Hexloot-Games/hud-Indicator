using UnityEngine;

namespace HUDIndicator {

    /// <summary>
    /// Visual configuration for the directional arrow drawn next to an off-screen indicator.
    /// The arrow orbits the icon and points from the screen center towards the tracked target.
    /// </summary>
    [System.Serializable]
    public class IndicatorArrowStyle {

        [Tooltip("Texture drawn as the arrow. It should point downwards in its neutral orientation.")]
        public Texture texture = null;

        [Tooltip("Tint applied to the arrow texture.")]
        public Color color = Color.white;

        [Tooltip("Extra distance between the icon and the arrow. Negative values overlap the arrow with the icon.")]
        public float margin = 0f;

        [Tooltip("Arrow width, in canvas units.")]
        [Min(0f)] public float width = 24f;

        [Tooltip("Arrow height, in canvas units.")]
        [Min(0f)] public float height = 24f;

        /// <summary>Arrow size as a vector, in canvas units.</summary>
        public Vector2 Size => new Vector2(width, height);
    }
}
