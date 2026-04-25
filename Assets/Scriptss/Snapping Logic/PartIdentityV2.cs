using UnityEngine;
using System;

public class PartIdentityV2 : MonoBehaviour
{
    [Header("Part Information")]
    public ComponentType componentType;
    [TextArea(2,4)]
    public string guideMessage;
    
    [Header("Snap Offsets (relative to socket)")]
    public Vector3 positionOffset = Vector3.zero;   // local position offset after snapping
    public Vector3 rotationOffset = Vector3.zero;   // local rotation offset after snapping

    [Header("Installation")]
    public bool isInstalled = false;

    public static Action<string> OnComponentSnapped;

    public void ReportSnap()
    {
        string successMsg = $"{componentType} installed successfully!";
        OnComponentSnapped?.Invoke(successMsg);
    }
}