using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace HorrorHouse.Editor
{
    public class StairwellAutoFixer
    {
        [MenuItem("HorrorHouse/Stairwell Diagnostic Tool")]
        public static void RunDiagnostic()
        {
            Debug.Log("=== STAIRWELL AUTO FIX STARTED ===");
            StringBuilder report = new StringBuilder();
            report.AppendLine("=== STAIRWELL DIAGNOSTIC REPORT ===");

            // 3. VERIFY THE ACTUAL OBJECTS
            report.AppendLine("\n--- 3. VERIFY THE ACTUAL OBJECTS ---");
            
            GameObject g2s = GameObject.Find("GroundToSecond");
            if (g2s == null)
            {
                // Try to find it manually
                report.AppendLine("GroundToSecond not found by exact name. Searching for 'Staircase' or 'GroundToSecond'...");
                foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (go.name.Contains("Stair") || go.name.Contains("GroundToSecond"))
                    {
                        g2s = go;
                        report.AppendLine($"Found alternative: {go.name} at {GetPath(go.transform)}");
                        break;
                    }
                }
            }

            if (g2s != null)
            {
                report.AppendLine($"Staircase Object: {g2s.name}");
                report.AppendLine($"Path: {GetPath(g2s.transform)}");
                report.AppendLine($"Active: {g2s.activeInHierarchy}");
                report.AppendLine($"Position: {g2s.transform.position}");
                
                Renderer[] rs = g2s.GetComponentsInChildren<Renderer>();
                report.AppendLine($"Step Children Count: {rs.Length}");
                
                // Don't log every single step to avoid massive spam, but log the first and last to verify
                if (rs.Length > 0)
                {
                    report.AppendLine($"  - First Child: {rs[0].name}, Path: {GetPath(rs[0].transform)}, Pos: {rs[0].transform.position}");
                    report.AppendLine($"  - Last Child: {rs[rs.Length-1].name}, Path: {GetPath(rs[rs.Length-1].transform)}, Pos: {rs[rs.Length-1].transform.position}");
                }

                if (rs.Length > 0)
                {
                    Bounds stairBounds = rs[0].bounds;
                    foreach (var r in rs) stairBounds.Encapsulate(r.bounds);
                    report.AppendLine($"\nSTAIR BOUNDS (WORLD SPACE):");
                    report.AppendLine($"min X: {stairBounds.min.x:F3}");
                    report.AppendLine($"max X: {stairBounds.max.x:F3}");
                    report.AppendLine($"min Y: {stairBounds.min.y:F3}");
                    report.AppendLine($"max Y: {stairBounds.max.y:F3}");
                    report.AppendLine($"min Z: {stairBounds.min.z:F3}");
                    report.AppendLine($"max Z: {stairBounds.max.z:F3}");

                    // 5. FIND THE WALL
                    report.AppendLine("\n--- 5. FIND THE WALL CAUSING THE PROBLEM ---");
                    GameObject gf = GameObject.Find("GroundFloor");
                    if (gf != null)
                    {
                        Renderer[] walls = gf.GetComponentsInChildren<Renderer>();
                        Renderer closestWall = null;
                        float minClearance = 0.20f;
                        float smallestDist = float.MaxValue;
                        float finalDx = 0, finalDz = 0;

                        foreach (var w in walls)
                        {
                            if (w.gameObject.name.Contains("Slab") || w.gameObject.name.Contains("Floor")) continue;
                            Bounds wb = w.bounds;
                            if (wb.max.y > stairBounds.min.y && wb.min.y < stairBounds.max.y)
                            {
                                float dx = Mathf.Max(wb.min.x - stairBounds.max.x, stairBounds.min.x - wb.max.x);
                                float dz = Mathf.Max(wb.min.z - stairBounds.max.z, stairBounds.min.z - wb.max.z);
                                float dist = Mathf.Max(dx, dz);
                                
                                if (dist < minClearance && dist < smallestDist)
                                {
                                    smallestDist = dist;
                                    closestWall = w;
                                    finalDx = dx; finalDz = dz;
                                }
                            }
                        }

                        if (closestWall != null)
                        {
                            Bounds wb = closestWall.bounds;
                            report.AppendLine($"WALL BOUNDS: {closestWall.name}");
                            report.AppendLine($"min X: {wb.min.x:F3}");
                            report.AppendLine($"max X: {wb.max.x:F3}");
                            report.AppendLine($"min Y: {wb.min.y:F3}");
                            report.AppendLine($"max Y: {wb.max.y:F3}");
                            report.AppendLine($"min Z: {wb.min.z:F3}");
                            report.AppendLine($"max Z: {wb.max.z:F3}");
                            
                            bool intersects = (finalDx < 0 && finalDz < 0);
                            report.AppendLine($"Intersects Staircase: {intersects}");
                            report.AppendLine($"Current Clearance: {smallestDist:F3}m");

                            // 6. CALCULATE TRANSLATION
                            float neededShiftX = 0;
                            if (stairBounds.center.x > wb.center.x)
                                neededShiftX = (wb.max.x + minClearance) - stairBounds.min.x;
                            else
                                neededShiftX = (wb.min.x - minClearance) - stairBounds.max.x;

                            float neededShiftZ = 0;
                            if (stairBounds.center.z > wb.center.z)
                                neededShiftZ = (wb.max.z + minClearance) - stairBounds.min.z;
                            else
                                neededShiftZ = (wb.min.z - minClearance) - stairBounds.max.z;

                            Vector3 pushVector = Vector3.zero;
                            if (Mathf.Abs(neededShiftX) < Mathf.Abs(neededShiftZ))
                                pushVector.x = neededShiftX;
                            else
                                pushVector.z = neededShiftZ;

                            report.AppendLine($"Calculated required translation (NOT APPLIED): {pushVector}");
                        }
                        else
                        {
                            report.AppendLine("No GroundFloor wall found within 0.20m of staircase.");
                        }
                    }
                }
            }

            // 8. CHECK FOR DUPLICATE/OLD SLABS
            report.AppendLine("\n--- 7 & 8. CHECK FOR SECOND-FLOOR SLABS ---");
            Renderer[] allRenderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var r in allRenderers)
            {
                if (r.gameObject.name.Contains("Second_FloorSlab"))
                {
                    report.AppendLine($"Found Slab: {r.gameObject.name}");
                    report.AppendLine($"Path: {GetPath(r.transform)}");
                    report.AppendLine($"Active: {r.gameObject.activeInHierarchy}");
                    report.AppendLine($"Bounds: Min({r.bounds.min.x:F3}, {r.bounds.min.y:F3}, {r.bounds.min.z:F3}) Max({r.bounds.max.x:F3}, {r.bounds.max.y:F3}, {r.bounds.max.z:F3})");
                }
            }

            File.WriteAllText("StairwellDiagnostics.txt", report.ToString());
            Debug.Log(report.ToString());
            Debug.Log("=== STAIRWELL AUTO FIX FINISHED ===");
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}
