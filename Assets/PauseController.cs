using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering; // Required for the Blur Volume

public class PauseController : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject pauseMenuUI; // Drag your 'Background' panel here
    public Volume blurVolume;      // Drag your 'Global Volume' with the blur here

    private bool isPaused = false;

    void Start()
    {
        // Ensure the menu and blur are off when the game starts
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (blurVolume != null) blurVolume.enabled = false;

        // Ensure time is running and cursor is locked for gameplay
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Allows toggling with the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // This is the function you link to your Small HUD Button
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (blurVolume != null) blurVolume.enabled = false;

        Time.timeScale = 1f; // Unfreeze game
        isPaused = false;

        // Hide and lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (blurVolume != null) blurVolume.enabled = true;

        Time.timeScale = 0f; // Freeze game
        isPaused = true;

        // Show cursor so you can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // ALWAYS reset time before changing scenes
        SceneManager.LoadScene("Hallway Scene"); // Make sure this matches your scene name EXACTLY
    }

    public void QuitGame()
    {
        Debug.Log("Exiting Game...");
        Application.Quit(); // Note: This only works in the final build (.exe), not the Editor
    }
}