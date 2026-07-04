using NUnit.Framework;
using UnityEngine;

namespace HUDIndicator.Tests {

    /// <summary>Tests that the label flips to the opposite side only when it would overflow.</summary>
    public class IndicatorTextFlipTests {

        // Drawable area 1000 x 600 centred on the origin.
        private static readonly Rect Area = new Rect(-500f, -300f, 1000f, 600f);
        private static readonly Vector2 IconSize = new Vector2(48f, 48f);
        private static readonly Vector2 LabelSize = new Vector2(200f, 30f);

        private static IndicatorTextStyle Style(IndicatorTextPlacement placement, bool autoFlip = true) {
            return new IndicatorTextStyle { placement = placement, autoFlip = autoFlip, spacing = 8f };
        }

        [Test]
        public void AutoFlipOff_KeepsPreferredSide() {
            var style = Style(IndicatorTextPlacement.Right, autoFlip: false);
            // Even jammed against the right edge, it must not flip.
            var result = style.ResolvePlacement(new Vector2(490f, 0f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Right, result);
        }

        [Test]
        public void Right_StaysRight_WhenItFits() {
            var style = Style(IndicatorTextPlacement.Right);
            var result = style.ResolvePlacement(Vector2.zero, IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Right, result);
        }

        [Test]
        public void Right_FlipsLeft_NearRightEdge() {
            var style = Style(IndicatorTextPlacement.Right);
            var result = style.ResolvePlacement(new Vector2(400f, 0f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Left, result);
        }

        [Test]
        public void Left_FlipsRight_NearLeftEdge() {
            var style = Style(IndicatorTextPlacement.Left);
            var result = style.ResolvePlacement(new Vector2(-400f, 0f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Right, result);
        }

        [Test]
        public void Above_FlipsBelow_NearTopEdge() {
            var style = Style(IndicatorTextPlacement.Above);
            var result = style.ResolvePlacement(new Vector2(0f, 250f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Below, result);
        }

        [Test]
        public void Below_FlipsAbove_NearBottomEdge() {
            var style = Style(IndicatorTextPlacement.Below);
            var result = style.ResolvePlacement(new Vector2(0f, -250f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Above, result);
        }

        [Test]
        public void Center_NeverFlips() {
            var style = Style(IndicatorTextPlacement.Center);
            var result = style.ResolvePlacement(new Vector2(490f, 290f), IconSize, LabelSize, Area);
            Assert.AreEqual(IndicatorTextPlacement.Center, result);
        }
    }
}
