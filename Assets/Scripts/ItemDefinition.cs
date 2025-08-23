using UnityEngine;

public enum ItemCategory
{
    Resource,
    // Add other categories as needed
}

[CreateAssetMenu(fileName = "NewItemDefinition", menuName = "Items/Item Definition", order = 1)]
public class ItemDefinition : ScriptableObject
{
    [Tooltip("Unique string ID for the item, e.g. 'item.raw_ironstone'")]
    public string id;

    [Tooltip("Display name of the item")]
    public string displayName;

    [Tooltip("Category of the item")]
    public ItemCategory category;

    [Tooltip("Maximum stack size for the item")]
    public int maxStack = 200;

    [Tooltip("Optional icon for the item")]
    public Sprite icon;
}