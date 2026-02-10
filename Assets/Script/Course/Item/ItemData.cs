using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Info Item")]
    public string itemName;
    public Sprite icon;

    public abstract void Activate(CarController kart);
}