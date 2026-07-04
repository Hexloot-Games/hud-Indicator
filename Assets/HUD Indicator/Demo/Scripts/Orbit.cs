using UnityEngine;

namespace HUDIndicator.Demo {

    /// <summary>Demo helper: moves the object in a horizontal circle around a world point.</summary>
    public class Orbit : MonoBehaviour {

        public Vector3 center = Vector3.zero;
        public float radius = 10f;
        public float height = 1f;
        public float degreesPerSecond = 25f;
        public float startAngle = 0f;

        private void Update() {
            float angle = (startAngle + degreesPerSecond * Time.time) * Mathf.Deg2Rad;
            transform.position = center + new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius);
        }
    }
}
