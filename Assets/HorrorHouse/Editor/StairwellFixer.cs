using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HorrorHouse.Editor
{
    public class StairwellFixer
    {
        [MenuItem("HorrorHouse/Fix Dynamic Stairwell Opening")]
        public static void Fix()
        {
            GameObject g2s = GameObject.Find("GroundToSecond");
            if (g2s == null)
            {
                Debug.LogError("Staircase 'GroundToSecond' not found.");
                return;
            }

            Renderer[] rs = g2s.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0)
            {
                Debug.LogError("Staircase has no renderers.");
                return;
            }

            // Calculate exact bounding box of the staircase
            Bounds b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);

            // Add 0.1m clearance on sides for the player to walk up without scraping walls
            float holeMinX = b.min.x - 0.1f;
            float holeMaxX = b.max.x + 0.1f;
            
            // Add 0.1m clearance at the bottom of the stairs so they can enter easily
            float holeMinZ = b.min.z - 0.1f;
            
            // No clearance at the top (max Z) - the slab must start exactly where the top step ends
            // so the player has a floor to step onto.
            float holeMaxZ = b.max.z;

            // Find the blocking slab
            GameObject oldSlab = GameObject.Find("Second_FloorSlab");
            if (oldSlab != null)
            {
                Transform parent = oldSlab.transform.parent;
                Material mat = oldSlab.GetComponent<Renderer>().sharedMaterial;
                
                // Keep the exact overall room footprint and elevation
                Bounds slabBounds = oldSlab.GetComponent<Renderer>().bounds;
                float minX = slabBounds.min.x;
                float maxX = slabBounds.max.x;
                float minZ = slabBounds.min.z;
                float maxZ = slabBounds.max.z;
                float minY = slabBounds.min.y;
                float maxY = slabBounds.max.y;

                // Destroy the solid blocking slab
                UnityEngine.Object.DestroyImmediate(oldSlab);

                // Create the 4 sections of the floor that surround the hole
                CreateBox("Second_FloorSlab_Left", parent, minX, holeMinX, minY, maxY, minZ, maxZ, mat);
                CreateBox("Second_FloorSlab_Right", parent, holeMaxX, maxX, minY, maxY, minZ, maxZ, mat);
                CreateBox("Second_FloorSlab_Front", parent, holeMinX, holeMaxX, minY, maxY, minZ, holeMinZ, mat);
                CreateBox("Second_FloorSlab_Back", parent, holeMinX, holeMaxX, minY, maxY, holeMaxZ, maxZ, mat);

                var scene = EditorSceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"SUCCESS: Replaced solid slab with 4 sections. Stairwell Hole created exactly at X({holeMinX:F2} to {holeMaxX:F2}) Z({holeMinZ:F2} to {holeMaxZ:F2}).");
            }
            else
            {
                Debug.LogWarning("Second_FloorSlab not found. It might have already been split.");
            }
        }
        
        private static void CreateBox(string name, Transform parent, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Material mat)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = new Vector3((minX + maxX) / 2.0f, (minY + maxY) / 2.0f, (minZ + maxZ) / 2.0f);
            cube.transform.localScale = new Vector3(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY), Mathf.Abs(maxZ - minZ));
            
            if (mat != null) 
            {
                cube.GetComponent<Renderer>().sharedMaterial = mat;
            }
        }
    }
}
