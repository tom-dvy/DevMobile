using UnityEngine;
using System.Collections;

public class ItemBox : MonoBehaviour
{
    [Header("Configuration")]
    public ItemData[] possibleItems; 
    public float respawnTime = 3f; 
    
    [Header("Visuel")]
    public GameObject visualModel; 
    public ParticleSystem pickupEffect;

    private Collider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarInventory inventory = other.GetComponent<CarInventory>();
            
            if (inventory != null && inventory.currentItem == null)
            {
                PickRandomItem(inventory);
                StartCoroutine(DisableBoxRoutine());
            }
        }
    }

    void PickRandomItem(CarInventory inventory)
    {
        if (possibleItems.Length == 0) return;

        int randomIndex = Random.Range(0, possibleItems.Length);
        inventory.ObtainItem(possibleItems[randomIndex]);
    }

    IEnumerator DisableBoxRoutine()
    {
        // Disable
        visualModel.SetActive(false);
        boxCollider.enabled = false;
        if(pickupEffect) pickupEffect.Play();

        yield return new WaitForSeconds(respawnTime);

        // Reactivate
        visualModel.SetActive(true);
        boxCollider.enabled = true;
    }
}