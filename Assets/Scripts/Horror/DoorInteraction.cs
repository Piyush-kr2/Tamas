using UnityEngine;

/// <summary>
/// 3D Door Interaction script.
/// Allows player to press 'E' when close to open/close or unlock 3D doors.
/// </summary>
public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public bool isLocked = false;
    public float openAngle = 90.0f;
    public float speed = 3.0f;
    public float interactionDistance = 3.0f;

    [Header("Key / Secret Requirements")]
    public string requiredKeyName = "";

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + openAngle, transform.localEulerAngles.z);
    }

    void Update()
    {
        // Smooth rotation towards target open/closed state
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * speed);

        // Check for player interaction (Press E)
        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform mainCam = Camera.main != null ? Camera.main.transform : null;
            if (mainCam != null)
            {
                float distance = Vector3.Distance(mainCam.position, transform.position);
                if (distance <= interactionDistance)
                {
                    TryToggleDoor();
                }
            }
        }
    }

    public void TryToggleDoor()
    {
        if (isLocked)
        {
            Debug.Log("[Horror System] The door is locked tight!");
            return;
        }

        isOpen = !isOpen;
        Debug.Log("[Horror System] Door toggled: " + (isOpen ? "OPEN" : "CLOSED"));
    }

    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("[Horror System] Door Unlocked!");
    }

    public void ForceSlamClose()
    {
        isOpen = false;
        Debug.Log("[Horror System] DOOR SLAMMED SHUT!");
    }
}
