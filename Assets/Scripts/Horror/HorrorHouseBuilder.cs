using UnityEngine;

/// <summary>
/// 3D Horror House Generator Script.
/// Automatically builds a complete 3D Horror House layout with:
/// - Main Foyer & Creepy Room
/// - Secret Hidden Chamber behind a moving bookshelf/wall
/// - Secret Book / Switch mechanism
/// - Flickering atmospheric lights & flashlight player controls
/// - Jumpscare / Horror Trigger zones
/// </summary>
public class HorrorHouseBuilder : MonoBehaviour
{
    [ContextMenu("Build 3D Horror House Scene")]
    void Start()
    {
        BuildHouse();
    }

    public void BuildHouse()
    {
        Debug.Log("🏚️ Building 3D Horror House Environment...");

        // Parent container
        GameObject houseObj = new GameObject("3D_Horror_House");

        // Materials
        Material floorMat = CreateMaterial("Mat_DarkFloor", new Color(0.12f, 0.12f, 0.14f));
        Material wallMat = CreateMaterial("Mat_CreepyWall", new Color(0.2f, 0.22f, 0.25f));
        Material secretMat = CreateMaterial("Mat_SecretRoom", new Color(0.35f, 0.08f, 0.08f)); // Dark red ritual room
        Material goldMat = CreateMaterial("Mat_CursedBook", new Color(0.85f, 0.65f, 0.1f));

        // 1. FLOOR
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "House_Floor";
        floor.transform.SetParent(houseObj.transform);
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(2.5f, 1f, 2.5f); // 25x25m floor
        floor.GetComponent<Renderer>().material = floorMat;

        // 2. WALLS
        // Outer Walls
        CreateWall("Wall_North", new Vector3(0, 2.5f, 12.5f), new Vector3(25, 5, 0.4f), wallMat, houseObj);
        CreateWall("Wall_South", new Vector3(0, 2.5f, -12.5f), new Vector3(25, 5, 0.4f), wallMat, houseObj);
        CreateWall("Wall_West", new Vector3(-12.5f, 2.5f, 0), new Vector3(0.4f, 5, 25), wallMat, houseObj);
        CreateWall("Wall_East", new Vector3(12.5f, 2.5f, 0), new Vector3(0.4f, 5, 25), wallMat, houseObj);

        // Divider Wall between Normal Room & Secret Room
        CreateWall("Divider_Left", new Vector3(-6.5f, 2.5f, 2f), new Vector3(12, 5, 0.4f), wallMat, houseObj);
        CreateWall("Divider_Right", new Vector3(8.5f, 2.5f, 2f), new Vector3(8, 5, 0.4f), wallMat, houseObj);

        // 3. SECRET BOOKSHELF WALL (Coordinates X: 0, Y: 2.5, Z: 2)
        GameObject secretWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        secretWall.name = "Secret_Bookshelf_Wall";
        secretWall.transform.SetParent(houseObj.transform);
        secretWall.transform.position = new Vector3(1f, 2.5f, 2f);
        secretWall.transform.localScale = new Vector3(3f, 5f, 0.5f);
        secretWall.GetComponent<Renderer>().material = wallMat;

        // 4. SECRET ROOM (Behind Divider)
        // Decorate Secret Room with dark red walls
        CreateWall("SecretRoom_BackWall", new Vector3(0, 2.5f, 10f), new Vector3(12, 5, 0.4f), secretMat, houseObj);

        // 5. SECRET SWITCH / CURSED ITEM (On a pedestal)
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.name = "Secret_Pedestal";
        pedestal.transform.SetParent(houseObj.transform);
        pedestal.transform.position = new Vector3(-4f, 0.6f, -2f);
        pedestal.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        pedestal.GetComponent<Renderer>().material = wallMat;

        GameObject cursedBook = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cursedBook.name = "Cursed_Book_SecretSwitch";
        cursedBook.transform.SetParent(pedestal.transform);
        cursedBook.transform.position = new Vector3(-4f, 1.4f, -2f);
        cursedBook.transform.localScale = new Vector3(0.4f, 0.2f, 0.5f);
        cursedBook.GetComponent<Renderer>().material = goldMat;

        SecretDoorTrigger secretTrigger = cursedBook.AddComponent<SecretDoorTrigger>();
        secretTrigger.secretWall = secretWall.transform;
        secretTrigger.openPositionOffset = new Vector3(-3.2f, 0f, 0f);

        // 6. LIGHTING
        // Ambient Creepy Red Light in Secret Room
        GameObject secretLightObj = new GameObject("SecretRoom_RedLight");
        secretLightObj.transform.SetParent(houseObj.transform);
        secretLightObj.transform.position = new Vector3(0f, 3.5f, 6f);
        Light secretLight = secretLightObj.AddComponent<Light>();
        secretLight.type = LightType.Point;
        secretLight.color = new Color(1f, 0.1f, 0.1f);
        secretLight.intensity = 1.5f;
        secretLight.range = 10f;
        secretLightObj.AddComponent<FlickeringLight>();

        // Foyer Lamp Light
        GameObject foyerLightObj = new GameObject("Foyer_FlickeringLight");
        foyerLightObj.transform.SetParent(houseObj.transform);
        foyerLightObj.transform.position = new Vector3(0f, 3.5f, -4f);
        Light foyerLight = foyerLightObj.AddComponent<Light>();
        foyerLight.type = LightType.Point;
        foyerLight.color = new Color(0.9f, 0.8f, 0.6f);
        foyerLight.intensity = 1.0f;
        foyerLight.range = 12f;
        foyerLightObj.AddComponent<FlickeringLight>();

        // 7. HORROR TRIGGER ZONE (Inside Secret Room)
        GameObject triggerObj = new GameObject("SecretRoom_SurpriseTrigger");
        triggerObj.transform.SetParent(houseObj.transform);
        triggerObj.transform.position = new Vector3(0f, 1.5f, 5f);
        BoxCollider boxCol = triggerObj.AddComponent<BoxCollider>();
        boxCol.isTrigger = true;
        boxCol.size = new Vector3(6f, 3f, 3f);

        HorrorSurpriseTrigger horrorTrigger = triggerObj.AddComponent<HorrorSurpriseTrigger>();
        horrorTrigger.lightToCutOut = foyerLight;

        // 8. SETUP PLAYER & CAMERA
        SetupPlayerAndCamera();

        Debug.Log("✨ 3D Horror House with Secret Spaces & Surprises successfully built!");
    }

    private void CreateWall(string name, Vector3 pos, Vector3 scale, Material mat, GameObject parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent.transform);
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = mat;
    }

    private Material CreateMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material mat = new Material(shader);
        mat.color = color;
        return mat;
    }

    private void SetupPlayerAndCamera()
    {
        // Find existing or create Player
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
        }
        player.transform.position = new Vector3(0f, 1f, -8f);

        CharacterController cc = player.GetComponent<CharacterController>() ?? player.AddComponent<CharacterController>();
        ThirdPersonPlayerController playerController = player.GetComponent<ThirdPersonPlayerController>() ?? player.AddComponent<ThirdPersonPlayerController>();

        // Camera setup
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            ThirdPersonCameraFollow camFollow = mainCam.GetComponent<ThirdPersonCameraFollow>() ?? mainCam.gameObject.AddComponent<ThirdPersonCameraFollow>();
            camFollow.target = player.transform;

            FlashlightController flashlight = mainCam.GetComponent<FlashlightController>() ?? mainCam.gameObject.AddComponent<FlashlightController>();
        }
    }
}
