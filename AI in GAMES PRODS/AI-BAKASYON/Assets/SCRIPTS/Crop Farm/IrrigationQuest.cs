using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class IrrigationQuestManager : MonoBehaviour
{
    public NPCDialogue_Irrigation npc;
    public Collider2D blockedPath;
    public Tilemap irrigationOn; // ✅ Changed from Collider2D to Tilemap
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    public int totalSwitches = 4;
    private int currentStep = 0;

    private IrrigationSwitch[] switches;

    void Start()
    {
        switches = FindObjectsOfType<IrrigationSwitch>();
        irrigationOn.GetComponent<TilemapRenderer>().enabled = false; // Start hidden
    }

    public void TryActivateSwitch(int index, IrrigationSwitch sw)
    {
        if (!npc.QuestAccepted())
        {
            dialogueUI.SetActive(true);
            dialogueText.text = "Talk to the old man first!";
            return;
        }

        if (index == currentStep)
        {
            sw.Activate();
            currentStep++;

            dialogueUI.SetActive(true);
            dialogueText.text = "Switch " + (index + 1) + " activated!";

            if (currentStep >= totalSwitches)
            {
                blockedPath.gameObject.SetActive(false);
                irrigationOn.GetComponent<TilemapRenderer>().enabled = true; // ✅ Show tilemap
                dialogueText.text = "All switches activated! The water flows to the crops.";
                Debug.Log("Irrigation quest completed.");
            }
        }
        else
        {
            currentStep = 0;
            dialogueUI.SetActive(true);
            dialogueText.text = "Wrong switch! Start again from the first one.";

            foreach (var s in switches)
            {
                s.ResetSwitch();
            }
        }
    }
}
