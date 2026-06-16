using System.Collections.Generic;
using NUnit.Framework;
using BeeneticToolkit.Collections;
using BeeneticToolkit.Collections.Enums;

namespace BeeneticToolkit.Unity.Tests {

    // Exercises the data structures and — importantly — AutoEnumItem's reflection-based
    // self-registration. This passes under the editor's Mono runtime; the real test is a Phase 3
    // IL2CPP build, where managed-code stripping can remove reflectively-accessed members.
    public class CollectionsContractTests {

        [Test]
        public void RingBuffer_OverwritesOldestWhenFull() {
            var rb = new RingBuffer<int>(3);
            rb.Add(1); rb.Add(2); rb.Add(3); rb.Add(4); // 1 drops out
            Assert.AreEqual(3, rb.Count);
            Assert.AreEqual(2, rb[0]);            // 0 = oldest
            Assert.AreEqual(4, rb.PeekNewest());
        }

        [Test]
        public void PriorityQueue_DequeuesLowestPriorityFirst() {
            var pq = new PriorityQueue<string, float>();
            pq.Enqueue("boss", 60f);
            pq.Enqueue("tick", 1f);
            pq.Enqueue("wave", 12f);
            Assert.AreEqual("tick", pq.Peek());
        }

        [Test]
        public void LruCache_EvictsLeastRecentlyUsed() {
            var cache = new LruCache<string, int>(capacity: 2);
            cache.Set("x", 1);
            cache.Set("y", 2);
            cache.TryGet("x", out _);   // x is now most-recently-used
            cache.Set("z", 3);          // evicts y (the LRU entry)
            Assert.IsTrue(cache.ContainsKey("x"));
            Assert.IsFalse(cache.ContainsKey("y"));
            Assert.IsTrue(cache.ContainsKey("z"));
        }

        [Test]
        public void AutoEnumItem_SelfRegisters_AndLooksUpByKey() {
            Assert.AreEqual(3, Planet.Count);
            Assert.AreEqual(Planet.Earth, Planet.FromKey("earth"));
            Assert.AreEqual(Planet.Jupiter, Planet.FromName("Jupiter"));
            Assert.Contains(Planet.Mars, new List<Planet>(Planet.All));
        }
    }

    public enum PlanetGroup { None = 0, Rocky, GasGiant }

    // A self-registering smart enum used by the test above.
    public sealed class Planet : AutoEnumItem<Planet, string, PlanetGroup> {
        public static readonly Planet Earth   = new Planet("earth",   "Earth",   "E", PlanetGroup.Rocky);
        public static readonly Planet Mars    = new Planet("mars",    "Mars",    "M", PlanetGroup.Rocky);
        public static readonly Planet Jupiter = new Planet("jupiter", "Jupiter", "J", PlanetGroup.GasGiant);

        private Planet(string key, string name, string shortName, PlanetGroup group)
            : base(key, name, shortName, group: group) { }
    }
}
