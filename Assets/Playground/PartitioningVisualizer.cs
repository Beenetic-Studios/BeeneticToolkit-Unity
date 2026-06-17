using System.Collections.Generic;
using BeeneticToolkit.Spatial.Partitioning;
using UnityEngine;

namespace BeeneticToolkit.Unity.Playground {

    /// <summary>
    /// Scatters seeded points, builds a <see cref="Quadtree{T}"/> over them, and highlights everything a box or
    /// circle query returns — from BeeneticToolkit.Spatial.Partitioning. Optionally overlays the
    /// <see cref="SpatialHash{T}"/> cell grid. Attach to an empty GameObject and view in the Scene window (2D).
    /// Move the query gizmo via the Inspector to watch the hit set update.
    /// </summary>
    public class PartitioningVisualizer : MonoBehaviour {

        public enum QueryShape { Circle, Box, Nearest }

        [Header("Points")]
        public int pointCount = 200;
        public int seed = 12345;
        public Vector2 areaSize = new Vector2(20f, 20f);

        [Header("Query")]
        public QueryShape queryShape = QueryShape.Circle;
        public Vector2 queryCenter = Vector2.zero;
        public float queryRadius = 4f;
        public Vector2 queryBoxHalf = new Vector2(3f, 2f);
        [Tooltip("How many nearest points to return when Query Shape is Nearest.")]
        public int nearestCount = 8;

        [Header("Spatial hash overlay")]
        public bool showHashGrid = false;
        public float cellSize = 2f;

        private List<Vector2> _points;
        private int _builtSeed;
        private int _builtCount;
        private Vector2 _builtArea;

        private void OnDrawGizmos() {
            Gizmos.matrix = Matrix4x4.Translate(transform.position); // lets several visualizers sit apart in one scene
            EnsurePoints();

            float halfX = areaSize.x * 0.5f;
            float halfY = areaSize.y * 0.5f;
            var bounds = new Aabb(-halfX, -halfY, halfX, halfY);

            var tree = new Quadtree<int>(bounds, capacity: 8, maxDepth: 8);
            for (int i = 0; i < _points.Count; i++)
                tree.Insert((_points[i].x, _points[i].y), i);

            // The covered area.
            Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 0f));

            if (showHashGrid)
                DrawHashGrid(bounds);

            // All points, dim.
            Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
            foreach (Vector2 p in _points)
                Gizmos.DrawSphere(new Vector3(p.x, p.y, 0f), 0.07f);

            // Run the query and collect hits.
            HashSet<int> hits;
            Gizmos.color = Color.yellow;
            if (queryShape == QueryShape.Circle) {
                hits = new HashSet<int>(tree.QueryRadius((queryCenter.x, queryCenter.y), queryRadius));
                DrawWireCircle(queryCenter, queryRadius);
            } else if (queryShape == QueryShape.Box) {
                var region = Aabb.FromCenter((queryCenter.x, queryCenter.y), queryBoxHalf.x, queryBoxHalf.y);
                hits = new HashSet<int>(tree.Query(region));
                Gizmos.DrawWireCube(new Vector3(queryCenter.x, queryCenter.y, 0f),
                    new Vector3(queryBoxHalf.x * 2f, queryBoxHalf.y * 2f, 0f));
            } else { // Nearest
                var near = tree.Nearest((queryCenter.x, queryCenter.y), Mathf.Max(1, nearestCount));
                hits = new HashSet<int>(near);
                var qc = new Vector3(queryCenter.x, queryCenter.y, 0f);
                Gizmos.color = new Color(1f, 0.4f, 1f, 0.9f);
                foreach (int i in near)
                    Gizmos.DrawLine(qc, new Vector3(_points[i].x, _points[i].y, 0f));
                Gizmos.DrawWireSphere(qc, 0.2f);
            }

            // Highlight hits.
            Gizmos.color = Color.green;
            foreach (int i in hits)
                Gizmos.DrawSphere(new Vector3(_points[i].x, _points[i].y, 0f), 0.14f);

#if UNITY_EDITOR
            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.position);
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.Label(new Vector3(-halfX, halfY + 0.4f, 0f), $"hits: {hits.Count} / {_points.Count}");
#endif
        }

        private void EnsurePoints() {
            if (_points != null && _builtSeed == seed && _builtCount == pointCount && _builtArea == areaSize)
                return;

            Random.InitState(seed);
            _points = new List<Vector2>(Mathf.Max(0, pointCount));
            float halfX = areaSize.x * 0.5f;
            float halfY = areaSize.y * 0.5f;
            for (int i = 0; i < pointCount; i++)
                _points.Add(new Vector2(Random.Range(-halfX, halfX), Random.Range(-halfY, halfY)));

            _builtSeed = seed;
            _builtCount = pointCount;
            _builtArea = areaSize;
        }

        private void DrawHashGrid(Aabb bounds) {
            if (cellSize <= 0f)
                return;

            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.25f);
            for (float x = Mathf.Floor(bounds.MinX / cellSize) * cellSize; x <= bounds.MaxX; x += cellSize)
                Gizmos.DrawLine(new Vector3(x, bounds.MinY, 0f), new Vector3(x, bounds.MaxY, 0f));
            for (float y = Mathf.Floor(bounds.MinY / cellSize) * cellSize; y <= bounds.MaxY; y += cellSize)
                Gizmos.DrawLine(new Vector3(bounds.MinX, y, 0f), new Vector3(bounds.MaxX, y, 0f));
        }

        private static void DrawWireCircle(Vector2 center, float radius) {
            const int segments = 48;
            Vector3 prev = new Vector3(center.x + radius, center.y, 0f);
            for (int i = 1; i <= segments; i++) {
                float a = i / (float)segments * Mathf.PI * 2f;
                var next = new Vector3(center.x + Mathf.Cos(a) * radius, center.y + Mathf.Sin(a) * radius, 0f);
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}
