using UnityEditor;

namespace HUDIndicator {

    /// <summary>
    /// Inspector for the unified <see cref="Indicator"/> component. Shows on-screen, off-screen and
    /// label sections with context-sensitive fields so only the relevant options are visible.
    /// </summary>
    [CustomEditor(typeof(Indicator))]
    [CanEditMultipleObjects]
    public class IndicatorEditor : Editor {

        private SerializedProperty _visible;
        private SerializedProperty _renderers;
        private SerializedProperty _icon;

        private SerializedProperty _showOnScreen;
        private SerializedProperty _scaleWithDistance;
        private SerializedProperty _nearDistance;
        private SerializedProperty _farDistance;
        private SerializedProperty _minScale;
        private SerializedProperty _maxScale;

        private SerializedProperty _showOffScreen;
        private SerializedProperty _showArrow;
        private SerializedProperty _arrowStyle;
        private SerializedProperty _rotateIcon;

        private SerializedProperty _label;
        private SerializedProperty _showDistance;
        private SerializedProperty _showTextOnScreen;
        private SerializedProperty _showTextOffScreen;
        private SerializedProperty _textStyle;

        private void OnEnable() {
            _visible = serializedObject.FindProperty("visible");
            _renderers = serializedObject.FindProperty("renderers");
            _icon = serializedObject.FindProperty("icon");

            _showOnScreen = serializedObject.FindProperty("showOnScreen");
            _scaleWithDistance = serializedObject.FindProperty("scaleWithDistance");
            _nearDistance = serializedObject.FindProperty("nearDistance");
            _farDistance = serializedObject.FindProperty("farDistance");
            _minScale = serializedObject.FindProperty("minScale");
            _maxScale = serializedObject.FindProperty("maxScale");

            _showOffScreen = serializedObject.FindProperty("showOffScreen");
            _showArrow = serializedObject.FindProperty("showArrow");
            _arrowStyle = serializedObject.FindProperty("arrowStyle");
            _rotateIcon = serializedObject.FindProperty("rotateIcon");

            _label = serializedObject.FindProperty("label");
            _showDistance = serializedObject.FindProperty("showDistance");
            _showTextOnScreen = serializedObject.FindProperty("showTextOnScreen");
            _showTextOffScreen = serializedObject.FindProperty("showTextOffScreen");
            _textStyle = serializedObject.FindProperty("textStyle");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_visible);
            EditorGUILayout.PropertyField(_renderers, true);
            EditorGUILayout.PropertyField(_icon, true);

            // On Screen (header is on the showOnScreen field).
            EditorGUILayout.PropertyField(_showOnScreen);
            if (ShouldExpand(_showOnScreen)) {
                using (new EditorGUI.IndentLevelScope()) {
                    EditorGUILayout.PropertyField(_scaleWithDistance);
                    if (ShouldExpand(_scaleWithDistance)) {
                        using (new EditorGUI.IndentLevelScope()) {
                            EditorGUILayout.PropertyField(_nearDistance);
                            EditorGUILayout.PropertyField(_farDistance);
                            EditorGUILayout.PropertyField(_minScale);
                            EditorGUILayout.PropertyField(_maxScale);
                        }
                    }
                }
            }

            // Off Screen (header is on the showOffScreen field).
            EditorGUILayout.PropertyField(_showOffScreen);
            if (ShouldExpand(_showOffScreen)) {
                using (new EditorGUI.IndentLevelScope()) {
                    EditorGUILayout.PropertyField(_showArrow);
                    if (ShouldExpand(_showArrow)) {
                        using (new EditorGUI.IndentLevelScope()) {
                            EditorGUILayout.PropertyField(_arrowStyle, true);
                        }
                    }
                    EditorGUILayout.PropertyField(_rotateIcon);
                }
            }

            // Label (header is on the label field).
            EditorGUILayout.PropertyField(_label);
            EditorGUILayout.PropertyField(_showDistance);
            EditorGUILayout.PropertyField(_showTextOnScreen);
            EditorGUILayout.PropertyField(_showTextOffScreen);
            EditorGUILayout.PropertyField(_textStyle, true);

            serializedObject.ApplyModifiedProperties();
        }

        private static bool ShouldExpand(SerializedProperty toggle) {
            return toggle.boolValue && !toggle.hasMultipleDifferentValues;
        }
    }
}
