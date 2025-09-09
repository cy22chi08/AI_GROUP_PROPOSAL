using UnityEngine;
using TMPro;

public class NPCDialogue_Irrigation : MonoBehaviour
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
                dialogueText.text = 
                    "Hello there Toy! I need help with my water irrigation.\n" +
                    "Can you help me with that? My knees are killing me.";
                questAccepted = true;
                Debug.Log("Irrigation Quest accepted.");
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

    public bool QuestAccepted() => questAccepted;
}
