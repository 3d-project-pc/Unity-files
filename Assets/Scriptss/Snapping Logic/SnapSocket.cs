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
        //Debug.Log($"Trigger stay with: {other.name}"); // ADD THIS

        if (dockedObject != null) return;

        ComponentTag tag = other.GetComponent<ComponentTag>();
        PartIdentity identity = other.GetComponent<PartIdentity>();
        if (tag == null) return;

        bool isCompatible = string.IsNullOrEmpty(requiredSocket) || tag.socket == requiredSocket;

        if (tag.componentType == acceptableType && isCompatible)
        {
            Debug.Log($"Compatible component detected: {tag.componentType}"); // ADD THIS

            if (tag.componentType != ComponentType.Motherboard && !isMotherboardPlaced)
            {
                if (!Input.GetMouseButton(0)) ShowWarning();
                return;
            }

            if (placementHintUI != null) placementHintUI.SetActive(true);

            if (!Input.GetMouseButton(0))
            {
                Debug.Log("About to call SnapObject"); // ADD THIS
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
        //Debug.Log($"=== SNAP OBJECT CALLED for: {obj.name} ===");
        Debug.Log($"=== SNAPPING TO SOCKET: {gameObject.name} ===");
        Debug.Log($"Socket position: {transform.position}");
        Debug.Log($"Socket parent: {transform.parent?.name}");

        if (dockedObject != null) return;

        dockedObject = obj;
        if (placementHintUI != null) placementHintUI.SetActive(false);

        // Stop the component from returning to table
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

        // DEBUG: Check values before applying rotation
        Debug.Log($"Socket transform rotation: {transform.rotation.eulerAngles}");
        Debug.Log($"Socket local rotation: {transform.localRotation.eulerAngles}");

        if (identity != null)
        {
            Debug.Log($"PartIdentity exists! rotationOffset = {identity.rotationOffset}");
            Debug.Log($"Applying localRotation = Quaternion.Euler({identity.rotationOffset})");

            obj.transform.rotation = Quaternion.Euler(identity.rotationOffset);

            Debug.Log($"Result localRotation: {obj.transform.localRotation.eulerAngles}");
            Debug.Log($"Result world rotation: {obj.transform.rotation.eulerAngles}");

            identity.isInstalled = true;
            identity.ReportSnap();
        }
        else
        {
            Debug.LogWarning("No PartIdentity component found on this object!");
            obj.transform.localRotation = Quaternion.identity;
        }

        tag.isInstalled = true;
        if (tag.componentType == ComponentType.Motherboard) isMotherboardPlaced = true;

        onSnap?.Invoke();
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
