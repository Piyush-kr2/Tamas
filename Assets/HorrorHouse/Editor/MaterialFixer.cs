using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

namespace HorrorHouse.Editor
{
    public class MaterialFixer
    {
        [MenuItem("HorrorHouse/Fix Pink Materials")]
        public static void FixMaterials()
        {
            // Check if URP is active
            if (GraphicsSettings.currentRenderPipeline == null)
            {
                Debug.LogWarning("WARNING: No URP Asset assigned in Graphics Settings! Materials might stay pink. Please check Project Settings -> Graphics.");
            }

            string matPath = "Assets/HorrorHouse/Materials/Mat_Greybox_Fixed.mat";
            
            // Create a fresh guaranteed URP material
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null) 
            {
                Debug.LogError("Could not find 'Universal Render Pipeline/Lit' shader. Ensure the URP package is installed.");
                return;
            }

            Material fixedMat = new Material(urpLit);
            fixedMat.SetColor("_BaseColor", new Color(0.7f, 0.7f, 0.7f, 1.0f));
            fixedMat.SetFloat("_Smoothness", 0.1f);
            
            AssetDatabase.CreateAsset(fixedMat, matPath);
            AssetDatabase.SaveAssets();

            // Find all renderers in the current scene and apply the material
            MeshRenderer[] renderers = UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int count = 0;
            
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.sharedMaterial = fixedMat;
                EditorUtility.SetDirty(renderer);
                count++;
            }

            // Save the scene
            var currentScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);

            Debug.Log($"Success: Fixed {count} renderers by applying a fresh URP Material!");
        }
    }
}
