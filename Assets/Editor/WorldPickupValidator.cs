using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldPickup))]
public class WorldPickupValidator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldPickup pickup = (WorldPickup)target;

        if (pickup == null)
            return;

        if (pickup.triggerCollider == null)
        {
            EditorGUILayout.HelpBox("Trigger Collider reference is missing.", MessageType.Warning);
        }
        else
        {
            if (!pickup.triggerCollider.isTrigger)
            {
                EditorGUILayout.HelpBox("Assigned collider is not set as a trigger. Please enable 'Is Trigger'.", MessageType.Warning);
            }
        }

        if (pickup.item == null)
        {
            EditorGUILayout.HelpBox("Item reference is missing.", MessageType.Warning);
        }
    }
}