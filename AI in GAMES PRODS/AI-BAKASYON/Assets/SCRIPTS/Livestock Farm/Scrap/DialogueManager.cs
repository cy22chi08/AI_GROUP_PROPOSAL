using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel; // The panel background
    public TMP_Text dialogueText;    // Text inside the panel

    private void Start()
    {
        dialoguePanel.SetActive(false); // Hidden at start
    }

    public void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
