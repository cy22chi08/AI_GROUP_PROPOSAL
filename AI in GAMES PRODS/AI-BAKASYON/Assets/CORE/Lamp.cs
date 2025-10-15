using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampLight : MonoBehaviour
{
    public Light2D lampLight;
    public DayNightCycle dayNightCycle;

    void Update()
    {
        if (dayNightCycle.timeOfDay < 6f || dayNightCycle.timeOfDay > 18f)
        {
            lampLight.enabled = true; // night
        }
        else
        {
            lampLight.enabled = false; // day
        }
    }
}
