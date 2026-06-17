using System.Collections.Generic;
using BeeneticToolkit.Spatial;
using BeeneticToolkit.Spatial.Pathfinding;
using UnityEngine;

namespace BeeneticToolkit.Unity.Playground {

    /// <summary>
    /// Draws a square grid with blocked cells, the A* path from start to goal, and (optionally) the flow field
    /// toward the goal — all from BeeneticToolkit.Spatial.Pathfinding. Attach to an empty GameObject and view in
    /// the Scene window (2D toggle). Edit the blocked list and start/goal in the Inspector to watch the path react.
    /// </summary>
    public class GridPathfindingVisualizer : MonoBehaviour {

        [Header("Grid")]
        public int width = 12;
        public int height = 8;
        public bool eightWay = false;

        [Header("Endpoints (cell coordinates)")]
        public Vector2Int start = new Vector2Int(0, 0);
        public Vector2Int goal = new Vector2Int(11, 7);

        [Header("Obstacles")]
        public List<Vector2Int> blocked = new List<Vector2Int>();

        [Header("Display")]
        public bool showFlowField = false;

        private void OnDrawGizmos() {
            Gizmos.matrix = Matrix4x4.Translate(transform.position); // lets several visualizers sit apart in one scene
            var blockedSet = new HashSet<Vector2Int>(blocked);
            bool Passable(GridCoord c) => !blockedSet.Contains(new Vector2Int(c.X, c.Y));

            var graph = new GridGraph(
                width, height,
                eightWay ? GridConnectivity.EightWay : GridConnectivity.FourWay,
                Passable);

            // Cells: blocked are solid red, walkable are faint wire.
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var c = new GridCoord(x, y);
                    if (graph.IsWalkable(c)) {
                        Gizmos.color = new Color(1f, 1f, 1f, 0.12f);
                        Gizmos.DrawWireCube(Cell(x, y), Vector3.one * 0.95f);
                    } else {
                        Gizmos.color = new Color(0.9f, 0.2f, 0.2f, 0.55f);
                        Gizmos.DrawCube(Cell(x, y), Vector3.one * 0.9f);
                    }
                }
            }

            var s = new GridCoord(start.x, start.y);
            var g = new GridCoord(goal.x, goal.y);

            if (showFlowField && graph.IsWalkable(g)) {
                FlowField<GridCoord> field = FlowField<GridCoord>.Compute(graph, new[] { g });
                Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.8f);
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        var c = new GridCoord(x, y);
                        if (field.TryGetNext(c, out GridCoord next))
                            Gizmos.DrawLine(Cell(c), Vector3.Lerp(Cell(c), Cell(next), 0.45f));
                    }
                }
            }

            if (graph.IsWalkable(s) && graph.IsWalkable(g)) {
                PathResult<GridCoord> result = Pathfinder.AStar(graph, s, g, graph.Heuristic);
                if (result.Found) {
                    Gizmos.color = Color.green;
                    for (int i = 1; i < result.Nodes.Count; i++)
                        Gizmos.DrawLine(Cell(result.Nodes[i - 1]), Cell(result.Nodes[i]));
                    foreach (GridCoord n in result.Nodes)
                        Gizmos.DrawSphere(Cell(n), 0.12f);
                }
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Cell(s), 0.32f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Cell(g), 0.32f);
        }

        private static Vector3 Cell(int x, int y) => new Vector3(x, y, 0f);

        private static Vector3 Cell(GridCoord c) => new Vector3(c.X, c.Y, 0f);
    }
}
