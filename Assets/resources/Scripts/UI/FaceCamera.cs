using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [Header("Position Settings")]
    public float distanceFromFace = 1.2f; // Target distance
    public float heightOffset = -0.1f;    // Slightly below eye level
    public float smoothSpeed = 5.0f;      // How fast it catches up (higher = snappier)
    
    [Header("Behavior")]
    public bool lockYAxis = true;         // Keep vertical (true) or tilt with head (false)
    public bool followContinuously = true; // Uncheck this if you only want it to jump once

    private void LateUpdate()
    {
        // 1. Safety Check: Find the camera every frame
        if (Camera.main == null) return;
        Transform cam = Camera.main.transform;

        // 2. Calculate the Goal Position
        Vector3 targetPos = cam.position;
        
        // Get forward direction
        Vector3 forward = cam.forward;
        if (lockYAxis) forward.y = 0; // Flatten on ground plane
        forward.Normalize();

        // Project point in front of camera
        targetPos += forward * distanceFromFace;
        targetPos.y += heightOffset;

        // 3. Move the Canvas
        if (followContinuously)
        {
            // Smoothly fly to the target position
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // SNAP instantly (good for debugging)
            transform.position = targetPos;
        }

        // 4. Rotate to Face Camera
        if (lockYAxis)
        {
            Vector3 lookPos = transform.position + cam.rotation * Vector3.forward;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }
        else
        {
            transform.LookAt(transform.position + cam.rotation * Vector3.forward);
        }
    }
}