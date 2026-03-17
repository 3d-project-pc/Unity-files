using UnityEngine;

public class AvatarSwitcher : MonoBehaviour
{
    public GameObject femaleModel;
    public GameObject maleModel;

    void Start()
    {
        // Check the memory to see who was picked
        if (CharacterData.SelectedAvatarIndex == 1)
        {
            femaleModel.SetActive(true);
            maleModel.SetActive(false);
        }
        else
        {
            femaleModel.SetActive(false);
            maleModel.SetActive(true);
        }
    }
}