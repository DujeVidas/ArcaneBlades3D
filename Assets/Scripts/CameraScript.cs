using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Sensitivity of the mouse
    public Transform playerBody; // Reference to the player's body transform

    private float xRotation = 0f; // Current X axis rotation

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = GamePreferences.sensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust the xRotation by the mouse Y input (inverted)
        xRotation -= mouseY;
        // Clamp the rotation to prevent flipping the camera
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the vertical rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotate the player body along the Y axis by the mouse X input
        playerBody.Rotate(Vector3.up * mouseX);
    }

    // LateUpdate is called once per frame, after all Update functions have been called
    void LateUpdate()
    {
        // Ensure the camera follows the player's position
        transform.position = playerBody.position;
    }
}
