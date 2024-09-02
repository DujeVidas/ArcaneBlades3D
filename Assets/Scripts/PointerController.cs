using UnityEngine;

public class PointerController : MonoBehaviour
{
    public Transform pointA; // Reference to the starting point
    public Transform pointB; // Reference to the ending point
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    public float moveSpeed = 100f; // Speed of the pointer movement

    public RectTransform pointerTransform;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        targetPosition = pointB.position;
    }

    void Update()
    {
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    void CheckSuccess()
    {
        // Check if the pointer is within the safe zone
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
        {
            Debug.Log("Success!");
        }
        else
        {
            Debug.Log("Fail!");
        }
    }

    public void SetPointerPosition(float progress)
    {
        // Interpolate the position based on progress
        pointerTransform.position = Vector3.Lerp(pointA.position, pointB.position, progress);
    }
}
