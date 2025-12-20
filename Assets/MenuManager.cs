using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject mainMenuPanel;
    public GameObject pauseButton; // NEW: Drag your small HUD Pause Button here

    [Header("Player Settings")]
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        mainMenuPanel.SetActive(true);

        // Ensure the small pause button is hidden while on the main menu
        if (pauseButton != null) pauseButton.SetActive(false);

        if (playerMovementScript != null) playerMovementScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);

        // REVEAL the pause button now that the game has started
        if (pauseButton != null) pauseButton.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}