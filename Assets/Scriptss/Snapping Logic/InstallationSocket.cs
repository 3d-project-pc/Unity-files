using UnityEngine;

public class InstallationSocket : MonoBehaviour
{
    [Header("Configuration")]
    public ComponentType acceptableType;
    public string requiredSocket;

    [Header("Current State")]
    public ComponentTag installedComponent;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log($"InstallationSocket trigger with: {other.name}"); // ADD THIS

        if (other.TryGetComponent<ComponentTag>(out ComponentTag component))
        {
            if (component.componentType == acceptableType &&
                component.socket == requiredSocket &&
                installedComponent == null)
            {
                Debug.Log($"Compatible - waiting for mouse up"); // ADD THIS

                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Mouse up - calling SnapToSlot"); // ADD THIS
                    SnapToSlot(component);
                }
            }
        }
    }

    void SnapToSlot(ComponentTag component)
    {
        component.transform.position = transform.position;
        component.transform.SetParent(this.transform);
        Debug.Log("=== InstallationSocket.SnapToSlot is RUNNING ===");

        // Apply the predefined snap rotation from PartIdentity if present.
        // Falls back to matching the socket's own rotation if the component
        // has no PartIdentity (e.g. simple props without an offset defined).
        PartIdentity identity = component.GetComponent<PartIdentity>();
        if (identity != null)
        {
            component.transform.rotation = transform.rotation * Quaternion.Euler(identity.rotationOffset);
            identity.isInstalled = true;
            identity.ReportSnap();
        }
        else
        {
            component.transform.rotation = transform.rotation;
        }

        component.isInstalled = true;
        installedComponent = component;

        Debug.Log($"Successfully installed: {component.componentName}");
    }
}
