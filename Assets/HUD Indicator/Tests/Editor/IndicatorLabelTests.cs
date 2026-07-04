using NUnit.Framework;

namespace HUDIndicator.Tests {

    /// <summary>Tests for the label composition rules (text + optional distance).</summary>
    public class IndicatorLabelTests {

        [Test]
        public void ComposeLabel_TextAndDistance_PutsDistanceOnLineBelow() {
            string result = IndicatorMath.ComposeLabel("Enemy", true, 12f, "0 m");
            Assert.AreEqual("Enemy\n12 m", result);
        }

        [Test]
        public void ComposeLabel_DistanceOnly_ReturnsDistanceAsSingleLine() {
            string result = IndicatorMath.ComposeLabel("", true, 12f, "0 m");
            Assert.AreEqual("12 m", result);
            Assert.IsFalse(result.Contains("\n"));
        }

        [Test]
        public void ComposeLabel_TextOnly_ReturnsText() {
            Assert.AreEqual("Enemy", IndicatorMath.ComposeLabel("Enemy", false, 12f, "0 m"));
        }

        [Test]
        public void ComposeLabel_Neither_ReturnsEmpty() {
            Assert.AreEqual(string.Empty, IndicatorMath.ComposeLabel("", false, 12f, "0 m"));
        }

        [Test]
        public void ComposeLabel_NullText_TreatedAsEmpty() {
            Assert.AreEqual("12 m", IndicatorMath.ComposeLabel(null, true, 12f, "0 m"));
        }

        [Test]
        public void FormatDistance_UsesInvariantCulture() {
            // Invariant culture uses '.' as the decimal separator regardless of the machine locale.
            Assert.AreEqual("12.5 m", IndicatorMath.FormatDistance(12.5f, "0.0 m"));
        }

        [Test]
        public void FormatDistance_MalformedFormat_FallsBackInsteadOfThrowing() {
            // A stray '{' is an invalid composite/numeric format; must not throw.
            Assert.DoesNotThrow(() => IndicatorMath.FormatDistance(10f, "{0:bad"));
        }

        [Test]
        public void ComposeLabel_EmptyFormat_DefaultsToMeters() {
            Assert.AreEqual("7 m", IndicatorMath.ComposeLabel("", true, 7f, ""));
        }
    }
}
