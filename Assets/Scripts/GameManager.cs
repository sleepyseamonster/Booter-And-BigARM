using UnityEngine;

namespace BooterAndBigARM.Gameplay
{
    public class GameManager : MonoBehaviour
{
    public float dayLength = 120f; // Length of a day in seconds
    private float timeOfDay = 0f;

    void Update()
    {
        UpdateDayNightCycle();
    }

    private void UpdateDayNightCycle()
    {
        timeOfDay += Time.deltaTime;
        if (timeOfDay >= dayLength)
        {
            timeOfDay = 0f;
            // Trigger day-night transition events
        }

        // Update lighting and other environmental effects based on timeOfDay
    }
    }
}