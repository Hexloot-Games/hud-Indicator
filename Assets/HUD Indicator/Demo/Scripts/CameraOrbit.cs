using UnityEngine;

namespace HUDIndicator.Demo {

    /// <summary>
    /// Demo helper: slowly orbits the camera around a focus point so targets sweep on and off screen,
    /// showing off both the on-screen icons and the off-screen edge arrows.
    /// </summary>
    public class CameraOrbit : MonoBehaviour {

        public Vector3 focus = Vector3.zero;
        public float degreesPerSecond = 12f;

        private void Update() {
            transform.RotateAround(focus, Vector3.up, degreesPerSecond * Time.deltaTime);
            transform.LookAt(focus);
        }
    }
}
