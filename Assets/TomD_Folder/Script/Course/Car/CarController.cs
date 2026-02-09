using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Controls car movement and rotation.
/// Supports keyboard, mobile joystick, and UI brake button inputs.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 30f;      // Maximum forward speed
    [SerializeField] private float acceleration = 10f;      // Acceleration rate
    [SerializeField] private float turnSpeed = 70f;         // Rotation speed
    [SerializeField] private float brakeDeceleration = 15f; // Deceleration when braking
    [SerializeField] private float minSpeed = 0.1f;         // Speed threshold to fully stop

    [Header("Input Settings")]
    [SerializeField] private InputMode inputMode = InputMode.Keyboard;

    [Header("Keyboard Settings")]
    [SerializeField] private Key leftKey = Key.A;    // Turn left key
    [SerializeField] private Key rightKey = Key.D;   // Turn right key
    [SerializeField] private Key brakeKey = Key.Space; // Brake key

    [Header("Joystick Settings")]
    [SerializeField] private VariableJoystick variableJoystick; // Mobile joystick reference
    [SerializeField] private float joystickDeadzone = 0.1f;     // Ignore small joystick input

    [Header("Brake Button (Mobile)")]
    [SerializeField] private GameObject brakeButtonObject; // UI brake button

    private Rigidbody rb;
    private Keyboard keyboard;

    private float currentSpeed;   // Current forward speed
    private float turnInput;      // Steering input (-1 to 1)
    private bool isBraking;       // Final braking state
    private bool uiBrakeActive;   // UI brake button state

    public enum InputMode
    {
        Keyboard,
        Joystick,
        Both
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lower center of mass for better stability
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        keyboard = Keyboard.current;
        currentSpeed = 0f;

        // Automatically configure the mobile brake button
        SetupBrakeButton();
    }

    private void SetupBrakeButton()
    {
        if (brakeButtonObject == null) return;

        EventTrigger trigger = brakeButtonObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = brakeButtonObject.AddComponent<EventTrigger>();
        }

        trigger.triggers.Clear();

        // Brake press
        EventTrigger.Entry pointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDown.callback.AddListener(_ => OnBrakePressed());
        trigger.triggers.Add(pointerDown);

        // Brake release
        EventTrigger.Entry pointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUp.callback.AddListener(_ => OnBrakeReleased());
        trigger.triggers.Add(pointerUp);
    }

    private void Update()
    {
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
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleKeyboardInput()
    {
        float steering = 0f;

        if (keyboard[leftKey].isPressed)
            steering -= 1f;

        if (keyboard[rightKey].isPressed)
            steering += 1f;

        bool braking = keyboard[brakeKey].isPressed || uiBrakeActive;

        SetInputs(steering, braking);
    }

    private void HandleJoystickInput()
    {
        if (variableJoystick == null) return;

        float horizontal = variableJoystick.Horizontal;

        // Apply deadzone
        if (Mathf.Abs(horizontal) < joystickDeadzone)
            horizontal = 0f;

        bool braking = uiBrakeActive;

        // Pulling joystick down triggers braking
        if (variableJoystick.Vertical < -0.5f)
            braking = true;

        SetInputs(horizontal, braking);
    }

    // Receives steering and braking input from any input source
    public void SetInputs(float steering, bool braking)
    {
        turnInput = steering;
        isBraking = braking;
    }

    // Called while the brake button is held
    public void OnBrakePressed()
    {
        uiBrakeActive = true;
    }

    // Called when the brake button is released
    public void OnBrakeReleased()
    {
        uiBrakeActive = false;
    }

    private void HandleMovement()
    {
        if (isBraking)
        {
            // Reduce speed while braking
            currentSpeed = Mathf.Max(0f, currentSpeed - brakeDeceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Accelerate forward
            currentSpeed = Mathf.Min(forwardSpeed, currentSpeed + acceleration * Time.fixedDeltaTime);
        }

        // Stop completely if speed is very low
        if (currentSpeed < minSpeed)
        {
            currentSpeed = 0f;
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            return;
        }

        Vector3 forwardVelocity = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(forwardVelocity.x, rb.linearVelocity.y, forwardVelocity.z);
    }

    private void HandleRotation()
    {
        // Only allow steering when the car is moving
        if (currentSpeed <= 1f) return;

        float rotation = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    // Returns the current speed value
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    // Returns speed normalized between 0 and 1
    public float GetSpeedPercentage()
    {
        return currentSpeed / forwardSpeed;
    }
}