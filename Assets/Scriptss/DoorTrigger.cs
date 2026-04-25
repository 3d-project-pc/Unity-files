using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject interactionUI; // Drag your Text (TMP) here
    public float reachDistance = 4.0f;

    void Update()
    {
        // 1. Shoot a simple ray from the center of the screen
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 2. We check EVERYTHING (no layers) within reachDistance
        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            // 3. Try to find the Door script on the object we hit
            Door door = hit.collider.GetComponent<Door>();

            if (door != null && !door.isOpen)
            {
                interactionUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    door.ToggleDoor();
                    interactionUI.SetActive(false);
                }
            }
            else
            {
                interactionUI.SetActive(false);
            }
        }
        else
        {
            // If the ray hits nothing (the sky/floor), hide UI
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }
}