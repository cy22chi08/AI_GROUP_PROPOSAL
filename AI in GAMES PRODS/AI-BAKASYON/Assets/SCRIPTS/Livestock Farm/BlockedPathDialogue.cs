using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlockedPathDialogue : MonoBehaviour
{
    public NPCDialogue npc;
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !npc.QuestAccepted())
        {
            dialogueUI.SetActive(true);
            dialogueText.text = "Won't you help an old lady with her farm?";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            dialogueUI.SetActive(false);
        }
    }
}
