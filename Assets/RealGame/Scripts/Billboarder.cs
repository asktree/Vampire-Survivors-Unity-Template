using UnityEngine;

public class Billboarder : MonoBehaviour
{
  private Camera mainCamera;

  private void Start()
  {
    // Find the main camera in the scene
    mainCamera = Camera.main;

    // If no main camera is found, log an error
    if (mainCamera == null)
    {
      Debug.LogError("No main camera found in the scene!");
    }
  }

  private void LateUpdate()
  {
    if (mainCamera != null)
    {
      // Make the parent object look at the camera
      //transform.parent.LookAt(mainCamera.transform, Vector3.forward);

      // Rotate the parent object 180 degrees around its up axis
      // This ensures the front of the object faces the camera
      //transform.parent.Rotate(0, 0, 180);

      transform.up = mainCamera.transform.up;
    }
  }
}
