using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;

namespace HorrorHouse.Editor
{
    public class StairwellInspector
    {
        public static void Inspect()
        {
            var scene = EditorSceneManager.OpenScene("Assets/HorrorHouse/Scenes/House_Blockout.unity");
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== STAIRWELL INSPECTION ===");

            GameObject g2s = GameObject.Find("GroundToSecond");
            if (g2s != null)
            {
                Renderer[] renderers = g2s.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    Bounds b = renderers[0].bounds;
                    foreach (var r in renderers) b.Encapsulate(r.bounds);
                    sb.AppendLine($"GroundToSecond Staircase Bounds: Min({b.min.x:F2}, {b.min.y:F2}, {b.min.z:F2}) Max({b.max.x:F2}, {b.max.y:F2}, {b.max.z:F2})");
                    sb.AppendLine($"GroundToSecond Transform Position: {g2s.transform.position}");
                }
                else
                {
                    sb.AppendLine("GroundToSecond Staircase has no renderers!");
                }
            }
            else
            {
                sb.AppendLine("GroundToSecond Staircase NOT FOUND!");
            }

            GameObject slab = GameObject.Find("Second_FloorSlab");
            if (slab != null)
            {
                Renderer r = slab.GetComponent<Renderer>();
                if (r != null)
                {
                    Bounds b = r.bounds;
                    sb.AppendLine($"Second_FloorSlab Bounds: Min({b.min.x:F2}, {b.min.y:F2}, {b.min.z:F2}) Max({b.max.x:F2}, {b.max.y:F2}, {b.max.z:F2})");
                }
            }
            else
            {
                sb.AppendLine("Second_FloorSlab NOT FOUND! It might have been split already.");
            }

            Debug.Log(sb.ToString());
        }
    }
}
