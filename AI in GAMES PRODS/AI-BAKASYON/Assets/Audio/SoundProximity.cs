using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
    public Transform player;
    public float audibleRange = 6f;
    public float fullVolumeDistance = 1.5f;
    public float fadeSpeed = 2f;

    private AudioSource audioSource;
    private bool isPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f; // start silent
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= audibleRange)
        {
            if (!isPlaying)
            {
                audioSource.Play();
                isPlaying = true;
            }

            float t = Mathf.InverseLerp(audibleRange, fullVolumeDistance, distance);
            float targetVolume = Mathf.Lerp(0f, AudioManager.Instance.sfxVolume, 1 - t);
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            if (isPlaying)
            {
                audioSource.Stop();
                isPlaying = false;
            }
        }
    }
}
