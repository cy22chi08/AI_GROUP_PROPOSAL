using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DogNPC : MonoBehaviour
{
    [Header("Dog Settings")]
    public Animator dogAnimator;
    public Transform player;
    public float chaseSpeed = 3f;
    public bool chaseEnabled = false;

    [Header("Transition Settings")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas; // 👈 Add this
    public string nextSceneName;

    private bool isChasing = false;
    private bool isTransitioning = false;

    public void OnDogTrigger(string triggerType, Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (triggerType == "Outer" && !isChasing)
        {
            WakeUpDog();
        }
        else if (triggerType == "Inner" && !isTransitioning)
        {
            StartCoroutine(PlayTransition());
        }
    }

    void WakeUpDog()
    {
        isChasing = true;

        if (dogAnimator != null)
            dogAnimator.SetTrigger("WakeUp");

        if (chaseEnabled && player != null)
            StartCoroutine(ChasePlayer());
    }

    System.Collections.IEnumerator ChasePlayer()
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

    System.Collections.IEnumerator PlayTransition()
    {
        isTransitioning = true;

        // Disable player movement
        if (player != null && player.GetComponent<PlayerMovement>() != null)
            player.GetComponent<PlayerMovement>().enabled = false;

        // ✅ Enable the video canvas before playing
        if (videoCanvas != null)
            videoCanvas.SetActive(true);

        if (videoPlayer != null)
        {
            // Prepare video first
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;

            // Play video
            videoPlayer.Play();

            // Wait for it to finish
            while (videoPlayer.isPlaying)
                yield return null;
        }

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
