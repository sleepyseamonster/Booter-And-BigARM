using UnityEngine;
using UnityEngine.InputSystem;

namespace BooterAndBigARM.GameCore.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveActionRef;
        [SerializeField] private InputActionReference runActionRef;

        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float acceleration = 20f;
        [SerializeField] private float deceleration = 10f;

        private CharacterController controller;
        private Vector2 inputDir;
        private Vector3 velocity;
        private bool isRunning;

        // 45-degree rotation for correct isometric forward (Z+) and right (X+)
        private static readonly Quaternion isoRotation = Quaternion.Euler(0, 45f, 0);

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            moveActionRef.action.Enable();
            runActionRef.action.Enable();
        }

        private void OnDisable()
        {
            moveActionRef.action.Disable();
            runActionRef.action.Disable();
        }

        private void Update()
        {
            // Read input
            inputDir = moveActionRef.action.ReadValue<Vector2>();
            isRunning = runActionRef.action.IsPressed();

            // Map 2D input to 3D world space and rotate for isometric feel
            Vector3 rawInput = new Vector3(inputDir.x, 0f, inputDir.y);
            Vector3 rotatedInput = isoRotation * rawInput;

            float targetSpeed = isRunning ? runSpeed : maxSpeed;
            Vector3 targetVelocity = rotatedInput.normalized * targetSpeed;

            // Accelerate or decelerate
            float rate = (targetVelocity.magnitude > velocity.magnitude) ? acceleration : deceleration;
            velocity = Vector3.MoveTowards(velocity, targetVelocity, rate * Time.deltaTime);

            controller.Move(velocity * Time.deltaTime);
        }
    }
}
