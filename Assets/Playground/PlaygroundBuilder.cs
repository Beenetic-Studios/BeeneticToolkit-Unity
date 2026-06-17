#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeeneticToolkit.Unity.Playground {

    // One-click setup: builds a scene with all four spatial visualizers laid out side by side so you
    // can see grids / pathfinding / FOV / partitioning at once. Each visualizer offsets its gizmos by
    // its GameObject position (via Gizmos.matrix), so they don't overlap. Menu: Beenetic > Create
    // Spatial Playground. Editor-only — compiled out of player builds.
    public static class PlaygroundBuilder {

        [MenuItem("Beenetic/Create Spatial Playground")]
        public static void Create() {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return; // user cancelled the save prompt — don't discard their current scene

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var spawned = new List<Object> {
                Spawn<HexGridVisualizer>("Hex Grid Visualizer", new Vector3(0f, 0f, 0f)),
                Spawn<GridPathfindingVisualizer>("Grid Pathfinding Visualizer", new Vector3(30f, 0f, 0f)),
                Spawn<GridFovVisualizer>("Grid FOV Visualizer", new Vector3(60f, 0f, 0f)),
                Spawn<PartitioningVisualizer>("Partitioning Visualizer", new Vector3(30f, -40f, 0f)),
            };

            Selection.objects = spawned.ToArray();
            SceneView sv = SceneView.lastActiveSceneView;
            if (sv != null) {
                sv.in2DMode = true;
                sv.FrameSelected();
            }

            const string path = "Assets/Playground/SpatialPlayground.unity";
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[Beenetic] Created spatial playground at {path}. View it in the Scene window with the 2D toggle on.");
        }

        private static GameObject Spawn<T>(string name, Vector3 position) where T : MonoBehaviour {
            var go = new GameObject(name);
            go.transform.position = position;
            go.AddComponent<T>();
            return go;
        }
    }
}
#endif
