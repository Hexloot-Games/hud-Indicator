using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace HUDIndicator.Tests {

    /// <summary>
    /// Edit-mode tests for the public component API. Components are created without entering play
    /// mode, so no MonoBehaviour lifecycle runs and no views are spawned; only serialized state and
    /// the runtime API surface are exercised.
    /// </summary>
    public class IndicatorComponentTests {

        private readonly List<Object> _spawned = new List<Object>();

        private T New<T>() where T : Component {
            var go = new GameObject(typeof(T).Name);
            _spawned.Add(go);
            return go.AddComponent<T>();
        }

        [TearDown]
        public void TearDown() {
            foreach (Object obj in _spawned) {
                if (obj != null) Object.DestroyImmediate(obj);
            }
            _spawned.Clear();
        }

        // --- Label & toggles -------------------------------------------------------------------

        [Test]
        public void Label_DefaultsToEmpty() {
            Assert.AreEqual(string.Empty, New<Indicator>().Label);
        }

        [Test]
        public void SetLabel_NullStoresEmptyString() {
            var indicator = New<Indicator>();
            indicator.SetLabel(null);
            Assert.AreEqual(string.Empty, indicator.Label);
        }

        [Test]
        public void Toggles_RoundTrip() {
            var indicator = New<Indicator>();
            indicator.Visible = false;
            indicator.ShowOnScreen = false;
            indicator.ShowOffScreen = false;
            indicator.ShowDistance = true;
            indicator.ShowTextOnScreen = false;
            indicator.ShowTextOffScreen = true;

            Assert.IsFalse(indicator.Visible);
            Assert.IsFalse(indicator.ShowOnScreen);
            Assert.IsFalse(indicator.ShowOffScreen);
            Assert.IsTrue(indicator.ShowDistance);
            Assert.IsFalse(indicator.ShowTextOnScreen);
            Assert.IsTrue(indicator.ShowTextOffScreen);
        }

        [Test]
        public void Styles_AreNeverNull() {
            var indicator = New<Indicator>();
            Assert.IsNotNull(indicator.Icon);
            Assert.IsNotNull(indicator.ArrowStyle);
            Assert.IsNotNull(indicator.TextStyle);
        }

        [Test]
        public void ArrowDefaultsOn() {
            Assert.IsTrue(New<Indicator>().ShowArrow);
        }

        // --- Renderer management ---------------------------------------------------------------

        [Test]
        public void SetRenderer_ReplacesRendererList() {
            var indicator = New<Indicator>();
            var renderer = New<IndicatorRenderer>();
            indicator.SetRenderer(renderer);
            CollectionAssert.AreEqual(new[] { renderer }, indicator.GetRenderers());
        }

        [Test]
        public void SetRenderer_Null_ClearsRendererList() {
            var indicator = New<Indicator>();
            indicator.SetRenderer(New<IndicatorRenderer>());
            indicator.SetRenderer(null);
            Assert.AreEqual(0, indicator.GetRenderers().Count);
        }

        [Test]
        public void AddRenderer_IsDeduplicated() {
            var indicator = New<Indicator>();
            var renderer = New<IndicatorRenderer>();
            indicator.AddRenderer(renderer);
            indicator.AddRenderer(renderer);
            Assert.AreEqual(1, indicator.GetRenderers().Count);
        }

        [Test]
        public void RemoveRenderer_RemovesFromList() {
            var indicator = New<Indicator>();
            var renderer = New<IndicatorRenderer>();
            indicator.AddRenderer(renderer);
            indicator.RemoveRenderer(renderer);
            Assert.AreEqual(0, indicator.GetRenderers().Count);
        }

        // --- Distance scaling clamps -----------------------------------------------------------

        [Test]
        public void ScaleSetters_ClampNegativeToZero() {
            var indicator = New<Indicator>();
            indicator.MinScale = -5f;
            indicator.MaxScale = -1f;
            indicator.NearDistance = -3f;
            indicator.FarDistance = -2f;
            Assert.AreEqual(0f, indicator.MinScale, 1e-6f);
            Assert.AreEqual(0f, indicator.MaxScale, 1e-6f);
            Assert.AreEqual(0f, indicator.NearDistance, 1e-6f);
            Assert.AreEqual(0f, indicator.FarDistance, 1e-6f);
        }

        // --- IndicatorRenderer -----------------------------------------------------------------

        [Test]
        public void Renderer_MarginClampsNegativeToZero() {
            var renderer = New<IndicatorRenderer>();
            renderer.Margin = -50f;
            Assert.AreEqual(0f, renderer.Margin, 1e-6f);
        }

        [Test]
        public void Renderer_GizmoColorRoundTrips() {
            var renderer = New<IndicatorRenderer>();
            renderer.GizmoColor = Color.red;
            Assert.AreEqual(Color.red, renderer.GizmoColor);
        }

        [Test]
        public void Renderer_GetRect_AppliesMarginSymmetrically() {
            var renderer = New<IndicatorRenderer>();
            var rect = renderer.RectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(200f, 200f);

            renderer.Margin = 20f;
            Rect result = renderer.GetRect();

            Assert.AreEqual(-80f, result.xMin, 1e-3f);
            Assert.AreEqual(-80f, result.yMin, 1e-3f);
            Assert.AreEqual(160f, result.width, 1e-3f);
            Assert.AreEqual(160f, result.height, 1e-3f);
        }
    }
}
