using System.Collections.Generic;
using BeeneticToolkit.Spatial;
using BeeneticToolkit.Spatial.Visibility;
using UnityEngine;

namespace BeeneticToolkit.Unity.Playground {

    /// <summary>
    /// Draws a square-grid field of view from BeeneticToolkit.Spatial.Visibility: a viewer, walls, and the cells
    /// that are lit (visible) vs. in shadow, computed by recursive shadowcasting. Attach to an empty GameObject and
    /// view in the Scene window (2D). Edit the walls list and viewer/radius in the Inspector to watch shadows move.
    /// </summary>
    public class GridFovVisualizer : MonoBehaviour {

        [Header("Grid")]
        public int width = 16;
        public int height = 12;

        [Header("Viewer")]
        public Vector2Int viewer = new Vector2Int(3, 3);
        public int radius = 6;

        [Header("Obstacles")]
        public List<Vector2Int> walls = new List<Vector2Int>();

        private void OnDrawGizmos() {
            Gizmos.matrix = Matrix4x4.Translate(transform.position); // lets several visualizers sit apart in one scene
            var wallSet = new HashSet<Vector2Int>(walls);
            bool Blocks(GridCoord c) => wallSet.Contains(new Vector2Int(c.X, c.Y));

            var origin = new GridCoord(viewer.x, viewer.y);
            HashSet<GridCoord> visible = GridFieldOfView.Compute(origin, radius, Blocks);

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    var v = new Vector3(x, y, 0f);
                    bool isWall = wallSet.Contains(new Vector2Int(x, y));
                    bool lit = visible.Contains(new GridCoord(x, y));

                    if (isWall) {
                        Gizmos.color = lit ? new Color(0.95f, 0.55f, 0.15f, 0.95f) : new Color(0.4f, 0.22f, 0.12f, 0.7f);
                        Gizmos.DrawCube(v, Vector3.one * 0.9f);
                    } else if (lit) {
                        Gizmos.color = new Color(1f, 0.95f, 0.45f, 0.35f);
                        Gizmos.DrawCube(v, Vector3.one * 0.85f);
                    } else {
                        Gizmos.color = new Color(0.12f, 0.12f, 0.18f, 0.5f);
                        Gizmos.DrawWireCube(v, Vector3.one * 0.85f);
                    }
                }
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(new Vector3(viewer.x, viewer.y, 0f), 0.35f);

#if UNITY_EDITOR
            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.position);
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(new Vector3(0f, height + 0.3f, 0f), $"visible cells: {visible.Count}");
#endif
        }
    }
}
