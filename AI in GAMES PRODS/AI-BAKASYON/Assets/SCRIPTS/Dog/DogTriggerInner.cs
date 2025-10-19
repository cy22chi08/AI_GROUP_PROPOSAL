using UnityEngine;

public class DogInnerTrigger : MonoBehaviour
{
    public DogNPC dogNPC;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (dogNPC != null)
            dogNPC.TriggerInner(other);
    }
}
