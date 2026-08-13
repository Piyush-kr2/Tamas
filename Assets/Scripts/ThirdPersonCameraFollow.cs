using UnityEngine;

/// <summary>
/// Smooth Third-Person Camera Follow script with mouse orbital camera rotation.
/// Attach this script to your Main Camera GameObject.
/// </summary>
public class ThirdPersonCameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target;

    [Header("Camera Distance & Offset")]
    public Vector3 offset = new Vector3(0f, 2.5f, -5.0f);
    public float smoothSpeed = 10.0f;

    [Header("Mouse Orbit Controls")]
    public float mouseSensitivity = 3.0f;
    public float minVerticalAngle = -20.0f;
    public float maxVerticalAngle = 60.0f;

    private float yaw = 0.0f;
    private float pitch = 15.0f;

    void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }

        // Lock mouse cursor when right-clicking or playing (Press Esc to unlock)
        Cursor.lockState = CursorLockMode.None;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Rotate camera with Right Mouse Button held down (or always if desired)
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // Calculate desired position and rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Smoothly interpolate camera position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
