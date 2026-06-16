using NUnit.Framework;
using BeeneticToolkit.Spatial;
using BeeneticToolkit.Spatial.Pathfinding;
using BeeneticToolkit.Spatial.Partitioning;

namespace BeeneticToolkit.Unity.Tests {

    // Exercises hex/grid coordinates, A* pathfinding, and partitioning on the Unity runtime.
    // (Generic value-type instantiations like Quadtree<T>/PriorityQueue are also AOT-relevant for Phase 3.)
    public class SpatialContractTests {

        [Test]
        public void Hex_DistanceTo_CountsSteps() {
            Assert.AreEqual(3, new Hex(0, 0).DistanceTo(new Hex(3, -1)));
        }

        [Test]
        public void GridCoord_ManhattanDistance() {
            Assert.AreEqual(7, new GridCoord(2, 3).ManhattanDistanceTo(new GridCoord(6, 0)));
        }

        [Test]
        public void AStar_FindsShortestPath_OnOpenGrid() {
            var grid = new GridGraph(5, 5);
            PathResult<GridCoord> path =
                Pathfinder.AStar(grid, new GridCoord(0, 0), new GridCoord(4, 0), grid.Heuristic);

            Assert.IsTrue(path.Found);
            Assert.AreEqual(5, path.Nodes.Count); // start + 4 steps along a straight row
        }

        [Test]
        public void Quadtree_RadiusQuery_ReturnsOnlyNearbyItems() {
            var tree = new Quadtree<int>(new Aabb(0f, 0f, 100f, 100f));
            tree.Insert((10f, 10f), 1);
            tree.Insert((90f, 90f), 2);

            var near = tree.QueryRadius((12f, 12f), 10f);

            Assert.AreEqual(1, near.Count);
            Assert.AreEqual(1, near[0]);
        }
    }
}
