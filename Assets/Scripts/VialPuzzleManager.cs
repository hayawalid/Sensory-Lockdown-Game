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

    private List<VialClickMover> sequence = new List<VialClickMover>();
    private int playerIndex = 0;
    private bool playerTurn = false;

    void Start()
    {
        GenerateSequence();
        StartCoroutine(PlaySequence());
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
        if (!playerTurn)
            return;

        if (vial == sequence[playerIndex])
        {
            // correct
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                Debug.Log("Puzzle Completed!");
                playerTurn = false;
            }
        }
        else
        {
            Debug.Log("Wrong vial!");
            playerTurn = false;
        }
    }

    public void ReplaySequence()
    {
        StartCoroutine(PlaySequence());
    }
}
