using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Put this script on the car GameObject to control its movement.
/// Supports both keyboard and mobile joystick inputs.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 30f; // Speed of the car
    [SerializeField] private float acceleration = 10f; // How fast the car accelerates
    [SerializeField] private float turnSpeed = 70f; // Speed when player turns
    [SerializeField] private float brakeDeceleration = 15f; // Deceleration rate when braking (higher = faster brake)
    [SerializeField] private float naturalDeceleration = 2f; // Slowdown when not accelerating
    [SerializeField] private float minSpeed = 0.1f; // Speed threshold to stop completely

    [Header("Input Settings")]
    [SerializeField] private InputMode inputMode = InputMode.Keyboard;

    [Header("Keyboard Settings")]
    [SerializeField] private Key leftKey = Key.A; // Key to turn left
    [SerializeField] private Key rightKey = Key.D; // Key to turn right
    [SerializeField] private Key brakeKey = Key.Space; // Key to brake

    [Header("Joystick Settings")]
    [SerializeField] private VariableJoystick variableJoystick; // Reference to the joystick
    [SerializeField] private float joystickDeadzone = 0.1f; // Deadzone for joystick input

    [Header("Brake Button")]
    [SerializeField] private GameObject brakeButtonObject; // Drag the button GameObject here

    [Header("Item Settings")]
    [SerializeField] private Key useItemKey = Key.E; // Key to use item
    [SerializeField] private GameObject itemUseButton; // Mobile button for item use

    private ItemInventory itemInventory;
    private Rigidbody rb;
    private float currentSpeed; // Current actual speed
    private float turnInput;
    private bool isBraking;
    private bool uiBrakeActive = false; // Track UI button brake state
    private Keyboard keyboard;

    public enum InputMode
    {
        Keyboard,
        Joystick,
        Both // Allow both inputs simultaneously
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Down the mass center | It's for stability

        keyboard = Keyboard.current;
        currentSpeed = 0f;

        itemInventory = GetComponent<ItemInventory>();

        // Setup item button
        SetupItemButton();

        // Setup brake button automatically
        SetupBrakeButton();
    }

    private void SetupItemButton()
    {
        if (itemUseButton == null) return;

        EventTrigger trigger = itemUseButton.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = itemUseButton.AddComponent<EventTrigger>();
        }

        trigger.triggers.Clear();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { UseItem(); });
        trigger.triggers.Add(pointerDownEntry);
    }

    private void SetupBrakeButton()
    {
        if (brakeButtonObject == null) return;

        // Get or add EventTrigger component
        EventTrigger trigger = brakeButtonObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = brakeButtonObject.AddComponent<EventTrigger>();
        }

        // Clear existing entries
        trigger.triggers.Clear();

        // Add PointerDown event (when button is pressed)
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnBrakePressed(); });
        trigger.triggers.Add(pointerDownEntry);

        // Add PointerUp event (when button is released)
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnBrakeReleased(); });
        trigger.triggers.Add(pointerUpEntry);

        Debug.Log("Brake button configured successfully!");
    }

    void Update()
    {
        // Handle inputs based on selected mode
        switch (inputMode)
        {
            case InputMode.Keyboard:
                if (keyboard != null)
                    HandleKeyboardInput();
                break;

            case InputMode.Joystick:
                HandleJoystickInput();
                break;

            case InputMode.Both:
                if (keyboard != null)
                    HandleKeyboardInput();
                HandleJoystickInput();
                break;

                if (keyboard != null && keyboard[useItemKey].wasPressedThisFrame)
                {
                    UseItem();
                }
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleKeyboardInput()
    {
        // Directional input
        float steering = 0f;
        if (keyboard[leftKey].isPressed)
        {
            steering -= 1f;
        }

        if (keyboard[rightKey].isPressed)
        {
            steering += 1f;
        }

        // Brake input (keyboard or UI button)
        bool braking = keyboard[brakeKey].isPressed || uiBrakeActive;

        SetInputs(steering, braking);
    }

    private void HandleJoystickInput()
    {
        if (variableJoystick == null) return;

        float horizontal = variableJoystick.Horizontal;

        // Apply deadzone
        if (Mathf.Abs(horizontal) < joystickDeadzone)
        {
            horizontal = 0f;
        }

        // Combine UI button brake with joystick vertical brake
        bool braking = uiBrakeActive; // Start with UI button state

        if (variableJoystick.Vertical < -0.5f) // Joystick pulled down
        {
            braking = true;
        }

        SetInputs(horizontal, braking);
    }

    // Function to set inputs from other scripts (for mobile or keyboard controls)
    public void SetInputs(float steering, bool braking)
    {
        turnInput = steering;
        isBraking = braking;
    }

    // Called when brake button is pressed (hold)
    public void OnBrakePressed()
    {
        Debug.Log("Brake button PRESSED");
        uiBrakeActive = true;
    }

    // Called when brake button is released
    public void OnBrakeReleased()
    {
        Debug.Log("Brake button RELEASED");
        uiBrakeActive = false;
    }

    private void HandleMovement()
    {
        if (isBraking)
        {
            // Progressive braking
            currentSpeed = Mathf.Max(0f, currentSpeed - brakeDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Accelerate towards max speed
            currentSpeed = Mathf.Min(forwardSpeed, currentSpeed + acceleration * Time.fixedDeltaTime);

            // Natural deceleration
            if (currentSpeed > forwardSpeed)
            {
                currentSpeed = Mathf.Max(forwardSpeed, currentSpeed - naturalDeceleration * Time.fixedDeltaTime);
            }
        }

        // Stop completely if speed is very low
        if (currentSpeed < minSpeed)
        {
            currentSpeed = 0f;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
        else
        {
            // Apply the speed in the forward direction
            Vector3 targetVelocity = transform.forward * currentSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
    }

    private void HandleRotation()
    {
        if (currentSpeed > 1f) // Only turn when moving
        {
            float rotation = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void UseItem()
    {
        if (itemInventory != null)
        {
            itemInventory.UseItem();
        }
    }

    // Public getter for current speed
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetSpeedPercentage()
    {
        return currentSpeed / forwardSpeed;
    }
}
