using UnityEngine;
using System.Collections;

public class SnapSocket : MonoBehaviour
{
    public string requiredSocket;
    public ComponentType acceptableType;
    public static bool isMotherboardPlaced = false;

    private GameObject dockedObject;

    [Header("UI Feedback")]
    public GameObject warningText;
    public GameObject placementHintUI;
    public float displayTime = 2.5f;

    private void OnTriggerStay(Collider other)
    {
        if (dockedObject != null) return;

        ComponentTag tag = other.GetComponent<ComponentTag>();
        if (tag == null) return;

        bool isCompatible = string.IsNullOrEmpty(requiredSocket) || tag.socket == requiredSocket;

        if (tag.componentType == acceptableType && isCompatible)
        {
            if (tag.componentType != ComponentType.Motherboard && !isMotherboardPlaced)
            {
                if (!Input.GetMouseButton(0)) ShowWarning();
                return;
            }

            if (placementHintUI != null) placementHintUI.SetActive(true);

            if (!Input.GetMouseButton(0))
            {
                SnapObject(other.gameObject, tag);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (placementHintUI != null) placementHintUI.SetActive(false);
    }

    void SnapObject(GameObject obj, ComponentTag tag)
    {
        dockedObject = obj;
        if (placementHintUI != null) placementHintUI.SetActive(false);

        // 1. Tell Draggable to stop returning to table
        DraggableComponent dragger = obj.GetComponent<DraggableComponent>();
        if (dragger != null)
        {
            dragger.isSnapped = true;
            dragger.enabled = false;
        }

        // 2. Parent it
        obj.transform.SetParent(transform);

        // 3. FORCE ALIGNMENT
        // This makes the object match the Socket's position and rotation EXACTLY
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        // 4. Stop Physics movement
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        tag.isInstalled = true;
        if (tag.componentType == ComponentType.Motherboard) isMotherboardPlaced = true;
    }

    void ShowWarning()
    {
        if (warningText != null)
        {
            StopAllCoroutines();
            StartCoroutine(HideWarningAfterTime());
        }
    }

    IEnumerator HideWarningAfterTime()
    {
        warningText.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        warningText.SetActive(false);
    }
}