using NUnit.Framework;
using UnityEngine;

namespace HUDIndicator.Tests {

    /// <summary>Unit tests for the pure projection / clamping / label math.</summary>
    public class IndicatorMathTests {

        private static readonly Rect UnitRect = new Rect(-100f, -100f, 200f, 200f); // centred on origin

        // --- ViewportToLocal -------------------------------------------------------------------

        [Test]
        public void ViewportToLocal_Center_MapsToRectCenter() {
            Vector2 result = IndicatorMath.ViewportToLocal(new Vector2(0.5f, 0.5f), UnitRect);
            Assert.AreEqual(0f, result.x, 1e-4f);
            Assert.AreEqual(0f, result.y, 1e-4f);
        }

        [Test]
        public void ViewportToLocal_Corners_MapToRectCorners() {
            Vector2 min = IndicatorMath.ViewportToLocal(new Vector2(0f, 0f), UnitRect);
            Vector2 max = IndicatorMath.ViewportToLocal(new Vector2(1f, 1f), UnitRect);
            Assert.AreEqual(-100f, min.x, 1e-4f);
            Assert.AreEqual(-100f, min.y, 1e-4f);
            Assert.AreEqual(100f, max.x, 1e-4f);
            Assert.AreEqual(100f, max.y, 1e-4f);
        }

        // --- Inset -----------------------------------------------------------------------------

        [Test]
        public void Inset_ShrinksByMarginPlusHalfIcon_AndStaysCentered() {
            Rect inset = IndicatorMath.Inset(UnitRect, 10f, new Vector2(40f, 40f));
            // Inset per side = 10 + 20 = 30 -> width/height reduced by 60.
            Assert.AreEqual(140f, inset.width, 1e-4f);
            Assert.AreEqual(140f, inset.height, 1e-4f);
            Assert.AreEqual(0f, inset.center.x, 1e-4f);
            Assert.AreEqual(0f, inset.center.y, 1e-4f);
        }

        [Test]
        public void Inset_NeverProducesNegativeSize() {
            Rect inset = IndicatorMath.Inset(UnitRect, 1000f, new Vector2(1000f, 1000f));
            Assert.GreaterOrEqual(inset.width, 0f);
            Assert.GreaterOrEqual(inset.height, 0f);
        }

        // --- IsOnScreen ------------------------------------------------------------------------

        [Test]
        public void IsOnScreen_InsideAndInFront_IsTrue() {
            Assert.IsTrue(IndicatorMath.IsOnScreen(1f, Vector2.zero, UnitRect));
        }

        [Test]
        public void IsOnScreen_BehindCamera_IsFalse() {
            Assert.IsFalse(IndicatorMath.IsOnScreen(-1f, Vector2.zero, UnitRect));
        }

        [Test]
        public void IsOnScreen_OutsideBounds_IsFalse() {
            Assert.IsFalse(IndicatorMath.IsOnScreen(1f, new Vector2(500f, 0f), UnitRect));
        }

        // --- ClampToEdge -----------------------------------------------------------------------

        [Test]
        public void ClampToEdge_RightOfCenter_LandsOnRightEdge() {
            Vector2 edge = IndicatorMath.ClampToEdge(new Vector2(500f, 0f), UnitRect, false, out Vector2 dir);
            Assert.AreEqual(100f, edge.x, 1e-3f);
            Assert.AreEqual(0f, edge.y, 1e-3f);
            Assert.Greater(dir.x, 0f);
        }

        [Test]
        public void ClampToEdge_ResultAlwaysOnBoundary() {
            Vector2 edge = IndicatorMath.ClampToEdge(new Vector2(70f, 500f), UnitRect, false, out _);
            // The point must lie on the border of the rect (one axis hits the half-extent).
            bool onVertical = Mathf.Abs(Mathf.Abs(edge.x) - 100f) < 1e-3f;
            bool onHorizontal = Mathf.Abs(Mathf.Abs(edge.y) - 100f) < 1e-3f;
            Assert.IsTrue(onVertical || onHorizontal);
            Assert.LessOrEqual(Mathf.Abs(edge.x), 100f + 1e-3f);
            Assert.LessOrEqual(Mathf.Abs(edge.y), 100f + 1e-3f);
        }

        [Test]
        public void ClampToEdge_BehindCamera_InvertsDirection() {
            // Target projects to the right, but is behind the camera -> should clamp to the LEFT edge.
            Vector2 edge = IndicatorMath.ClampToEdge(new Vector2(500f, 0f), UnitRect, true, out Vector2 dir);
            Assert.AreEqual(-100f, edge.x, 1e-3f);
            Assert.Less(dir.x, 0f);
        }

        [Test]
        public void ClampToEdge_AtCenter_UsesStableFallback() {
            Vector2 edge = IndicatorMath.ClampToEdge(Vector2.zero, UnitRect, false, out Vector2 dir);
            Assert.AreNotEqual(Vector2.zero, dir);
            Assert.LessOrEqual(Mathf.Abs(edge.x), 100f + 1e-3f);
            Assert.LessOrEqual(Mathf.Abs(edge.y), 100f + 1e-3f);
        }

        // --- ArrowAngle ------------------------------------------------------------------------

        [Test]
        public void ArrowAngle_Down_IsZero() {
            Assert.AreEqual(0f, IndicatorMath.ArrowAngle(Vector2.down), 1e-3f);
        }

        [Test]
        public void ArrowAngle_Up_IsHalfTurn() {
            Assert.AreEqual(180f, Mathf.Abs(IndicatorMath.ArrowAngle(Vector2.up)), 1e-3f);
        }

        [Test]
        public void ArrowAngle_ZeroVector_IsZero() {
            Assert.AreEqual(0f, IndicatorMath.ArrowAngle(Vector2.zero), 1e-3f);
        }

        // --- DistanceScale ---------------------------------------------------------------------

        [Test]
        public void DistanceScale_AtNear_ReturnsMax() {
            Assert.AreEqual(1f, IndicatorMath.DistanceScale(5f, 5f, 50f, 0.5f, 1f), 1e-4f);
        }

        [Test]
        public void DistanceScale_AtFar_ReturnsMin() {
            Assert.AreEqual(0.5f, IndicatorMath.DistanceScale(50f, 5f, 50f, 0.5f, 1f), 1e-4f);
        }

        [Test]
        public void DistanceScale_BeyondFar_IsClampedToMin() {
            Assert.AreEqual(0.5f, IndicatorMath.DistanceScale(9999f, 5f, 50f, 0.5f, 1f), 1e-4f);
        }

        [Test]
        public void DistanceScale_DegenerateRange_ReturnsMax() {
            Assert.AreEqual(1f, IndicatorMath.DistanceScale(20f, 50f, 50f, 0.5f, 1f), 1e-4f);
        }
    }
}
