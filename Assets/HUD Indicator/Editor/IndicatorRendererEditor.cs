using UnityEditor;
using UnityEngine;

namespace HUDIndicator {

    [CustomEditor(typeof(IndicatorRenderer))]
    [CanEditMultipleObjects]
    public class IndicatorRendererEditor : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            foreach (Object obj in targets) {
                var renderer = (IndicatorRenderer)obj;
                if (renderer.GetComponentInParent<Canvas>(true) == null) {
                    EditorGUILayout.HelpBox(
                        "This Indicator Renderer is not under a Canvas. Place it on a RectTransform " +
                        "inside a Canvas so indicators can be drawn.", MessageType.Warning);
                    break;
                }
            }

            if (Camera.main == null) {
                bool anyMissingCamera = false;
                foreach (Object obj in targets) {
                    var so = new SerializedObject(obj);
                    if (so.FindProperty("camera").objectReferenceValue == null) {
                        anyMissingCamera = true;
                        break;
                    }
                }
                if (anyMissingCamera) {
                    EditorGUILayout.HelpBox(
                        "No Camera is assigned and there is no Camera tagged 'MainCamera'. " +
                        "Assign a camera so world targets can be projected.", MessageType.Info);
                }
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        private static void DrawRendererGizmo(IndicatorRenderer renderer, GizmoType gizmoType) {
            RectTransform rect = renderer.RectTransform;
            if (rect == null) return;

            Rect area = renderer.GetRect();
            if (area.width <= 0f || area.height <= 0f) return;

            Vector3 bottomLeft = rect.TransformPoint(new Vector3(area.xMin, area.yMin, 0f));
            Vector3 topLeft = rect.TransformPoint(new Vector3(area.xMin, area.yMax, 0f));
            Vector3 topRight = rect.TransformPoint(new Vector3(area.xMax, area.yMax, 0f));
            Vector3 bottomRight = rect.TransformPoint(new Vector3(area.xMax, area.yMin, 0f));

            bool selected = (gizmoType & (GizmoType.Selected | GizmoType.InSelectionHierarchy)) != 0;

            Color baseColor = renderer.GizmoColor;
            Color face = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * (selected ? 1f : 0.4f));
            Color outline = new Color(baseColor.r, baseColor.g, baseColor.b, selected ? 1f : 0.5f);

            Handles.DrawSolidRectangleWithOutline(
                new[] { bottomLeft, topLeft, topRight, bottomRight }, face, outline);
        }
    }
}
