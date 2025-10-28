using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class DogNPC : MonoBehaviour
{
    [Header("Dog Settings")]
    public Animation dogAnimation;
    public AnimationClip sleepClip;
    public AnimationClip wakeClip;
    public AnimationClip chaseClip;

    public Transform player;
    public float chaseSpeed = 3f;

    [Header("Transition Settings")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;
    public string nextSceneName;

    private bool isChasing = false;
    private bool isTransitioning = false;

    void Start()
    {
        if (dogAnimation != null)
        {
            if (sleepClip) dogAnimation.AddClip(sleepClip, "Sleep");
            if (wakeClip)  dogAnimation.AddClip(wakeClip, "Wake");
            if (chaseClip) dogAnimation.AddClip(chaseClip, "Chase");

            dogAnimation.Play("Sleep");
        }
    }

    public void TriggerOuter(Collider2D other)
    {
        if (other.CompareTag("Player") && !isChasing)
            StartCoroutine(WakeUpDog());
    }

    public void TriggerInner(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
            StartCoroutine(PlayTransition());
    }

    IEnumerator WakeUpDog()
    {
        isChasing = true;

        if (wakeClip)
        {
            dogAnimation.Play("Wake");
            yield return new WaitForSeconds(wakeClip.length);
        }

        dogAnimation.Play("Chase");
        StartCoroutine(ChasePlayer());
    }

    IEnumerator ChasePlayer()
    {
        while (isChasing && player)
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
        dogAnimation.Stop();

        var move = player.GetComponent<PlayerMovement>();
        if (move) move.enabled = false;

        if (videoCanvas) videoCanvas.SetActive(true);

        if (videoPlayer)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared) yield return null;

            videoPlayer.Play();
            while (videoPlayer.isPlaying) yield return null;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
