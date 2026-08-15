using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HorrorHouse.Editor
{
    public class SliceBlockedStairwell
    {
        [MenuItem("HorrorHouse/Slice Blocked Stairwell")]
        public static void Slice()
        {
            GameObject slab = GameObject.Find("Second_FloorSlab");
            if (slab == null)
            {
                Debug.LogError("Second_FloorSlab not found. It may have already been sliced.");
                return;
            }

            GameObject g2s = GameObject.Find("GroundToSecond");
            if (g2s == null)
            {
                Debug.LogError("GroundToSecond staircase not found.");
                return;
            }

            // 1. Calculate Staircase Bounds
            Renderer[] rs = g2s.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) return;
            Bounds b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);

            // Give the stairwell 0.1m clearance on left, right, and bottom (Z).
            // Do NOT give clearance at the top (max Z) so the player has floor to step on at the landing.
            float holeMinX = b.min.x - 0.1f;
            float holeMaxX = b.max.x + 0.1f;
            float holeMinZ = b.min.z - 0.1f;
            float holeMaxZ = b.max.z;

            // 2. Get the old slab properties
            Transform parent = slab.transform.parent;
            Material mat = slab.GetComponent<Renderer>().sharedMaterial;
            Bounds slabBounds = slab.GetComponent<Renderer>().bounds;
            float minX = slabBounds.min.x; // -14
            float maxX = slabBounds.max.x; // 14
            float minZ = slabBounds.min.z; // -10
            float maxZ = slabBounds.max.z; // 10
            float minY = slabBounds.min.y; // 3.1
            float maxY = slabBounds.max.y; // 3.3

            // 3. Destroy the blocker
            UnityEngine.Object.DestroyImmediate(slab);

            // 4. Create 4 real rectangular slabs around the opening
            CreateBox("Second_FloorSlab_Left", parent, minX, holeMinX, minY, maxY, minZ, maxZ, mat);
            CreateBox("Second_FloorSlab_Right", parent, holeMaxX, maxX, minY, maxY, minZ, maxZ, mat);
            CreateBox("Second_FloorSlab_Front", parent, holeMinX, holeMaxX, minY, maxY, minZ, holeMinZ, mat);
            CreateBox("Second_FloorSlab_Back", parent, holeMinX, holeMaxX, minY, maxY, holeMaxZ, maxZ, mat);

            // 5. Save the scene
            var currentScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            Debug.Log($"SUCCESS: Sliced Second_FloorSlab! Opening created at X({holeMinX:F2} to {holeMaxX:F2}), Z({holeMinZ:F2} to {holeMaxZ:F2}).");
        }
        
        private static void CreateBox(string name, Transform parent, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Material mat)
        {
            if (minX >= maxX || minZ >= maxZ) return;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            if (parent != null) cube.transform.SetParent(parent);
            cube.transform.position = new Vector3((minX + maxX) / 2.0f, (minY + maxY) / 2.0f, (minZ + maxZ) / 2.0f);
            cube.transform.localScale = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), Mathf.Abs(maxZ - minZ));
            if (mat != null) cube.GetComponent<Renderer>().sharedMaterial = mat;
        }
    }
}
