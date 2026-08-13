using UnityEngine;

/// <summary>
/// 3D Horror Trigger Zone for jumpscares and atmospheric surprises.
/// Place on a 3D Trigger Collider in a hallway or doorway.
/// </summary>
public class HorrorSurpriseTrigger : MonoBehaviour
{
    [Header("Surprise Options")]
    public DoorInteraction doorToSlam;
    public Light lightToCutOut;
    public Rigidbody physicsPropToThrow;
    public Vector3 throwForce = new Vector3(0f, 2f, -8f);

    [Header("Trigger Behavior")]
    public bool triggerOnce = true;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;

        // Check if player entered trigger
        if (other.CompareTag("Player") || other.GetComponent<ThirdPersonPlayerController>() != null || other.GetComponent<CharacterController>() != null)
        {
            hasTriggered = true;
            ExecuteSurprise();
        }
    }

    private void ExecuteSurprise()
    {
        Debug.Log("👻 [HORROR SURPRISE TRIGGERED!]");

        // 1. Slam door shut behind player
        if (doorToSlam != null)
        {
            doorToSlam.ForceSlamClose();
        }

        // 2. Blackout light
        if (lightToCutOut != null)
        {
            lightToCutOut.enabled = false;
        }

        // 3. Throw prop across room with 3D physics
        if (physicsPropToThrow != null)
        {
            physicsPropToThrow.isKinematic = false;
            physicsPropToThrow.AddForce(throwForce, ForceMode.Impulse);
            physicsPropToThrow.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
    }
}
