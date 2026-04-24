using UnityEngine;

public class BookInteractable : MonoBehaviour
{
    public GameObject bookUI; // Drag your BookUI panel here in the Inspector
    private bool isPlayerNearby = false;

    void Update()
    {
        Debug.Log("Standing near the book!");
        // Check if player is close AND presses 'B'
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B key pressed while near book!");
            ToggleBook();
        }
    }
    void ToggleBook()
    {
        bool isActive = bookUI.activeSelf;
        bookUI.SetActive(!isActive);

        // Optional: Lock/Unlock cursor when reading
        if (!isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Optional: Pause the game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Player entered the book zone!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            bookUI.SetActive(false); // Close book if they walk away
            Time.timeScale = 1f;
        }
    }
}