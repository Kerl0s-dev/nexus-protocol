using UnityEngine;
using MiniTween.CameraTween;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, 0);
    
    public float sensitivity = 40f;

    float xRotation, yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in CameraMovement script.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = InputManager.Look.x * sensitivity * Time.deltaTime;
        float mouseY = InputManager.Look.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        playerTransform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void LateUpdate()
    {
        // --- POSITION ---
        Vector3 desiredPosition = playerTransform.position + offset;

        // Appliquer le shake
        Vector3 shakeOffset = CameraExtensions.GetShakeOffset();
        transform.position = desiredPosition + shakeOffset;
    }
}