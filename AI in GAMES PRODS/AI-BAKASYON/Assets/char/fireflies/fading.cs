using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireflyLightFade : MonoBehaviour
{
    private Light2D fireflyLight;
    private SpriteRenderer sr;

    void Awake()
    {
        fireflyLight = GetComponent<Light2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (fireflyLight != null && sr != null)
        {
            fireflyLight.intensity = sr.color.a; // sync light brightness with sprite alpha
        }
    }
}
