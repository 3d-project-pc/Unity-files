using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CrosshairInteractor : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask uiLayer;

    void Update()
    {
        // 1. Always start the ray from the center of the CAMERA's view
        // This creates a ray from the Camera lens through the middle of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        float interactDistance = 5f;

        // 2. Draw the ray so we can see it in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {

            Debug.Log("Looking at: " + hit.collider.gameObject.name);

            if (Input.GetMouseButtonDown(0)) // Left Click
            {
                Button btn = hit.collider.GetComponentInParent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
            }
        }
    }
}