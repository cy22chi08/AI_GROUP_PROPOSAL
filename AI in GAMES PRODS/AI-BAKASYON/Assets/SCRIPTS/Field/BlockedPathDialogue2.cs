using UnityEngine;
using TMPro;

public class BlockedPathDialogue_Irrigation : MonoBehaviour
{
    public NPCDialogue_Irrigation npc;
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !npc.QuestAccepted())
        {
            dialogueUI.SetActive(true);
            dialogueText.text = "Won't you help that old man with his crops?";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) dialogueUI.SetActive(false);
    }
}
