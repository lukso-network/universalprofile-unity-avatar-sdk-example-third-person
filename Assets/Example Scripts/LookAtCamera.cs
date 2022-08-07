using UnityEngine;

/// <summary>
/// Simple script to make an object face the camera. Used for making the loading avatar percentage face the camera.
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    public Transform cameraTarget;

    void Update()
    {
        transform.LookAt(cameraTarget);
    }
}
