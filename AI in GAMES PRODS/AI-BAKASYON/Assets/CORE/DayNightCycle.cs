using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Light2D globalLight;
    [Range(0, 24)] public float timeOfDay = 12f; // start at noon
    public float dayDuration = 120f; // 2 minutes for a full cycle
    private float timeSpeed; 

    void Start()
    {
        timeSpeed = 24f / dayDuration;
    }

    void Update()
    {
        // Advance time
        timeOfDay += Time.deltaTime * timeSpeed;
        if (timeOfDay > 24f) timeOfDay -= 24f;

        // --- Smooth transition version ---
        float targetIntensity;

        if (timeOfDay < 6f || timeOfDay > 18f)
            targetIntensity = 0.01f; // night
        else if (timeOfDay < 8f)
            targetIntensity = Mathf.Lerp(0.25f, 1f, (timeOfDay - 6f) / 2f); // sunrise
        else if (timeOfDay > 16f)
            targetIntensity = Mathf.Lerp(1f, 0.25f, (timeOfDay - 16f) / 2f); // sunset
        else
            targetIntensity = 1f; // full day

        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, Time.deltaTime);

        // --- Color transition ---
        Color dayColor = new Color(1f, 0.95f, 0.8f);
        Color nightColor = new Color(0.25f, 0.35f, 0.65f);
        globalLight.color = Color.Lerp(nightColor, dayColor, Mathf.InverseLerp(0.25f, 1f, globalLight.intensity));
    }
}
