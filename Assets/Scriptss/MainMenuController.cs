using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioSource buttonSfx;
    public void StartGame()
    {
        SceneManager.LoadScene("AvatarSelection");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    IEnumerator PlaySoundAndLoad()
    {
        if (buttonSfx != null)
        {
            buttonSfx.PlayOneShot(buttonSfx.clip);
        }

        yield return new WaitForSeconds(0.4f);

        SceneManager.LoadScene("AvatarSelection");
    }
    public void OpenSettings()
    {
        Debug.Log("Settings Opened");
    }

    public void ExitGame()
    {
        // Note: This only works in the actual built .exe, not inside the Unity Editor
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

}