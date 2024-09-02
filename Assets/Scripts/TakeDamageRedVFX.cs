using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TakeDamageRedVFX : MonoBehaviour
{
    public float intensity = 0f;

    private PostProcessVolume _volume;
    private Vignette _vignette;
    private Coroutine damageCoroutine; // To manage coroutine state

    // Start is called before the first frame update
    void Start()
    {
        _volume = GetComponent<PostProcessVolume>();
        _volume.profile.TryGetSettings<Vignette>(out _vignette);

        if (!_vignette)
        {
            Debug.Log("Error, vignette empty");
        }
        else
        {
            _vignette.enabled.Override(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // No update logic needed unless you want continuous checks or effects
    }

    public void TriggerTakeDamageEffect(int health)
    {
        // Stop any existing coroutine to prevent overlapping effects
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        // Start the new coroutine with the given health value
        damageCoroutine = StartCoroutine(TakeDamageEffect(health));
    }
private IEnumerator TakeDamageEffect(int health)
{
    // Calculate initial intensity based on health (0 health -> 0.4 intensity, 100 health -> 0 intensity)
    intensity = Mathf.Lerp(0.4f, 0f, health / 100f);
    
    // Ensure intensity doesn't exceed 0.55
    intensity = Mathf.Min(intensity, 0.55f);

    // Enable the vignette effect with the calculated intensity
    _vignette.enabled.Override(true);
    _vignette.intensity.Override(intensity);

    // Increase intensity by 0.1 on hit
    intensity += 0.2f;

    // Cap the intensity to a maximum of 0.55
    intensity = Mathf.Min(intensity, 0.55f);
    _vignette.intensity.Override(intensity);

    // Wait for 1 second before starting the decay
    yield return new WaitForSeconds(1f);

    // Calculate the target intensity as half of the initial intensity
    float targetIntensity = intensity / 2f;

    while (intensity > targetIntensity)
    {
        // Gradually reduce intensity towards the target intensity
        intensity = Mathf.MoveTowards(intensity, targetIntensity, 0.01f);

        _vignette.intensity.Override(intensity);

        // Wait for 0.1 seconds before updating the intensity again
        yield return new WaitForSeconds(0.1f);
    }

    // After the loop, check if intensity is 0 or below, and disable the vignette if necessary
    if (intensity <= 0)
    {
        _vignette.enabled.Override(false);
    }
}

}
