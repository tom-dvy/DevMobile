using UnityEngine;
using UnityEngine.UI;

public class ItemSpriteHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject backgroundFrame; 

    void Start()
    {
        UpdateItemUI(null);
    }

    public void UpdateItemUI(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
        else
        {
            iconImage.sprite = itemSprite;
            iconImage.enabled = true;
        }
    }
}