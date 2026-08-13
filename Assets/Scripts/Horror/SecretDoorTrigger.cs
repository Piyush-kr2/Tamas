using UnityEngine;

/// <summary>
/// 3D Secret Wall / Bookshelf Trigger script.
/// When activated (press 'E' on secret object), slides or rotates a hidden wall to reveal a secret room!
/// </summary>
public class SecretDoorTrigger : MonoBehaviour
{
    [Header("Target Hidden Wall / Bookshelf Object")]
    public Transform secretWall;

    [Header("Movement Settings")]
    public Vector3 openPositionOffset = new Vector3(-2.5f, 0f, 0f);
    public float moveSpeed = 2.0f;
    public float interactionDistance = 3.0f;

    [Header("State")]
    public bool isActivated = false;

    private Vector3 closedPos;
    private Vector3 openPos;

    void Start()
    {
        if (secretWall != null)
        {
            closedPos = secretWall.localPosition;
            openPos = closedPos + openPositionOffset;
        }
    }

    void Update()
    {
        if (secretWall != null)
        {
            Vector3 targetPos = isActivated ? openPos : closedPos;
            secretWall.localPosition = Vector3.Lerp(secretWall.localPosition, targetPos, Time.deltaTime * moveSpeed);
        }

        // Press 'E' to trigger secret mechanism
        if (Input.GetKeyDown(KeyCode.E))
        {
            Transform mainCam = Camera.main != null ? Camera.main.transform : null;
            if (mainCam != null)
            {
                float distance = Vector3.Distance(mainCam.position, transform.position);
                if (distance <= interactionDistance)
                {
                    ActivateSecretPassage();
                }
            }
        }
    }

    public void ActivateSecretPassage()
    {
        isActivated = !isActivated;
        Debug.Log("[Horror System] SECRET PASSAGEWAY ACTIVATED! Wall moving...");
    }
}
