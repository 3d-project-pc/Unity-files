using UnityEngine;
using System.Collections;

public class SnapSocketV2 : MonoBehaviour
{
    [Header("Socket Settings")]
    public ComponentType acceptedType;
    public bool requireMotherboardFirst = false;

    [Header("Sequential Setup (for multiple identical sockets)")]
    public SnapSocketV2 nextSocket;           // Generic: works for RAM, Fans, Storage, etc.
    private SphereCollider socketCollider;

    [Header("Visual Feedback")]
    public GameObject highlightObject;
    public GameObject hintCanvas;
    public float hintDisplayTime = 1.5f;

    private GameObject dockedObject;
    private PartIdentityV2 pendingPart;
    private DraggableComponent pendingDragger;

    void Start()
    {
        socketCollider = GetComponent<SphereCollider>();
        if (socketCollider == null)
            Debug.LogError($"SnapSocketV2 on {gameObject.name} needs a SphereCollider");

        if (highlightObject) highlightObject.SetActive(false);
        if (hintCanvas) hintCanvas.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (dockedObject != null) return;

        PartIdentityV2 part = other.GetComponent<PartIdentityV2>();
        if (part == null || part.isInstalled) return;
        if (part.componentType != acceptedType) return;

        // Only check motherboard prerequisite if:
        // 1. This socket requires it (e.g., cooler socket)
        // 2. AND the part being installed is NOT the motherboard itself
        if (requireMotherboardFirst && acceptedType != ComponentType.Motherboard && !SnapSocketV2.IsMotherboardPlaced())
        {
            ShowTemporaryHint("Install motherboard first!");
            return;
        }

        pendingPart = part;
        pendingDragger = other.GetComponent<DraggableComponent>();

        if (highlightObject) highlightObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (dockedObject != null) return;

        PartIdentityV2 part = other.GetComponent<PartIdentityV2>();
        if (part == null) return;

        if (part == pendingPart)
        {
            if (highlightObject) highlightObject.SetActive(false);
            pendingPart = null;
            pendingDragger = null;
        }
    }

    void Update()
    {
        if (pendingPart != null && pendingDragger != null && Input.GetMouseButtonUp(0))
        {
            if (pendingDragger.isSnapped) return;
            if (pendingPart.isInstalled) return;

            SnapObject(pendingPart.gameObject, pendingPart);
        }
    }

    void SnapObject(GameObject obj, PartIdentityV2 part)
    {
        if (dockedObject != null) return;

        dockedObject = obj;

        if (pendingDragger != null)
        {
            pendingDragger.isSnapped = true;
            pendingDragger.enabled = false;
        }

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.transform.SetParent(transform);
        obj.transform.localPosition = part.positionOffset;
        obj.transform.localRotation = Quaternion.Euler(part.rotationOffset);

        part.isInstalled = true;
        part.ReportSnap();

        if (highlightObject) highlightObject.SetActive(false);
        if (hintCanvas) hintCanvas.SetActive(false);

        // ✅ GENERIC SEQUENTIAL LOGIC - works for RAM, Fans, Storage, etc.
        if (nextSocket != null)
        {
            // Disable this socket's collider
            if (socketCollider != null)
                socketCollider.enabled = false;
            
            // Enable the next socket in sequence
            nextSocket.EnableSocket();
            
            Debug.Log($"Socket {gameObject.name} filled. Enabling next socket: {nextSocket.gameObject.name}");
        }

        if (acceptedType == ComponentType.Motherboard)
            SetMotherboardPlaced(true);

        pendingPart = null;
        pendingDragger = null;
    }

    void ShowTemporaryHint(string message)
    {
        if (hintCanvas == null) return;

        var text = hintCanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (text != null) text.text = message;

        hintCanvas.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideHintAfterDelay());
    }

    IEnumerator HideHintAfterDelay()
    {
        yield return new WaitForSeconds(hintDisplayTime);
        if (hintCanvas != null) hintCanvas.SetActive(false);
    }

    public void EnableSocket()
    {
        if (socketCollider != null)
            socketCollider.enabled = true;
        
        Debug.Log($"Socket enabled: {gameObject.name}");
    }

    private static bool motherboardPlaced = false;
    public static bool IsMotherboardPlaced() => motherboardPlaced;
    private static void SetMotherboardPlaced(bool value) => motherboardPlaced = value;
}