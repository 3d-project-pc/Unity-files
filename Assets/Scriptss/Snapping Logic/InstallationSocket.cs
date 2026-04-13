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
        if (other.TryGetComponent<ComponentTag>(out ComponentTag component))
        {

            if (component.componentType == acceptableType &&
                component.socket == requiredSocket &&
                installedComponent == null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    SnapToSlot(component);
                }
            }
        }
    }

    void SnapToSlot(ComponentTag component)
    {

        component.transform.position = transform.position;
        component.transform.rotation = transform.rotation;

        component.transform.SetParent(this.transform);

        component.isInstalled = true;
        installedComponent = component;

        Debug.Log($"Successfully installed: {component.componentName}");
    }
}