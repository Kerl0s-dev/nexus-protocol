using UnityEngine;
using UnityEngine.UIElements;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance;

    public UIDocument consoleUI;
    public VisualElement consoleRoot;
    public TextField inputField;
    public Button submitButton;

    public ScrollView logContainer; // Container for log messages

    public bool isConsoleOpen = false;

    [HideInInspector] public CharacterMovement playerCtrl;
    [HideInInspector] public CameraMovement cameraCtrl;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        cameraCtrl = Camera.main.GetComponent<CameraMovement>();

        consoleUI = GetComponent<UIDocument>();

        if (consoleUI != null)
            consoleRoot = consoleUI.rootVisualElement.Q("consoleRoot");
        inputField = consoleUI.rootVisualElement.Q<TextField>("inputField");
        submitButton = consoleUI.rootVisualElement.Q<Button>("submitButton");
        submitButton.clicked += () =>
        {
            ConsoleExtensions.ProcessCommands(inputField.value);
            inputField.value = string.Empty; // Clear input after submission
            inputField.Focus(); // Refocus input field
        };

        logContainer = consoleUI.rootVisualElement.Q<ScrollView>("logContainer");

        if (consoleUI != null)
        {
            ConsoleExtensions.AddLog("Console initialized. Type 'help' to see available commands.", Color.cyan);
            consoleRoot.style.display = DisplayStyle.None; // Start with console hidden
        }
        else
        {
            Debug.LogWarning("Console UI Document is not assigned in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.ToggleConsole) // Toggle console with the backquote key
        {
            isConsoleOpen = !isConsoleOpen;
            if (consoleUI != null)
            {
                consoleRoot.style.display = isConsoleOpen ? DisplayStyle.Flex : DisplayStyle.None; // Start with console hidden
                GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>().enabled = !isConsoleOpen;
                Camera.main.GetComponent<CameraMovement>().enabled = !isConsoleOpen;
                UnityEngine.Cursor.lockState = isConsoleOpen ? CursorLockMode.None : CursorLockMode.Locked;
                UnityEngine.Cursor.visible = isConsoleOpen ? true : false;
            }
            else
            {
                Debug.LogWarning("Console UI Document is not assigned in the inspector.");
            }
        }
    }
}
