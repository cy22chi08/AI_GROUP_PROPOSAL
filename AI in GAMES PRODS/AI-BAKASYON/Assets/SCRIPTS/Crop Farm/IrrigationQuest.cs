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
            dialogueText.text = "Alam mo ba gagawin dito? Kausapin mo sa Tatang Boy!";
            return;
        }

        if (index == currentStep)
        {
            sw.Activate();
            currentStep++;

            dialogueUI.SetActive(true);
            dialogueText.text = "Daluyan number " + (index + 1) + " bukas na!";

            if (currentStep >= totalSwitches)
            {
                blockedPath.gameObject.SetActive(false);
                irrigationOn.GetComponent<TilemapRenderer>().enabled = true; // ✅ Show tilemap
                dialogueText.text = "Lahat ng daluyan ng tubig ay bukas na!";
                Debug.Log("Tapos ka na dito.");
            }
        }
        else
        {
            currentStep = 0;
            dialogueUI.SetActive(true);
            dialogueText.text = "Mali Poy, baka malunod ang mga pananim kapag inuna mo yan!.";

            foreach (var s in switches)
            {
                s.ResetSwitch();
            }
        }
    }
}
