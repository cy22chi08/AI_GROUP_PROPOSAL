using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    private bool playerNear = false;
    private bool questAccepted = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!questAccepted)
            {
                dialogueUI.SetActive(true);
                dialogueText.text = "Hey Totoy, Help me push the animals into the coop.";
                questAccepted = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            dialogueUI.SetActive(false);
        }
    }

    public bool QuestAccepted() => questAccepted;
}
