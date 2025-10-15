using UnityEngine;

public class FireflyController : MonoBehaviour
{
    public DayNightCycle dayNightCycle;
    private ParticleSystem particles;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (dayNightCycle.timeOfDay < 6f || dayNightCycle.timeOfDay > 18f)
        {
            if (!particles.isPlaying)
                particles.Play(); // Show at night
        }
        else
        {
            if (particles.isPlaying)
                particles.Stop(); // Hide at day
        }
    }
}
