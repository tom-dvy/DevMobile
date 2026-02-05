using UnityEngine;

/// <summary>
/// Place this on item pickups on the track
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Reference")]
    [SerializeField] private ItemData itemData;
    
    [Header("Pickup Settings")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float respawnTime = 5f;
    
    [Header("Visual")]
    [SerializeField] private GameObject visualObject;
    
    private Vector3 startPosition;
    private bool isActive = true;
    private float respawnTimer = 0f;

    void Start()
    {
        startPosition = transform.position;
        
        // Make sure it's a trigger
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    void Update()
    {
        if (isActive)
        {
            // Rotate the item
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
        else
        {
            // Handle respawn
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnTime)
            {
                Respawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            ItemInventory inventory = other.GetComponent<ItemInventory>();
            
            if (inventory != null && inventory.CanPickupItem())
            {
                // Give item to player
                inventory.AddItem(itemData);
                
                // Deactivate pickup
                Deactivate();
            }
        }
    }

    private void Deactivate()
    {
        isActive = false;
        respawnTimer = 0f;
        
        // Hide visual
        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        isActive = true;
        transform.position = startPosition;
        
        // Show visual
        if (visualObject != null)
        {
            visualObject.SetActive(true);
        }
    }

    // For manual setup
    public void SetItem(ItemData data)
    {
        itemData = data;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}