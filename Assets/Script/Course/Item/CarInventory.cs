using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CarInventory : MonoBehaviour
{
    [Header("État Actuel")]
    public ItemData currentItem;
    
    private CarController carController;

    public UnityEvent<Sprite> OnItemChanged; 

    void Start()
    {
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        bool useItemPressed = false;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            useItemPressed = true;
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                useItemPressed = true;
            }
        }

        if (useItemPressed)
        {
            UseItem();
        }
    }

    public void ObtainItem(ItemData newItem)
    {
        currentItem = newItem;
        OnItemChanged?.Invoke(currentItem.icon);
        Debug.Log("Item obtenu : " + newItem.itemName);
    }

    public void UseItem()
    {
        if (currentItem != null)
        {
            currentItem.Activate(carController);
            currentItem = null; 
            OnItemChanged?.Invoke(null);
            Debug.Log("Item utilisé !");
        }
    }
}