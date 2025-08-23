using UnityEngine;
using UnityEngine.InputSystem;

namespace BooterAndBigARM.Camera
{
    public class CameraTargetController : MonoBehaviour
    {
        [Header("Player Settings")]
        [Tooltip("The player transform to which this CameraTarget is parented.")]
        public Transform playerTransform;

        [Header("Pan Settings")]
        public float lookSpeed = 10f;
        public float returnSpeed = 5f;
        [SerializeField] private float maxPanDistance = 5f;
    
        [Header("Input")]
        [SerializeField] private InputActionReference rightStickAction;

        private Vector3 velocity = Vector3.zero;
        private Vector2 rightStickInput = Vector2.zero;

        void Start()
        {
            if (rightStickAction != null)
            {
                rightStickAction.action.Enable();
            }
            if (playerTransform != null)
            {
                transform.position = playerTransform.position;
            }
        }

        void Update()
        {
            if (rightStickAction != null)
            {
                rightStickInput = rightStickAction.action.ReadValue<Vector2>();
            }
    
            Vector3 playerPos = playerTransform.position;
            Vector3 currentPos = transform.position;
    
            // Clamp speeds to avoid zero or negative values
            float clampedLookSpeed = Mathf.Max(0.01f, lookSpeed);
            float clampedReturnSpeed = Mathf.Max(0.01f, returnSpeed);
    
            // Dead zone threshold for input magnitude
            float deadZone = 0.1f;
    
            if (rightStickInput.magnitude > deadZone)
            {
                // Calculate pan offset scaled by maxPanDistance and input magnitude
                Vector3 panInput = new Vector3(rightStickInput.x, 0f, rightStickInput.y).normalized;
                float panMagnitude = Mathf.Clamp01(rightStickInput.magnitude);
    
                // Rotate panInput by 45 degrees around Y axis to correct direction
                Quaternion rotationAdjustment = Quaternion.Euler(0f, 45f, 0f);
                Vector3 rotatedPanInput = rotationAdjustment * panInput;
    
                Vector3 panOffset = rotatedPanInput * panMagnitude * maxPanDistance;
    
                Vector3 targetPos = playerPos + panOffset;
    
                // Smoothly interpolate position towards targetPos using Lerp for smooth movement
                float lerpFactor = clampedLookSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(currentPos, targetPos, lerpFactor);
            }
            else
            {
                // Smoothly interpolate position back to player position using Lerp
                float lerpFactor = clampedReturnSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(currentPos, playerPos, lerpFactor);
            }
        }
    }
}