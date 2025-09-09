using UnityEngine;
using TMPro;

public class IrrigationQuestManager : MonoBehaviour
{
    public NPCDialogue_Irrigation npc;
    public Collider2D blockedPath;
    public GameObject dialogueUI;
    public TMP_Text dialogueText;

    public int totalSwitches = 4;
    private int currentStep = 0;

    private IrrigationSwitch[] switches;

    void Start()
    {
        switches = FindObjectsOfType<IrrigationSwitch>();
    }

    public void TryActivateSwitch(int index, IrrigationSwitch sw)
    {
        if (!npc.QuestAccepted())
        {
            dialogueUI.SetActive(true);
            dialogueText.text = "Talk to the old man first!";
            return;
        }

        if (index == currentStep) // ✅ correct switch
        {
            sw.Activate();
            currentStep++;

            dialogueUI.SetActive(true);
            dialogueText.text = "Switch " + (index + 1) + " activated!";

            if (currentStep >= totalSwitches)
            {
                blockedPath.gameObject.SetActive(false);
                dialogueText.text = "All switches activated! The water flows to the crops.";
                Debug.Log("Irrigation quest completed.");
            }
        }
        else // ❌ wrong switch → reset all
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
