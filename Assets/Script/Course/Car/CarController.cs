using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Put this script on the car GameObject to control its movement.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 30f; // Speed of the car
    [SerializeField] private float acceleration = 10f; // How fast the car accelerates
    [SerializeField] private float turnSpeed = 100f; // Speed when player turns
    [SerializeField] private float brakeDeceleration = 15f; // Deceleration rate when braking (higher = faster brake)
    [SerializeField] private float naturalDeceleration = 2f; // Slowdown when not accelerating
    [SerializeField] private float minSpeed = 0.1f; // Speed threshold to stop completely

    [Header("Input Settings")]
    [SerializeField] private bool useKeyboardInput = true; // Enable/disable keyboard input
    [SerializeField] private Key leftKey = Key.A; // Key to turn left
    [SerializeField] private Key rightKey = Key.D; // Key to turn right
    [SerializeField] private Key brakeKey = Key.Space; // Key to brake

    private Rigidbody rb;
    private float currentSpeed; // Current actual speed
    private float turnInput;
    private bool isBraking;
    private Keyboard keyboard;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Down the mass center | It's for stability
        
        keyboard = Keyboard.current;
        currentSpeed = 0f;
    }

    void Update()
    {
        // Keyboard input handling
        if (useKeyboardInput && keyboard != null)
        {
            HandleKeyboardInput();
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

        // Brake input
        bool braking = keyboard[brakeKey].isPressed;

        SetInputs(steering, braking);
    }

    // Function to set inputs from other scripts (for mobile or keyboard controls)
    public void SetInputs(float steering, bool braking)
    {
        turnInput = steering;
        isBraking = braking;
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
            
            // Decelerationvv
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

    // Public getter for current speed (for ui)
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetSpeedPercentage()
    {
        return currentSpeed / forwardSpeed;
    }
}