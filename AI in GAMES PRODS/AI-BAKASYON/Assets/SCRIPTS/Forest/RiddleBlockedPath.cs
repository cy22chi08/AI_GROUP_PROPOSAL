using UnityEngine;
using TMPro;

public class BlockedPathRiddle : MonoBehaviour
{
    public RiddleNPC npc;                // Reference to the Riddle NPC
    public GameObject dialogueUI;        // Dialogue UI panel
    public TMP_Text dialogueText;        // TMP dialogue text

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (!npc.AllRiddlesSolved()) // check if riddles are done
            {
                dialogueUI.SetActive(true);
                dialogueText.text = "Won’t you answer the old man’s riddles first?";
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerNear = true;
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
