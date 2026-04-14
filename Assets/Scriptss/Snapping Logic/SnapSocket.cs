using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SnapSocket : MonoBehaviour
{
    public string requiredSocket;
    public ComponentType acceptableType;
    public static bool isMotherboardPlaced = false;
    public UnityEvent onSnap;
    private GameObject dockedObject;

    [Header("UI Feedback")]
    public GameObject warningText;
    public GameObject placementHintUI;
    public float displayTime = 2.5f;

    private void Start()
    {
        if (placementHintUI == null)
        {
            placementHintUI = GameObject.Find("PlaceSign");
            if (placementHintUI != null) placementHintUI.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (dockedObject != null) return;

        ComponentTag tag = other.GetComponent<ComponentTag>();
        PartIdentity identity = other.GetComponent<PartIdentity>();
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
                SnapObject(other.gameObject, tag, identity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (placementHintUI != null) placementHintUI.SetActive(false);
    }

    void SnapObject(GameObject obj, ComponentTag tag, PartIdentity identity)
    {
        if (dockedObject != null) return;

        dockedObject = obj;
        if (placementHintUI != null) placementHintUI.SetActive(false);
        
        Quaternion targetRotation = obj.transform.rotation;

        // stop returning to table
        DraggableComponent dragger = obj.GetComponent<DraggableComponent>();
        if (dragger != null)
        {
            dragger.isSnapped = true;
            dragger.enabled = false;
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;

        StartCoroutine(ForceRotationRoutine(obj.transform, targetRotation));

        if (identity != null)
        {
            identity.isInstalled = true;
            obj.transform.localRotation = Quaternion.Euler(identity.rotationOffset);
            identity.ReportSnap();
        }
        else
        {
            obj.transform.localRotation = Quaternion.identity;
        }

        tag.isInstalled = true;
        if (tag.componentType == ComponentType.Motherboard) isMotherboardPlaced = true;
    }

    IEnumerator ForceRotationRoutine(Transform target, Quaternion rot)
    {
        yield return new WaitForEndOfFrame();
        target.rotation = rot;

        yield return null;
        target.rotation = rot;
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