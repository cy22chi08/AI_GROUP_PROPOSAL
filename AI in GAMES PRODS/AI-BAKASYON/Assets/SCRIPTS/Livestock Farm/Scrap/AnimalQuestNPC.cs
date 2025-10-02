using UnityEngine;

public class AnimalQuestNPC : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject rewardPathBlock; // The collider blocking next path
    private bool questStarted = false;
    private bool questCompleted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!questStarted)
        {
            dialogueManager.ShowDialogue("Villager: My chickens ran away! Please push them back into the coop.");
            questStarted = true;
        }
        else if (questCompleted)
        {
            dialogueManager.ShowDialogue("Villager: Thank you! The path ahead is now open.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueManager.HideDialogue();
        }
    }

    public void CompleteQuest()
    {
        questCompleted = true;
        if (rewardPathBlock != null)
        {
            rewardPathBlock.SetActive(false); // Unlock path
        }
    }
}
