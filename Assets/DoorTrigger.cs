using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Needed for TextMeshPro

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Laboratory Scene"; // Set this in the Inspector
    [SerializeField] private TextMeshProUGUI promptText; // Drag your InteractPrompt here

    private bool playerIsNear = false;

    // --- DETECTION LOGIC (When the Player Enters/Exits the Trigger) ---

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player (based on Tag)
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            // Show the "Press E" prompt
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting is the player
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
            // Hide the "Press E" prompt
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    // --- INPUT LOGIC (Checking for the 'E' Key Press) ---

    void Update()
    {
        // Only check for input if the player is currently inside the trigger
        if (playerIsNear)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Checks for the 'E' key press
            {
                LoadNextScene();
            }
        }
    }

    // --- SCENE LOADING FUNCTION ---

    void LoadNextScene()
    {
        // Make sure the scene is added to the Build Settings!
        SceneManager.LoadScene(sceneToLoad);
    }
}