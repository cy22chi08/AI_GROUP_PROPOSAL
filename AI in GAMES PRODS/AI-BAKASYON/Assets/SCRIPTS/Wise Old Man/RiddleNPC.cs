using UnityEngine;
using TMPro;

public class RiddleNPC : MonoBehaviour
{
    [System.Serializable]
    public class Riddle
    {
        [TextArea] public string question;
        public string answer; // keep lowercase for comparison
    }

    public GameObject dialogueUI;
    public TMP_Text dialogueText;
    public TMP_InputField answerInput;
    public Riddle[] riddles;

    public GameObject blockedPath; //  assign your blocked path object here

    private int currentRiddle = 0;
    private bool playerNear = false;
    private bool riddleActive = false;

    // 🔹 ADDED: reference to player movement script
    private PlayerMovement playerMovement; 

    void Start()
    {
        // 🔹 ADDED: find the player’s movement script (change "Player" if your tag is different)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E) && !riddleActive)
        {
            AskRiddle();
        }
    }

    void AskRiddle()
    {
        if (currentRiddle < riddles.Length)
        {
            dialogueUI.SetActive(true);
            dialogueText.text = riddles[currentRiddle].question;
            answerInput.gameObject.SetActive(true);
            answerInput.text = "";
            riddleActive = true;

            // 🔹 ADDED: stop player movement while riddle is active
            if (playerMovement != null) playerMovement.enabled = false;

            // Listen for enter/submit
            answerInput.onSubmit.AddListener(CheckAnswer);
        }
        else
        {
            dialogueText.text = "You have solved all my riddles! Well done, traveler.";
            if (blockedPath != null)
            {
                blockedPath.SetActive(false); // unblock the path
            }
        }
    }

    void CheckAnswer(string playerAnswer)
    {
        answerInput.onSubmit.RemoveListener(CheckAnswer); // avoid stacking
        playerAnswer = playerAnswer.Trim().ToLower();

        if (playerAnswer == riddles[currentRiddle].answer.ToLower())
        {
            dialogueText.text = "Eto";
            currentRiddle++;
            riddleActive = false;

            // 🔹 ADDED: re-enable player movement
            if (playerMovement != null) playerMovement.enabled = true;

            if (currentRiddle < riddles.Length)
            {
                Invoke(nameof(AskRiddle), 1.5f);
            }
            else
            {
                dialogueText.text = "You answered all riddles. The path ahead is yours!";
                if (blockedPath != null)
                {
                    blockedPath.SetActive(false); // remove the blocked path here too
                }
            }
        }
        else
        {
            dialogueText.text = "Wrong! Try again.";
            Invoke(nameof(AskRiddle), 1.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            dialogueUI.SetActive(false);
            answerInput.gameObject.SetActive(false);
            riddleActive = false;

            // 🔹 ADDED: re-enable movement if player leaves
            if (playerMovement != null) playerMovement.enabled = true;
        }
    }

    public bool AllRiddlesSolved()
    {
        return currentRiddle >= riddles.Length;
    }
}
