using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using BeeneticToolkit.Spatial;
using BeeneticToolkit.Spatial.Pathfinding;

namespace BeeneticToolkit.Unity.Tests {

    // A [UnityTest] runs inside the play-mode runtime and can span frames. This exercises the
    // toolkit in the actual player loop (not just a plain unit-test context) and is the kind of
    // assembly that gets built into an IL2CPP player for Phase 3.
    public class PlayModeSmokeTests {

        [UnityTest]
        public IEnumerator AStar_RunsInPlayerLoop_AcrossAFrame() {
            var grid = new GridGraph(8, 8);
            PathResult<GridCoord> path =
                Pathfinder.AStar(grid, new GridCoord(0, 0), new GridCoord(7, 7), grid.Heuristic);

            Assert.IsTrue(path.Found);

            yield return null; // advance one frame in the play-mode runtime

            Assert.AreEqual(new GridCoord(7, 7), path.Nodes[path.Nodes.Count - 1]);
        }
    }
}
