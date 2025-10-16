using UnityEngine;
using TMPro;

public class SimpleNPCDialogue : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    [Header("Dialogue Settings")]
    [TextArea]
    public string dialogueLine = "Hello there, Poy! Never give up!";

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueUI.activeSelf)
            {
                dialogueUI.SetActive(true);
                dialogueText.text = dialogueLine;
            }
            else
            {
                dialogueUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            dialogueUI.SetActive(false);
        }
    }
}
