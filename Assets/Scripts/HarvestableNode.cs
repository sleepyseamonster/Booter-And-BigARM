using System.Collections;
using UnityEngine;

public class HarvestableNode : MonoBehaviour
{
    [SerializeField] private int hitsRequired = 2;
    [SerializeField] private float respawnTime = 120f;
    [SerializeField] private GameObject intactVisual;
    [SerializeField] private GameObject depletedVisual;
    [SerializeField] private DropTable dropTable;
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private Transform dropOrigin;
    [SerializeField] private float scatterRadius = 0.75f;

    private int currentHits;
    private bool isDepleted = false;

    private void Awake()
    {
        currentHits = hitsRequired;
        SetVisualState(true);
    }

    public void ApplyHit(float power = 1f)
    {
        if (isDepleted)
            return;

        currentHits -= Mathf.CeilToInt(power);
        if (currentHits <= 0)
        {
            Deplete();
        }
    }

    private float respawnEndTime;

    private void Deplete()
    {
        isDepleted = true;
        SetVisualState(false);
        SpawnPickups();
        respawnEndTime = Time.time + respawnTime;
        StartCoroutine(RespawnCoroutine());
    }

    private void SetVisualState(bool intact)
    {
        if (intactVisual != null)
            intactVisual.SetActive(intact);
        if (depletedVisual != null)
            depletedVisual.SetActive(!intact);
    }

    private void SpawnPickups()
    {
        if (dropTable == null || pickupPrefab == null)
            return;

        var drops = dropTable.GetDrops();
        if (drops == null || drops.Count == 0)
            return;

        Vector3 origin = dropOrigin != null ? dropOrigin.position : transform.position;

        foreach (var drop in drops)
        {
            Vector3 scatterPos = origin + Random.insideUnitSphere * scatterRadius;
            scatterPos.y = origin.y; // Keep on same horizontal plane
            GameObject pickup = Instantiate(pickupPrefab, scatterPos, Quaternion.identity);
            WorldPickup worldPickup = pickup.GetComponent<WorldPickup>();
            if (worldPickup != null)
            {
                worldPickup.SetItem(drop.item, Random.Range(drop.minQuantity, drop.maxQuantity + 1));
            }
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        while (Time.time < respawnEndTime)
        {
            if (gameObject.activeInHierarchy)
            {
                yield return null;
            }
            else
            {
                // If inactive, wait until active again
                yield return new WaitUntil(() => gameObject.activeInHierarchy);
            }
        }
        currentHits = hitsRequired;
        isDepleted = false;
        SetVisualState(true);
    }

    private void OnEnable()
    {
        if (isDepleted)
        {
            StopAllCoroutines();
            if (Time.time >= respawnEndTime)
            {
                // Respawn immediately
                currentHits = hitsRequired;
                isDepleted = false;
                SetVisualState(true);
            }
            else
            {
                StartCoroutine(RespawnCoroutine());
            }
        }
    }
}