using UnityEngine;
using TMPro; 

public class QualityManager : MonoBehaviour
{
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        Debug.Log("Quality changed to index: " + qualityIndex);
    }
}