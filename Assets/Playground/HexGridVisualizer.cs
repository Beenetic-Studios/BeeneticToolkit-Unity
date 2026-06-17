using System.Collections.Generic;
using BeeneticToolkit.Spatial;
using UnityEngine;

namespace BeeneticToolkit.Unity.Playground {

    /// <summary>
    /// Draws a hex grid and one highlighted shape (Range / Ring / Spiral / Line) using BeeneticToolkit.Spatial's
    /// <see cref="Hex"/> and <see cref="HexLayout"/>. Attach to an empty GameObject and view in the Scene window
    /// (the 2D toggle gives the cleanest top-down view). Everything renders via gizmos — no materials or prefabs.
    /// </summary>
    public class HexGridVisualizer : MonoBehaviour {

        public enum HexShape { Range, Ring, Spiral, Line }

        [Header("Layout")]
        public bool pointyTop = true;
        public float hexSize = 1f;

        [Header("Shape")]
        public HexShape shape = HexShape.Range;
        public int radius = 3;
        public Vector2Int center = Vector2Int.zero;            // axial (q, r)
        public Vector2Int lineEnd = new Vector2Int(4, -2);     // axial (q, r), used by Line

        [Header("Display")]
        public bool drawLabels = true;

        [Header("Traversal order")]
        [Tooltip("Range/Ring/Spiral return the SAME cells but in different orders. " +
                 "This draws a green→red polyline through the hex centers in returned order so you can see it.")]
        public bool showTraversalOrder = false;

        [Tooltip("Label each hex with its order index (0,1,2…) instead of its q,r coordinate.")]
        public bool labelOrderIndex = false;

        private void OnDrawGizmos() {
            Gizmos.matrix = Matrix4x4.Translate(transform.position); // lets several visualizers sit apart in one scene
            HexOrientation orientation = pointyTop ? HexOrientation.Pointy : HexOrientation.Flat;
            var layout = new HexLayout(orientation, (hexSize, hexSize), (0f, 0f));
            var origin = new Hex(center.x, center.y);

            // Faint backdrop one ring larger than the shape.
            Gizmos.color = new Color(1f, 1f, 1f, 0.15f);
            foreach (Hex h in origin.Range(Mathf.Max(radius, origin.DistanceTo(new Hex(lineEnd.x, lineEnd.y))) + 1))
                DrawHexOutline(layout, h);

            // The highlighted set.
            List<Hex> selected = shape switch {
                HexShape.Range => origin.Range(radius),
                HexShape.Ring => origin.Ring(radius),
                HexShape.Spiral => origin.Spiral(radius),
                HexShape.Line => Hex.Line(origin, new Hex(lineEnd.x, lineEnd.y)),
                _ => new List<Hex>(),
            };

            Gizmos.color = Color.cyan;
            foreach (Hex h in selected)
                DrawHexOutline(layout, h);

            if (showTraversalOrder && selected.Count > 0) {
                // A polyline through the cells in returned order, fading green (first) -> red (last).
                Vector3 prev = Vector3.zero;
                for (int i = 0; i < selected.Count; i++) {
                    (float x, float y) = layout.ToPixel(selected[i]);
                    var center = new Vector3(x, y, 0f);

                    float t = selected.Count == 1 ? 0f : i / (float)(selected.Count - 1);
                    Gizmos.color = Color.Lerp(Color.green, Color.red, t);
                    Gizmos.DrawSphere(center, hexSize * 0.14f);
                    if (i > 0)
                        Gizmos.DrawLine(prev, center);

                    prev = center;
                }
            } else {
                Gizmos.color = Color.cyan;
                foreach (Hex h in selected) {
                    (float x, float y) = layout.ToPixel(h);
                    Gizmos.DrawSphere(new Vector3(x, y, 0f), hexSize * 0.12f);
                }
            }

            // Mark the origin.
            Gizmos.color = Color.magenta;
            (float ox, float oy) = layout.ToPixel(origin);
            Gizmos.DrawWireSphere(new Vector3(ox, oy, 0f), hexSize * 0.3f);

#if UNITY_EDITOR
            UnityEditor.Handles.matrix = Matrix4x4.Translate(transform.position);
            if (showTraversalOrder && labelOrderIndex) {
                UnityEditor.Handles.color = Color.white;
                for (int i = 0; i < selected.Count; i++) {
                    (float x, float y) = layout.ToPixel(selected[i]);
                    UnityEditor.Handles.Label(new Vector3(x, y, 0f), i.ToString());
                }
            } else if (drawLabels) {
                UnityEditor.Handles.color = Color.white;
                foreach (Hex h in selected) {
                    (float x, float y) = layout.ToPixel(h);
                    UnityEditor.Handles.Label(new Vector3(x, y, 0f), $"{h.Q},{h.R}");
                }
            }
#endif
        }

        private static void DrawHexOutline(HexLayout layout, Hex hex) {
            (float X, float Y)[] corners = layout.PolygonCorners(hex);
            for (int i = 0; i < corners.Length; i++) {
                (float X, float Y) a = corners[i];
                (float X, float Y) b = corners[(i + 1) % corners.Length];
                Gizmos.DrawLine(new Vector3(a.X, a.Y, 0f), new Vector3(b.X, b.Y, 0f));
            }
        }
    }
}
