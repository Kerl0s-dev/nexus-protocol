using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    PlayerInput playerInput;

    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction dashAction;
    InputAction toggleConsoleAction;

    public static Vector2 Move { get; private set; }
    public static Vector2 Look { get; private set; }
    public static bool Jump { get; private set; }
    public static bool Dash { get; private set; }
    public static bool ToggleConsole { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        // On récupère les actions une fois
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Sprint"];
        toggleConsoleAction = playerInput.actions["ToggleConsole"];
    }

    // Update is called once per frame
    void Update()
    {
        Move = moveAction.ReadValue<Vector2>();
        Look = lookAction.ReadValue<Vector2>();
        Jump = jumpAction.IsPressed();
        Dash = dashAction.WasPressedThisFrame();
        ToggleConsole = toggleConsoleAction.WasPressedThisFrame();
    }
}
