using UnityEngine;
using System;

public class PartIdentity : MonoBehaviour
{
    public ComponentType componentType;
    public bool isInstalled = false;
    [TextArea]
    public string guideMessage;
    public Vector3 rotationOffset;


    public static Action<string> OnComponentSnapped;

    public void ReportSnap()
    {
        // Shouts to the InstallationGuide script
        OnComponentSnapped?.Invoke(componentType.ToString() + " is in place!");
    }
}