using UnityEngine;

public class CoopTrigger : MonoBehaviour
{
    public QuestManager questManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Animal"))
        {
            questManager.AnimalEnteredCoop();
            Destroy(other.gameObject); // Animal inside coop disappears
        }
    }
}
