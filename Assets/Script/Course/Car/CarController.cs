using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Put this script on the car GameObject to control its movement.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 20f; // Speed of the car
    [SerializeField] private float turnSpeed = 100f; // Speed when player turns
    [SerializeField] private float brakeForce = 20f; // Force applied when braking

    [Header("Input Settings")]
    [SerializeField] private bool useKeyboardInput = true; // Enable/disable keyboard input
    [SerializeField] private Key leftKey = Key.A; // Key to turn left
    [SerializeField] private Key rightKey = Key.D; // Key to turn right
    [SerializeField] private Key brakeKey = Key.Space; // Key to brake

    private Rigidbody rb;
    private float moveInput; 
    private float turnInput;
    private bool isBraking;
    private Keyboard keyboard;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Down the mass center | It's for stability
        
        keyboard = Keyboard.current;
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
            // Apply braking force
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, brakeForce * Time.fixedDeltaTime);
        }
        else
        {
            // Apply forward movement
            Vector3 targetVelocity = transform.forward * forwardSpeed;
            rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        }
    }

    private void HandleRotation()
    {
        if (rb.linearVelocity.magnitude > 1f)
        {
            float rotation = turnInput * turnSpeed * Time.fixedDeltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}