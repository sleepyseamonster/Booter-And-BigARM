using UnityEngine;
using UnityEditor;

public class CreateRawIronstoneDropTable
{
    [MenuItem("Assets/Create/Drop Tables/Raw Ironstone Drop Table")]
    public static void CreateDropTable()
    {
        var dropTable = ScriptableObject.CreateInstance<DropTable>();

        var entry = new DropTable.Entry
        {
            minQuantity = 1,
            maxQuantity = 3,
            dropChance = 1f,
            item = null // Assign Raw Ironstone ItemDefinition in inspector
        };

        dropTable.entries.Add(entry);

        string assetPath = "Assets/DropTables/RawIronstoneDropTable.asset";
        AssetDatabase.CreateAsset(dropTable, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = dropTable;

        Debug.Log("Created Raw Ironstone DropTable asset at " + assetPath + ". Please assign the Raw Ironstone ItemDefinition in the inspector.");
    }
}