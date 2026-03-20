using UnityEngine;
using UnityEngine.EventSystems;


public class OffRoad : MonoBehaviour
{
    [SerializeField] private float respawnCooldown = 1.5f; 
    private float cooldownTimer = 0f;
    private Rigidbody carRigidbody;
    private CarController carController;
    private RigidbodyConstraints originalConstraints;

    void Start()
    {
        CarController car = FindFirstObjectByType<CarController>();
        if (car != null)
        {
            carRigidbody = car.GetComponent<Rigidbody>();
            carController = car;
        }
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            
            if (cooldownTimer <= 0)
            {
                if (carRigidbody != null)
                {
                    carRigidbody.constraints = originalConstraints;
                }
                if (carController != null)
                {
                    carController.enabled = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawner respawner = other.GetComponent<PlayerRespawner>();
        if (respawner != null)
        {
            respawner.Respawn();
            
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            StartCooldown(rb);
        }
    }

    private void StartCooldown(Rigidbody rb)
    {
        cooldownTimer = respawnCooldown;

        if (rb != null)
        {
            originalConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (carController != null)
        {
            carController.enabled = false;
        }
    }
}