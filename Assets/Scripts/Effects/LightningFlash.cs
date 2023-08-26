using UnityEngine;

public class LightningFlash : MonoBehaviour
{
    public Light flashLight; // drag your Light object here in inspector
    public ParticleSystem lightningParticle; // drag your Particle System object here in inspector
    public AudioClip[] lightningSounds;
    // Define the minimum and maximum duration for light to be on and off
    private float minLightOnDuration = 0.05f;
    private float maxLightOnDuration = 0.2f;
    private float minLightOffDuration = 0.05f;
    private float maxLightOffDuration = 0.2f;

    // Define the minimum and maximum intensity of the light
    private float minIntensity = 12.5f;
    private float maxIntensity = 36.0f;

    // Variables to control the state of the light
    private float lightTimer;
    private bool isLightOn = false;
    private bool isThunderSoundPlaying = false;

    void Start()
    {
        // Make sure the light is initially turned off
        flashLight.enabled = false;
    }

    void Update()
    {
        // Check if a particle was emitted this frame
        if (lightningParticle.particleCount > 0 && !isLightOn)
        {
            // Turn on the light and set a random intensity
            flashLight.intensity = Random.Range(minIntensity, maxIntensity);
            flashLight.enabled = true;
            isLightOn = true;
            if (!isThunderSoundPlaying)
            {
                isThunderSoundPlaying = true;
                AudioManager.Instance.PlaySecondaryEffect(lightningSounds[Random.Range(0, lightningSounds.Length)]);
            }
            // Start the timer with a random duration
            lightTimer = Random.Range(minLightOnDuration, maxLightOnDuration);
        }

        // Decrease timer
        lightTimer -= Time.deltaTime;

        // Check if the light should be turned off
        if (isLightOn && lightTimer <= 0)
        {
            // Turn off the light
            flashLight.enabled = false;
            isLightOn = false;
            isThunderSoundPlaying = false;
            // Start the timer with a random duration for off period
            lightTimer = Random.Range(minLightOffDuration, maxLightOffDuration);
        }
    }
}
