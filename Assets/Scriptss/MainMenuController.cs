using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // Loads the next scene in the list
        SceneManager.LoadScene("AvatarSelection");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettings()
    {
        // We will build this panel later!
        Debug.Log("Settings Opened");
    }

    public void ExitGame()
    {
        // Note: This only works in the actual built .exe, not inside the Unity Editor
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

}