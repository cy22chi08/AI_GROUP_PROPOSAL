using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public NPCDialogue npc;
    public Collider2D blockedPath;
    public int animalsNeeded = 3;
    private int animalsInCoop = 0;

    public void AnimalEnteredCoop()
    {
        animalsInCoop++;
        if (animalsInCoop >= animalsNeeded)
        {
            blockedPath.gameObject.SetActive(false); // Open the path
        }
    }
}
