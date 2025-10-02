using UnityEngine;

public class DogTriggerRelay : MonoBehaviour
{
    public string triggerType; // "Outer" or "Inner"
    private DogNPC dog;

    void Start()
    {
        dog = GetComponentInParent<DogNPC>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dog != null)
            dog.OnDogTrigger(triggerType, other);
    }
}
