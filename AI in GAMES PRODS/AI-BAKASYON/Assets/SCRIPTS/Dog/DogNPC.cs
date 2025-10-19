using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class DogNPC : MonoBehaviour
{
    [Header("Dog Settings")]
    public Animation dogAnimation;       // ✅ Legacy Animation component
    public AnimationClip sleepClip;      // Sleeping idle loop
    public AnimationClip wakeClip;       // Wake-up once
    public AnimationClip chaseClip;      // Chase loop

    public Transform player;
    public float chaseSpeed = 3f;
    public bool chaseEnabled = true;

    [Header("Transition Settings")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;
    public string nextSceneName;

    private bool isChasing = false;
    private bool isTransitioning = false;

    void Start()
    {
        // 💤 Start sleeping animation
        if (dogAnimation != null)
        {
            if (sleepClip != null) dogAnimation.AddClip(sleepClip, "Sleep");
            if (wakeClip != null)  dogAnimation.AddClip(wakeClip, "Wake");
            if (chaseClip != null) dogAnimation.AddClip(chaseClip, "Chase");

            if (sleepClip != null)
                dogAnimation.Play("Sleep");
        }
    }

    public void TriggerOuter(Collider2D other)
    {
        if (!other.CompareTag("Player") || isChasing) return;
        StartCoroutine(WakeUpDog());
    }

    public void TriggerInner(Collider2D other)
    {
        if (!other.CompareTag("Player") || isTransitioning) return;
        StartCoroutine(PlayTransition());
    }

    IEnumerator WakeUpDog()
    {
        isChasing = true;

        // 🐶 Wake-up animation
        if (dogAnimation != null && wakeClip != null)
        {
            dogAnimation.Play("Wake");
            yield return new WaitForSeconds(wakeClip.length);
        }

        // 🏃 Switch to chase
        if (dogAnimation != null && chaseClip != null)
            dogAnimation.Play("Chase");

        // Move toward player
        if (chaseEnabled && player != null)
            StartCoroutine(ChasePlayer());
    }

    IEnumerator ChasePlayer()
    {
        while (isChasing && player != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                chaseSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    IEnumerator PlayTransition()
    {
        isTransitioning = true;
        isChasing = false;

        if (dogAnimation != null)
            dogAnimation.Stop();

        // 🧍 Disable player movement
        if (player != null)
        {
            var move = player.GetComponent<PlayerMovement>();
            if (move != null)
                move.enabled = false;
        }

        // 🎬 Show video
        if (videoCanvas != null)
            videoCanvas.SetActive(true);

        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;

            videoPlayer.Play();
            while (videoPlayer.isPlaying)
                yield return null;
        }

        // 🔄 Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
