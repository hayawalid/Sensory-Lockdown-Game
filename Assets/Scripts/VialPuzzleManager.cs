using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VialPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public List<VialClickMover> allVials;   // assign all 12 vials here
    public int sequenceLength = 3;
    public float glowDuration = 0.7f;
    public float glowDelay = 0.3f;
    public BowlFlash bowlFlash;

    private List<VialClickMover> sequence = new List<VialClickMover>();
    private int playerIndex = 0;
    private bool playerTurn = false;
    public ColorFeedbackManager feedbackManager;

    void Start()
    {
        GenerateSequence();
       // StartCoroutine(PlaySequence());
    }

    void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < sequenceLength; i++)
        {
            int r = Random.Range(0, allVials.Count);
            sequence.Add(allVials[r]);
        }
    }

    IEnumerator PlaySequence()
    {
        playerTurn = false;
        playerIndex = 0;

        foreach (var vial in sequence)
        {
            VialGlow glow = vial.GetComponent<VialGlow>();
            glow.GlowOn();
            yield return new WaitForSeconds(glowDuration);
            glow.GlowOff();
            yield return new WaitForSeconds(glowDelay);
        }

        playerTurn = true;
    }

    public void PlayerSelected(VialClickMover vial)
    {
        if (!playerTurn) return;

        // ✅ Correct vial
        if (vial == sequence[playerIndex])
        {
            playerIndex++;
            // 2. Call the AI feedback for a correct move
            if(feedbackManager != null) 
                feedbackManager.OnCorrectSelection(playerIndex, sequence.Count);

            // ✅ WIN CONDITION
            if (playerIndex >= sequence.Count)
            {
                Debug.Log("Puzzle Completed!");
                playerTurn = false;
                OnPuzzleWin();
            }
        }
        else
        {
            Debug.Log("Wrong vial!");
            // 3. Call the AI feedback for a wrong move
            if(feedbackManager != null) 
                feedbackManager.OnWrongSelection();
            
            playerTurn = false;
            OnPuzzleFail();
        }
    }

    // 4. Reset color when replaying
    public void ReplaySequence()
    {
        GenerateSequence();

        if(feedbackManager != null) feedbackManager.ResetToGrayscale();
        StartCoroutine(PlaySequence());
    }

    // ✅ Called when the player wins
    void OnPuzzleWin()
    {
        Debug.Log("PLAYER WON THE PUZZLE!");
        if (bowlFlash != null) 
            bowlFlash.Flash();

        // Add anything you want here:
        // - Play animation
        // - Show UI
        // - Unlock next step
        // - Trigger bowl effect
    }

    // ✅ Called when the player fails
    void OnPuzzleFail()
    {
        Debug.Log("PLAYER FAILED THE PUZZLE!");

        // Optional:
        // - Flash red
        // - Reset puzzle
        // - Allow replay
    }
}

