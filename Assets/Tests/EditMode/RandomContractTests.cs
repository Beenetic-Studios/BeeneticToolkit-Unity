using NUnit.Framework;
using BeeneticToolkit.Random;
using BeeneticToolkit.Random.Noise;

namespace BeeneticToolkit.Unity.Tests {

    // Exercises BeeneticToolkit.Random's public API on the Unity runtime, plus its core
    // promise: bit-identical output for a given seed (validated harder cross-platform in Phase 2).
    public class RandomContractTests {

        [Test]
        public void SameSeed_ProducesIdenticalSequence() {
            var a = new RandomEnvironment("t", rootSeed: 12345).CreateAndRegister("main");
            var b = new RandomEnvironment("t", rootSeed: 12345).CreateAndRegister("main");
            for (int i = 0; i < 100; i++)
                Assert.AreEqual(a.NextInt(0, 1_000_000), b.NextInt(0, 1_000_000));
        }

        [Test]
        public void Environment_RecordsRootSeed_ForReplay() {
            var env = new RandomEnvironment("run"); // auto-seeded
            Assert.AreNotEqual(0L, env.RootSeed);
        }

        [Test]
        public void Scratch_ProducesValueInRange() {
            int n = RandomManager.Scratch.NextInt(1, 7); // [1, 7)
            Assert.That(n, Is.InRange(1, 6));
        }

        [Test]
        public void RandomChoice_ReturnsAMember() {
            var rng = new RandomEnvironment("c", rootSeed: 1).CreateAndRegister("g");
            var items = new[] { "a", "b", "c" };
            Assert.Contains(rng.RandomChoice(items), items);
        }

        [Test]
        public void PerlinNoise_StaysInRange_AndIsDeterministic() {
            var n1 = NoiseFactory.Create(NoiseAlgorithm.Perlin, seed: 1337);
            var n2 = NoiseFactory.Create(NoiseAlgorithm.Perlin, seed: 1337);
            for (int i = 0; i < 25; i++) {
                float v = n1.Sample(i * 0.1f, i * 0.2f);
                Assert.That(v, Is.InRange(-1f, 1f));
                Assert.AreEqual(v, n2.Sample(i * 0.1f, i * 0.2f), 1e-6f);
            }
        }
    }
}
