using UnityEngine;

public class IrrigationSwitch : MonoBehaviour
{
    public int switchIndex; // 0 = first, 1 = second, etc.
    public IrrigationQuestManager questManager;
    private bool playerNear = false;

    private SpriteRenderer sr;
    private Color defaultColor = Color.gray;
    private Color activeColor = Color.green;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = defaultColor; // start as "off"
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            questManager.TryActivateSwitch(switchIndex, this);
        }
    }

    public void Activate()
    {
        sr.color = activeColor;
    }

    public void ResetSwitch()
    {
        sr.color = defaultColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerNear = false;
    }
}
