using UnityEngine;

/// <summary>
/// 3D Flashlight Controller script.
/// Attach this script to your Main Camera or Player object.
/// Press 'F' to toggle the flashlight on/off.
/// </summary>
public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Light Reference")]
    public Light flashlight;

    [Header("Settings")]
    public KeyCode toggleKey = KeyCode.F;
    public bool startsOn = true;
    public float lightIntensity = 2.5f;

    [Header("Flashlight Sway / Bobbing")]
    public bool enableSway = true;
    public float swayAmount = 0.05f;
    public float smoothSway = 4.0f;

    private bool isOn;
    private Quaternion initialRotation;

    void Start()
    {
        // If Light component not manually assigned, search on this object or create one
        if (flashlight == null)
        {
            flashlight = GetComponentInChildren<Light>();
        }

        if (flashlight == null)
        {
            // Create a 3D Spotlight dynamically if none exists
            GameObject lightObj = new GameObject("Flashlight_SpotLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = new Vector3(0.3f, -0.2f, 0.4f);
            lightObj.transform.localRotation = Quaternion.identity;

            flashlight = lightObj.AddComponent<Light>();
            flashlight.type = LightType.Spot;
            flashlight.range = 15f;
            flashlight.spotAngle = 45f;
            flashlight.intensity = lightIntensity;
            flashlight.color = new Color(0.95f, 0.95f, 0.85f);
        }

        isOn = startsOn;
        if (flashlight != null)
        {
            flashlight.enabled = isOn;
        }

        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Toggle Flashlight
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            if (flashlight != null)
            {
                flashlight.enabled = isOn;
            }
        }

        // Subtle 3D movement sway
        if (enableSway && isOn)
        {
            float mouseX = Input.GetAxis("Mouse X") * swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
            Quaternion targetRotation = initialRotation * rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSway);
        }
    }
}
