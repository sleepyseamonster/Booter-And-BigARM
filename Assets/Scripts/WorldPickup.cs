using UnityEngine;

public class WorldPickup : MonoBehaviour
{
    private string resourceId;
    private int quantity;

    public void SetItem(ItemDefinition itemDef, int qty)
    {
        resourceId = itemDef.id;
        quantity = qty;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.Instance.GatherResource(resourceId, quantity);
            Destroy(gameObject);
        }
    }
}