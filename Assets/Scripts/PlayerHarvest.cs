using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHarvest : MonoBehaviour
{
    [SerializeField] private InputActionReference harvestActionReference;
    [SerializeField] private float harvestRange = 3f;
    private Camera playerCamera;

    private void OnEnable()
    {
        harvestActionReference.action.Enable();
        harvestActionReference.action.performed += OnHarvest;
    }

    private void OnDisable()
    {
        harvestActionReference.action.performed -= OnHarvest;
        harvestActionReference.action.Disable();
    }

    private void Awake()
    {
        playerCamera = Camera.main;
    }

    private void OnHarvest(InputAction.CallbackContext context)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, harvestRange);
        foreach (var hit in hits)
        {
            HarvestableNode node = hit.GetComponent<HarvestableNode>();
            if (node != null)
            {
                node.ApplyHit(1f);
                break;
            }
        }
    }
}