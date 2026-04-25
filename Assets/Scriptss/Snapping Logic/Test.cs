using UnityEngine;

public class TestSocketDebug : MonoBehaviour
{
    public ComponentType expectedType = ComponentType.Motherboard;
    
    void OnTriggerEnter(Collider other)
    {
        // Only respond to the motherboard
        PartIdentityV2 part = other.GetComponent<PartIdentityV2>();
        if (part == null) return;
        
        if (part.componentType == expectedType)
        {
            // Turn the socket red to show detection
            GetComponent<Renderer>().material.color = Color.red;
            Debug.Log($"✅ MOTHERBOARD DETECTED! Name: {other.gameObject.name}");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        PartIdentityV2 part = other.GetComponent<PartIdentityV2>();
        if (part != null && part.componentType == expectedType)
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
}