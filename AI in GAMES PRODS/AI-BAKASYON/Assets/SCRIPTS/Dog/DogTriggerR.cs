using UnityEngine;

public class DogOuterTrigger : MonoBehaviour
{
    public DogNPC dogNPC;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dogNPC != null)
            dogNPC.TriggerOuter(other);
    }
}
