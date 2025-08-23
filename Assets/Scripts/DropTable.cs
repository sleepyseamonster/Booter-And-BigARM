using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDropTable", menuName = "Drops/Drop Table", order = 1)]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public ItemDefinition item;
        public int minQuantity;
        public int maxQuantity;
        [Range(0f, 1f)]
        public float dropChance;
    }

    public List<Entry> entries = new List<Entry>();

    [System.Serializable]
    public struct Context
    {
        public string biome;
        public string timeOfDay;
        public int seed;
    }

    // For now, ignore context and return entries as is
    public List<Entry> GetDrops(Context? context = null)
    {
        return entries;
    }
}