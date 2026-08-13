using UnityEngine;

/// <summary>
/// 3D Flickering Light script for horror ambiance.
/// Attach to any 3D Point Light, Spot Light, or Lamp object.
/// </summary>
[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0.1f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.08f;

    [Header("Random Outages")]
    public bool allowSuddenOutages = true;
    public float outageChance = 0.05f;

    private Light targetLight;
    private float timer;

    void Start()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (targetLight == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Random chance for complete dark outage moment
            if (allowSuddenOutages && Random.value < outageChance)
            {
                targetLight.intensity = 0f;
                timer = Random.Range(0.2f, 0.8f);
            }
            else
            {
                targetLight.intensity = Random.Range(minIntensity, maxIntensity);
                timer = flickerSpeed + Random.Range(-0.02f, 0.02f);
            }
        }
    }
}
