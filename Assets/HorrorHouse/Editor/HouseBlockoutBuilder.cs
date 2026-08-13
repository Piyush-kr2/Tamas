using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace HorrorHouse.Editor
{
    public class HouseBlockoutBuilder
    {
        [MenuItem("HorrorHouse/Build Blockout Scene")]
        public static void BuildScene()
        {
            string scenePath = "Assets/HorrorHouse/Scenes/House_Blockout.unity";
            string matPath = "Assets/HorrorHouse/Materials/Mat_Greybox.mat";

            Material greyboxMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (greyboxMat == null)
            {
                // Fallback to URP Lit Shader
                Shader urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
                greyboxMat = new Material(urpLitShader != null ? urpLitShader : Shader.Find("Standard"));
                greyboxMat.color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                AssetDatabase.CreateAsset(greyboxMat, matPath);
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Root
            GameObject house = new GameObject("House");

            // Major Categories
            GameObject groundFloor = new GameObject("GroundFloor");
            groundFloor.transform.SetParent(house.transform);

            GameObject secondFloor = new GameObject("SecondFloor");
            secondFloor.transform.SetParent(house.transform);

            GameObject basement = new GameObject("Basement");
            basement.transform.SetParent(house.transform);

            GameObject staircase = new GameObject("Staircase");
            staircase.transform.SetParent(house.transform);

            GameObject balcony = new GameObject("Balcony");
            balcony.transform.SetParent(house.transform);

            GameObject exterior = new GameObject("Exterior");
            exterior.transform.SetParent(house.transform);

            // Ground Floor Rooms
            GameObject gf_fc = new GameObject("FloorsAndCeilings"); gf_fc.transform.SetParent(groundFloor.transform);
            GameObject gf_living = new GameObject("LivingRoom"); gf_living.transform.SetParent(groundFloor.transform);
            GameObject gf_kitchen = new GameObject("KitchenDining"); gf_kitchen.transform.SetParent(groundFloor.transform);
            GameObject gf_office = new GameObject("OfficeGameRoom"); gf_office.transform.SetParent(groundFloor.transform);
            GameObject gf_guest = new GameObject("GuestBedroom"); gf_guest.transform.SetParent(groundFloor.transform);
            GameObject gf_foyer = new GameObject("FoyerAndHall"); gf_foyer.transform.SetParent(groundFloor.transform);
            GameObject gf_garage = new GameObject("Garage"); gf_garage.transform.SetParent(groundFloor.transform);
            GameObject gf_bath = new GameObject("Bathroom"); gf_bath.transform.SetParent(groundFloor.transform);

            // Second Floor Rooms
            GameObject sf_fc = new GameObject("FloorsAndCeilings"); sf_fc.transform.SetParent(secondFloor.transform);
            GameObject sf_master = new GameObject("MasterBedroom"); sf_master.transform.SetParent(secondFloor.transform);
            GameObject sf_lounge = new GameObject("FamilyLounge"); sf_lounge.transform.SetParent(secondFloor.transform);
            GameObject sf_bed2 = new GameObject("Bedroom2"); sf_bed2.transform.SetParent(secondFloor.transform);
            GameObject sf_bed3 = new GameObject("Bedroom3"); sf_bed3.transform.SetParent(secondFloor.transform);
            GameObject sf_bath = new GameObject("Bathroom"); sf_bath.transform.SetParent(secondFloor.transform);
            GameObject sf_hall = new GameObject("Hall"); sf_hall.transform.SetParent(secondFloor.transform);

            // Basement Rooms
            GameObject bm_fc = new GameObject("FloorsAndCeilings"); bm_fc.transform.SetParent(basement.transform);
            GameObject bm_vault = new GameObject("OpenArea"); bm_vault.transform.SetParent(basement.transform);
            GameObject bm_storage = new GameObject("Storage"); bm_storage.transform.SetParent(basement.transform);
            GameObject bm_utility = new GameObject("UtilityRoom"); bm_utility.transform.SetParent(basement.transform);
            GameObject bm_workshop = new GameObject("Workshop"); bm_workshop.transform.SetParent(basement.transform);
            GameObject bm_lobby = new GameObject("StairLobby"); bm_lobby.transform.SetParent(basement.transform);

            // Staircase
            GameObject st_g2s = new GameObject("GroundToSecond"); st_g2s.transform.SetParent(staircase.transform);
            GameObject st_b2g = new GameObject("BasementToGround"); st_b2g.transform.SetParent(staircase.transform);

            // Balcony
            GameObject balc_slab = new GameObject("Slab"); balc_slab.transform.SetParent(balcony.transform);
            GameObject balc_rails = new GameObject("Railings"); balc_rails.transform.SetParent(balcony.transform);

            // Exterior
            GameObject ext_walls = new GameObject("OuterWalls"); ext_walls.transform.SetParent(exterior.transform);
            GameObject ext_roof = new GameObject("Roof"); ext_roof.transform.SetParent(exterior.transform);

            // Slabs
            CreateBox("Basement_FloorSlab", bm_fc.transform, -14.0f, 14.0f, -3.3f, -3.1f, -10.0f, 10.0f, greyboxMat);
            CreateBox("Ground_FloorSlab", gf_fc.transform, -14.0f, 14.0f, -0.1f, 0.1f, -10.0f, 10.0f, greyboxMat);
            CreateBox("Second_FloorSlab", sf_fc.transform, -14.0f, 14.0f, 3.1f, 3.3f, -10.0f, 10.0f, greyboxMat);
            CreateBox("Roof_Slab", ext_roof.transform, -14.0f, 14.0f, 6.3f, 6.5f, -10.0f, 10.0f, greyboxMat);

            // Outer Walls
            CreateBox("BM_Wall_North", bm_fc.transform, -14.0f, 14.0f, -3.1f, -0.1f, 9.8f, 10.0f, greyboxMat);
            CreateBox("BM_Wall_South", bm_fc.transform, -14.0f, 14.0f, -3.1f, -0.1f, -10.0f, -9.8f, greyboxMat);
            CreateBox("BM_Wall_West", bm_fc.transform, -14.0f, -13.8f, -3.1f, -0.1f, -10.0f, 10.0f, greyboxMat);
            CreateBox("BM_Wall_East", bm_fc.transform, 13.8f, 14.0f, -3.1f, -0.1f, -10.0f, 10.0f, greyboxMat);

            // Ground Floor Outer Walls
            CreateBox("GF_Wall_North_Left", ext_walls.transform, -14.0f, -10.0f, 0.1f, 3.1f, 9.8f, 10.0f, greyboxMat);
            CreateBox("GF_Wall_North_Mid", ext_walls.transform, -7.0f, -1.0f, 0.1f, 3.1f, 9.8f, 10.0f, greyboxMat);
            CreateBox("GF_Wall_North_Right", ext_walls.transform, 3.0f, 14.0f, 0.1f, 3.1f, 9.8f, 10.0f, greyboxMat);
            CreateBox("GF_Wall_South_Left", ext_walls.transform, -14.0f, -1.0f, 0.1f, 3.1f, -10.0f, -9.8f, greyboxMat);
            CreateBox("GF_Wall_South_Mid", ext_walls.transform, 1.0f, 7.0f, 0.1f, 3.1f, -10.0f, -9.8f, greyboxMat);
            CreateBox("GF_Wall_South_Right", ext_walls.transform, 12.5f, 14.0f, 0.1f, 3.1f, -10.0f, -9.8f, greyboxMat);
            CreateBox("GF_Door_MainEntrance_Top", ext_walls.transform, -1.0f, 1.0f, 2.3f, 3.1f, -10.0f, -9.8f, greyboxMat);
            CreateBox("GF_Shutter_Garage_Top", ext_walls.transform, 7.0f, 12.5f, 2.6f, 3.1f, -10.0f, -9.8f, greyboxMat);

            CreateBox("GF_Wall_West_Rear", ext_walls.transform, -14.0f, -13.8f, 0.1f, 3.1f, 7.0f, 10.0f, greyboxMat);
            CreateBox("GF_Wall_West_Mid", ext_walls.transform, -14.0f, -13.8f, 0.1f, 3.1f, -5.0f, 3.0f, greyboxMat);
            CreateBox("GF_Wall_West_Front", ext_walls.transform, -14.0f, -13.8f, 0.1f, 3.1f, -10.0f, -8.0f, greyboxMat);
            CreateBox("GF_Wall_East_Rear", ext_walls.transform, 13.8f, 14.0f, 0.1f, 3.1f, 7.0f, 10.0f, greyboxMat);
            CreateBox("GF_Wall_East_Front", ext_walls.transform, 13.8f, 14.0f, 0.1f, 3.1f, -10.0f, 4.0f, greyboxMat);

            // Second Floor Outer Walls
            CreateBox("SF_Wall_North", ext_walls.transform, -14.0f, 14.0f, 3.3f, 6.3f, 9.8f, 10.0f, greyboxMat);
            CreateBox("SF_Wall_South", ext_walls.transform, -14.0f, 14.0f, 3.3f, 6.3f, -10.0f, -9.8f, greyboxMat);
            CreateBox("SF_Wall_West", ext_walls.transform, -14.0f, -13.8f, 3.3f, 6.3f, -10.0f, 10.0f, greyboxMat);
            CreateBox("SF_Wall_East", ext_walls.transform, 13.8f, 14.0f, 3.3f, 6.3f, -10.0f, 10.0f, greyboxMat);

            // Balcony
            CreateBox("Balcony_Slab", balc_slab.transform, -13.8f, 5.85f, 3.1f, 3.3f, -9.8f, -6.5f, greyboxMat);
            CreateBox("Balcony_Railing_Front", balc_rails.transform, -13.8f, 5.85f, 3.3f, 4.4f, -9.8f, -9.65f, greyboxMat);
            CreateBox("Balcony_Railing_Left", balc_rails.transform, -13.8f, -13.65f, 3.3f, 4.4f, -9.8f, -6.5f, greyboxMat);
            CreateBox("Balcony_Railing_Right", balc_rails.transform, 5.7f, 5.85f, 3.3f, 4.4f, -9.8f, -6.5f, greyboxMat);

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"Horror House Blockout Scene successfully saved with URP materials to {scenePath}");
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
