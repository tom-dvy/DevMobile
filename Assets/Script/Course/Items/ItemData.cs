using UnityEngine;

/// <summary>
/// ScriptableObject that defines an item's properties
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Racing/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public string itemID; // Unique identifier
    public Sprite icon; // For UI
    public GameObject pickupPrefab; // Visual on track
    public GameObject usePrefab; // Effect when used
    
    [Header("Item Properties")]
    public ItemType type;
    public float duration = 0f;
    public float value = 0f;
    
    [Header("Rarity")]
    [Range(0f, 100f)]
    public float spawnChance = 50f;
    
    public enum ItemType
    {
        SpeedBoost,
        SuperSpeedBoost
    }
}