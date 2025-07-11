using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private InputSystem_Actions controls; // Référence au système d'Input

    public float sensitivity = .01f;
    public Transform playerBody; // L'objet que la caméra suit, souvent le "Player"

    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Awake()
    {
        // Instancier le système d'Input
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable(); // Activer l'Action Map "Player"
        controls.Player.Look.performed += OnLook;
    }

    private void OnDisable()
    {
        controls.Player.Look.canceled -= OnLook;
        controls.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        xRotation -= lookInput.y * sensitivity;
        xRotation = Mathf.Clamp(xRotation, -15f, 15f);

        yRotation += lookInput.x * sensitivity;
        yRotation = Mathf.Clamp(yRotation, -15f, 15f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    // Appelé par l'action "Look" du Input System
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
        Debug.Log("Mouse movement: " + lookInput);
    }
}
