using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the player's item inventory (attach to car)
/// </summary>
public class ItemInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int maxItems = 1;
    
    [Header("Events")]
    public UnityEvent<ItemData> onItemPickup;
    public UnityEvent<ItemData> onItemUsed;
    public UnityEvent onItemCleared;
    
    private ItemData currentItem = null;
    private bool hasItem = false;

    public bool CanPickupItem()
    {
        return !hasItem;
    }

    public void AddItem(ItemData item)
    {
        if (hasItem && maxItems == 1)
        {
            Debug.LogWarning("Already have an item!");
            return;
        }
        
        currentItem = item;
        hasItem = true;
        
        onItemPickup?.Invoke(item);
        Debug.Log($"Picked up item: {item.itemName}");
    }

    public void UseItem()
    {
        if (!hasItem || currentItem == null)
        {
            Debug.LogWarning("No item to use!");
            return;
        }
        
        ExecuteItemEffect(currentItem);
        
        onItemUsed?.Invoke(currentItem);
        
        ClearItem();
    }

    private void ExecuteItemEffect(ItemData item)
    {
        
        // Get references
        CarController carController = GetComponent<CarController>();
        
        switch (item.type)
        {
            case ItemData.ItemType.SpeedBoost:
                if (carController != null)
                {
                    StartCoroutine(ApplySpeedBoost(carController, item.value, item.duration));
                }
                break;
                
            case ItemData.ItemType.SuperSpeedBoost:
                StartCoroutine(ApplySuperSpeedBoost(carController, item.value, item.duration));
                break;
        }
        
        // Spawn use effect
        if (item.usePrefab != null)
        {
            Instantiate(item.usePrefab, transform.position, Quaternion.identity);
        }
    }

    private System.Collections.IEnumerator ApplySpeedBoost(CarController car, float boostAmount, float duration)
    {
        Debug.Log($"Speed boost activated! +{boostAmount} for {duration}s");
        
        float originalSpeed = car.GetComponent<CarController>().forwardSpeed;

        
        yield return new WaitForSeconds(duration);
        
        // Restore original speed
        // car.ResetSpeed();
        
        Debug.Log("Speed boost ended");
    }

    private System.Collections.IEnumerator ApplySuperSpeedBoost(CarController car, float boostAmount, float duration)
    {
        Debug.Log($"Super Speed boost activated! +{boostAmount} for {duration}s");
        
        float originalSpeed = car.GetComponent<CarController>().forwardSpeed;
        
        // TODO: Add public method to CarController to temporarily boost speed
        // car.ApplySpeedBoost(boostAmount);
        
        yield return new WaitForSeconds(duration);
        
        // Restore original speed
        // car.ResetSpeed();
        
        Debug.Log("Speed boost ended");
    }

    private void ClearItem()
    {
        currentItem = null;
        hasItem = false;
        onItemCleared?.Invoke();
    }

    // Getters
    public ItemData GetCurrentItem() => currentItem;
    public bool HasItem() => hasItem;
}