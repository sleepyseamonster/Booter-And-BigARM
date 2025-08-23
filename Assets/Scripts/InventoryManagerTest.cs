using UnityEngine;

public class InventoryManagerTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Starting InventoryManagerTest...");

        // Gather resources
        InventoryManager.Instance.GatherResource("wood", 10);
        InventoryManager.Instance.GatherResource("stone", 5);

        // Get resource amounts
        int woodAmount = InventoryManager.Instance.GetResourceAmount("wood");
        int stoneAmount = InventoryManager.Instance.GetResourceAmount("stone");
        Debug.Log($"Wood amount: {woodAmount}");
        Debug.Log($"Stone amount: {stoneAmount}");

        // Consume resources
        bool consumedWood = InventoryManager.Instance.ConsumeResource("wood", 7);
        bool consumedStone = InventoryManager.Instance.ConsumeResource("stone", 10); // Should fail

        Debug.Log($"Consumed 7 wood: {consumedWood}");
        Debug.Log($"Consumed 10 stone (should fail): {consumedStone}");

        // Final amounts
        Debug.Log($"Final wood amount: {InventoryManager.Instance.GetResourceAmount("wood")}");
        Debug.Log($"Final stone amount: {InventoryManager.Instance.GetResourceAmount("stone")}");
    }
}