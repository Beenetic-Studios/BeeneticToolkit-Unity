using NUnit.Framework;
using BeeneticToolkit.Numerics;

namespace BeeneticToolkit.Unity.Tests {

    // Exercises MathKit / EasingUtils on the Unity runtime. (Uses typed float literals so the
    // float/double/decimal overloads resolve unambiguously.)
    public class NumericsContractTests {

        [Test]
        public void Lerp_IsClamped() {
            Assert.AreEqual(50f, MathKit.Lerp(0f, 100f, 0.5f), 1e-4f);
            Assert.AreEqual(100f, MathKit.Lerp(0f, 100f, 2f), 1e-4f); // t clamped to 1
        }

        [Test]
        public void Remap_MapsBetweenRanges() {
            Assert.AreEqual(50f, MathKit.Remap(512f, 0f, 1024f, 0f, 100f), 1e-2f);
        }

        [Test]
        public void WrapDegrees_WrapsIntoRange() {
            Assert.AreEqual(10f, MathKit.WrapDegrees(370f), 1e-3f);
        }

        [Test]
        public void DeltaAngleDegrees_TakesShortestSignedTurn() {
            Assert.AreEqual(20f, MathKit.DeltaAngleDegrees(350f, 10f), 1e-3f);
        }

        [Test]
        public void EasingUtils_OutCubic_HitsEndpoints() {
            Assert.AreEqual(0f, EasingUtils.OutCubic(0f), 1e-5f);
            Assert.AreEqual(1f, EasingUtils.OutCubic(1f), 1e-5f);
        }
    }
}
