using UnityEngine;
using TMPro;
using System.Collections;

public class InstallationGuide : MonoBehaviour
{
    public static InstallationGuide Instance;
    public TextMeshProUGUI guideText;
    public float displayTime = 3f;

    // ADD THIS BLOCK
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // Changed from PartIdentity to PartIdentityV2
        PartIdentityV2.OnComponentSnapped += ShowSuccessMessage;
    }

    void OnDisable()
    {
        // Changed from PartIdentity to PartIdentityV2
        PartIdentityV2.OnComponentSnapped -= ShowSuccessMessage;
    }

    public void ShowSuccessMessage(string message)
    {
        if (guideText == null) return; 
        StopAllCoroutines();
        float calculatedTime = 1f + (message.Length * 0.05f);
        float finalTime = Mathf.Clamp(calculatedTime, 3f, 10f);
        StartCoroutine(DisplayRoutine(message, finalTime));
    }

    IEnumerator DisplayRoutine(string msg, float time)
    {
        guideText.text = msg;
        guideText.enabled = true;

        yield return new WaitForSeconds(time);

        guideText.enabled = false;
        guideText.text = "";
    }

    public void ClearMessage()
    {
        if (guideText != null)
        {
            StopAllCoroutines();
            guideText.enabled = false;
            guideText.text = "";
        }
    }
}