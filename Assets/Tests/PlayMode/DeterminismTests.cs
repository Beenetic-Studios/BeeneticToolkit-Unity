using NUnit.Framework;
using BeeneticToolkit.Random;
using BeeneticToolkit.Random.Noise;

namespace BeeneticToolkit.Unity.Tests {

    // Cross-runtime DETERMINISM. The golden vectors below were captured from the library running on
    // .NET (CoreCLR). These assert the *same* library, running in Unity, reproduces them — exactly
    // for integers (the headline "bit-identical across platforms" promise) and within a tight
    // tolerance for the float noise path. Today this runs under Mono (editor PlayMode); the same
    // assembly is built into an IL2CPP player in Phase 3, where this becomes the real proof.
    //
    // Regenerate with builds/goldgen in the main repo if the algorithms ever change.
    public class DeterminismTests {

        private const long RootSeed = 987654321L;
        private const string Key = "seq";
        private const int NoiseSeed = 24680;

        private static readonly long[] ExpectedLongs = {
            5803841072245425375L, 2612885134567182063L, 7040012640562637834L, 7063359576522863996L,
            7730686383382828116L, 4934455732476078226L, 8182900186394063484L, 4412293811193979575L
        };

        private static readonly int[] ExpectedInts = {
            425375, 182063, 637834, 863996, 828116, 78226, 63484, 979575
        };

        private static readonly float[] ExpectedPerlin = {
            0f, 0.052527912f, 0.014760753f, 0.20547642f, -0.10549445f, 0.19394572f
        };

        [Test]
        public void Xoshiro_NextLong_MatchesGoldenVector() {
            var rng = new RandomEnvironment("golden", rootSeed: RootSeed).CreateAndRegister(Key);
            for (int i = 0; i < ExpectedLongs.Length; i++)
                Assert.AreEqual(ExpectedLongs[i], rng.NextLong(), $"NextLong index {i}");
        }

        [Test]
        public void Xoshiro_NextIntBounded_MatchesGoldenVector() {
            var rng = new RandomEnvironment("golden", rootSeed: RootSeed).CreateAndRegister(Key);
            for (int i = 0; i < ExpectedInts.Length; i++)
                Assert.AreEqual(ExpectedInts[i], rng.NextInt(0, 1_000_000), $"NextInt index {i}");
        }

        [Test]
        public void PerlinNoise_MatchesGoldenVector() {
            var perlin = NoiseFactory.Create(NoiseAlgorithm.Perlin, seed: NoiseSeed);
            for (int i = 0; i < ExpectedPerlin.Length; i++)
                Assert.AreEqual(ExpectedPerlin[i], perlin.Sample(i * 0.37f, i * 0.91f), 1e-6f, $"Perlin index {i}");
        }
    }
}
