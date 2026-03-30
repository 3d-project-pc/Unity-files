using UnityEngine;
using UnityEngine.UI;

public class ModelSwapper : MonoBehaviour
{
    [Header("3D Viewer Setup")]
    public Camera modelCamera;           // Camera that renders the 3D model
    public RawImage modelDisplay;        // Raw Image that shows the Render Texture
    public Transform modelHolder;        // Empty GameObject where models will be placed
    public RenderTexture renderTexture;  // Render Texture for the camera

    [Header("Model Settings")]
    public Vector3 modelRotation = new Vector3(0, 45, 0);
    public Vector3 modelPosition = new Vector3(0, 0, 0);
    public Vector3 modelScale = new Vector3(2, 2, 2);

    private GameObject currentModel;

    void Start()
    {
        // Set up the camera to use the render texture if not already configured
        if (modelCamera != null && renderTexture != null)
        {
            modelCamera.targetTexture = renderTexture;
        }

        // Assign the render texture to the raw image
        if (modelDisplay != null && renderTexture != null)
        {
            modelDisplay.texture = renderTexture;
        }

        // Optionally set a default/placeholder model
        // SetDefaultModel();
    }

    public void SwapModel(GameObject newModelPrefab)
    {
        // Destroy the current model if it exists
        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        // Instantiate the new model
        if (newModelPrefab != null && modelHolder != null)
        {
            currentModel = Instantiate(newModelPrefab, modelHolder);
            currentModel.transform.localPosition = modelPosition;
            currentModel.transform.localRotation = Quaternion.Euler(modelRotation);
            currentModel.transform.localScale = Vector3.Scale(currentModel.transform.localScale, modelScale);
            currentModel.AddComponent<SimpleRotator>();
            currentModel.AddComponent<ModelRotation>();


            Debug.Log("Model swapped to: " + newModelPrefab.name);
        }
        else if (newModelPrefab == null)
        {
            Debug.LogWarning("Attempted to swap to a null model prefab");
        }
    }

    public void ClearModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
            Debug.Log("Model cleared");
        }
    }

    // Optional: Add rotation controls
    public void RotateModel(float xDelta, float yDelta)
    {
        if (currentModel != null)
        {
            currentModel.transform.Rotate(Vector3.up, -xDelta, Space.World);
            currentModel.transform.Rotate(Vector3.right, yDelta, Space.World);
        }
    }

    // Optional: Reset model rotation
    public void ResetModelRotation()
    {
        if (currentModel != null)
        {
            currentModel.transform.localRotation = Quaternion.Euler(modelRotation);
        }
    }
}