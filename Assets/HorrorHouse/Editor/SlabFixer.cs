using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HorrorHouse.Editor
{
    public class SlabFixer
    {
        [MenuItem("HorrorHouse/Fix Stairwell Slab Opening")]
        public static void FixSlab()
        {
            // 1. Find the old slab
            GameObject oldSlab = GameObject.Find("Second_FloorSlab");
            if (oldSlab == null)
            {
                Debug.LogWarning("Second_FloorSlab not found. It may have already been fixed.");
                return;
            }

            Transform parent = oldSlab.transform.parent;
            Material mat = oldSlab.GetComponent<Renderer>().sharedMaterial;

            // 2. Destroy the old solid slab
            UnityEngine.Object.DestroyImmediate(oldSlab);

            // 3. Create the 4 new slabs with a rectangular void for the stairs
            // Existing stairs: X = -1.8 to 1.8, Z = -3.0 to 1.0
            CreateBox("Second_FloorSlab_Left", parent, -14.0f, -1.8f, 3.1f, 3.3f, -10.0f, 10.0f, mat);
            CreateBox("Second_FloorSlab_Right", parent, 1.8f, 14.0f, 3.1f, 3.3f, -10.0f, 10.0f, mat);
            CreateBox("Second_FloorSlab_Front", parent, -1.8f, 1.8f, 3.1f, 3.3f, -10.0f, -3.0f, mat);
            CreateBox("Second_FloorSlab_Back", parent, -1.8f, 1.8f, 3.1f, 3.3f, 1.0f, 10.0f, mat);

            // 4. Save the scene
            var currentScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            Debug.Log("Successfully split the Second_FloorSlab and created a stairwell opening!");
        }

        private static GameObject CreateBox(string name, Transform parent, float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Material mat)
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
            return cube;
        }
    }
}
