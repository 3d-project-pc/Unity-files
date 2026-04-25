using UnityEngine;

public class ChairSit : MonoBehaviour
{
    [Header("Sit Position")]
    public Transform sitPosition;  // Empty GameObject on the chair seat
    
    [Header("Settings")]
    public float interactionRange = 2f;
    
    void Start()
    {
        // Auto-create sit position if not assigned
        if (sitPosition == null)
        {
            GameObject sitPoint = new GameObject("SitPoint");
            sitPoint.transform.parent = transform;
            sitPoint.transform.localPosition = new Vector3(0, 1f, 0);
            sitPosition = sitPoint.transform;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        if (sitPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(sitPosition.position, 0.3f);
        }
    }
}
