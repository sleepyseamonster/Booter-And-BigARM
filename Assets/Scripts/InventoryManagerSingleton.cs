using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    public class InventoryManagerSingleton : MonoBehaviour
    {
        public static InventoryManagerSingleton Instance { get; private set; }

        private Dictionary<string, int> resourceInventory = new Dictionary<string, int>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void GatherResource(string resourceId, int amount)
        {
            if (resourceInventory.ContainsKey(resourceId))
            {
                resourceInventory[resourceId] += amount;
            }
            else
            {
                resourceInventory[resourceId] = amount;
            }
            Debug.Log($"Gathered {amount} of {resourceId}. Total now: {resourceInventory[resourceId]}");
        }

        public int GetResourceAmount(string resourceId)
        {
            if (resourceInventory.TryGetValue(resourceId, out int amount))
            {
                return amount;
            }
            return 0;
        }

        public bool ConsumeResource(string resourceId, int amount)
        {
            if (resourceInventory.TryGetValue(resourceId, out int currentAmount) && currentAmount >= amount)
            {
                resourceInventory[resourceId] -= amount;
                Debug.Log($"Consumed {amount} of {resourceId}. Remaining: {resourceInventory[resourceId]}");
                return true;
            }
            Debug.LogWarning($"Not enough {resourceId} to consume. Requested: {amount}, Available: {currentAmount}");
            return false;
        }
    }
}