using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FireflySwarm : MonoBehaviour
{
    public Transform player;
    public DayNightCycle dayNightCycle;
    public GameObject fireflyPrefab;
    public int fireflyCount = 10;
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 1f;
    public float hoverAmplitude = 0.3f;
    public float hoverSpeed = 2f;
    public float fadeDuration = 2f;

    private bool active = false;
    private float currentAlpha = 0f;
    private GameObject[] fireflies;
    private float[] orbitOffsets;

    void Start()
    {
        // Spawn once, but hidden
        fireflies = new GameObject[fireflyCount];
        orbitOffsets = new float[fireflyCount];

        for (int i = 0; i < fireflyCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * orbitRadius;
            fireflies[i] = Instantiate(fireflyPrefab, player.position + (Vector3)offset, Quaternion.identity, transform);
            fireflies[i].SetActive(false);
            orbitOffsets[i] = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        if (player == null || dayNightCycle == null)
            return;

        bool isNight = dayNightCycle.timeOfDay < 6f || dayNightCycle.timeOfDay > 18f;

        if (isNight && !active)
            StartCoroutine(FadeInFireflies());
        else if (!isNight && active)
            StartCoroutine(FadeOutFireflies());

        if (active)
        {
            // Follow player smoothly
            transform.position = Vector3.Lerp(transform.position, player.position, Time.deltaTime * 3f);

            // Orbit & hover movement
            for (int i = 0; i < fireflies.Length; i++)
            {
                if (fireflies[i] == null) continue;

                float angle = Time.time * orbitSpeed + orbitOffsets[i];
                float hover = Mathf.Sin(Time.time * hoverSpeed + orbitOffsets[i]) * hoverAmplitude;

                Vector3 orbitPos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle) + hover, 0) * orbitRadius;
                fireflies[i].transform.position = Vector3.Lerp(fireflies[i].transform.position, orbitPos, Time.deltaTime * 2f);
            }
        }
    }

    IEnumerator FadeInFireflies()
    {
        active = true;

        foreach (var f in fireflies)
            f.SetActive(true);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            currentAlpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(currentAlpha);
            yield return null;
        }
        currentAlpha = 1f;
        SetAlpha(currentAlpha);
    }

    IEnumerator FadeOutFireflies()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            currentAlpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(currentAlpha);
            yield return null;
        }

        currentAlpha = 0f;
        SetAlpha(currentAlpha);

        foreach (var f in fireflies)
            f.SetActive(false);

        active = false;
    }

    void SetAlpha(float alpha)
    {
        foreach (var f in fireflies)
        {
            if (f == null) continue;
            SpriteRenderer sr = f.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            // Optional: dim light intensity too
            Light2D light = f.GetComponent<Light2D>();
            if (light != null)
                light.intensity = alpha; // fade brightness
        }
    }
}
