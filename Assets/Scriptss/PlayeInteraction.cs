using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float reachDistance = 5f;
    public GameObject openSign;   // "Press E to Open"
    public GameObject readSign;   // "Press B to Read" (Optional)
    public GameObject bookUI;     // Drag your Book Canvas Panel here

    void Update()
    {
        RaycastHit hit;
        // Shoot ray from the center of the camera
        if (Physics.Raycast(transform.position, transform.forward, out hit, reachDistance))
        {
            // --- DOOR LOGIC ---
            if (hit.collider.CompareTag("Door"))
            {
                if (openSign != null) openSign.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DoorController door = hit.collider.GetComponentInParent<DoorController>();
                    if (door != null) door.ToggleDoor();
                }
            }
            // --- BOOK LOGIC ---
            else if (hit.collider.CompareTag("Book"))
            {
                if (readSign != null && !bookUI.activeSelf)
                {
                    readSign.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.B))
                {
                    ToggleBook();
                }
            }
            // If looking at nothing interactable
            else
            {
                DisablePrompts();
            }
        }
        else
        {
            DisablePrompts();
        }
    }

    void DisablePrompts()
    {
        if (openSign != null) openSign.SetActive(false);
        if (readSign != null) readSign.SetActive(false);
    }

    void ToggleBook()
    {
        if (bookUI == null) return;

        bool isActive = !bookUI.activeSelf;
        bookUI.SetActive(isActive);

        if (isActive && readSign != null)
        {
            readSign.SetActive(false);
        }
        // Manage Mouse and Pause
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Pauses game while reading
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // Resumes game
        }
    }
}