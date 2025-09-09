using UnityEngine;

public class AnimalGoal : MonoBehaviour
{
    public int totalAnimals = 3; // How many must be herded
    private int animalsInside = 0;
    public AnimalQuestNPC npc;   // Reference to NPC Villager

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Animal"))
        {
            animalsInside++;
            CheckCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Animal"))
        {
            animalsInside--;
        }
    }

    void CheckCompletion()
    {
        if (animalsInside >= totalAnimals)
        {
            npc.CompleteQuest();
            Debug.Log("All animals herded! Quest complete.");
        }
    }
}
