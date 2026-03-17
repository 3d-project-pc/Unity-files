using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenu : MonoBehaviour
{
    public void SelectMale()
    {
        CharacterData.SelectedAvatarIndex = 0;
        SceneManager.LoadScene("project");
    }

    public void SelectFemale()
    {
        CharacterData.SelectedAvatarIndex = 1;
        SceneManager.LoadScene("project");
    }
}