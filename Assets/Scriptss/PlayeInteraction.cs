using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float reachDistance = 5f; // Increased distance
    public GameObject openSign;

    void Update()
    {
        RaycastHit hit;
        // This shoots the ray
        if (Physics.Raycast(transform.position, transform.forward, out hit, reachDistance))
        {
            // THIS LINE IS THE FIX: It looks for the tag on the mesh OR the parent
            if (hit.collider.CompareTag("Door"))
            {
                if (openSign != null) openSign.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Find the script on the parent (DoorPivot)
                    DoorController door = hit.collider.GetComponentInParent<DoorController>();
                    if (door != null) door.ToggleDoor();
                }
            }
            else
            {
                if (openSign != null) openSign.SetActive(false);
            }
        }
        else
        {
            if (openSign != null) openSign.SetActive(false);
        }
    }
}