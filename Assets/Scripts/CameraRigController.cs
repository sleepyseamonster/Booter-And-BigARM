using UnityEngine;

namespace BooterAndBigARM.Camera
{
    public class CameraRigController : MonoBehaviour
    {
        [Header("Target Settings")]
        [Tooltip("The transform the camera rig should follow (e.g., CameraTarget).")]
        public Transform target;

        [Header("Follow Settings")]
        [Tooltip("Offset from the target in local space.")]
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 5f, -10f);
        [Tooltip("Rotation offset from the target's rotation.")]
        [SerializeField] private Vector3 followRotationEuler = new Vector3(10f, 0f, 0f);
        [Tooltip("Smooth time for position interpolation.")]
        [SerializeField] private float positionSmoothTime = 0.15f;
        [Tooltip("Smooth time for rotation interpolation.")]
        [SerializeField] private float rotationSmoothTime = 0.15f;

        [Header("Zoom Settings")]
        [Tooltip("Speed of zooming in/out.")]
        [SerializeField] private float zoomSpeed = 10f;
        [Tooltip("Minimum zoom distance.")]
        [SerializeField] private float minZoomDistance = 2f;
        [Tooltip("Maximum zoom distance.")]
        [SerializeField] private float maxZoomDistance = 20f;

        private Vector3 currentVelocity = Vector3.zero;
        private float currentZoomDistance;
        private Quaternion followRotation;

        private void Start()
        {
            if (target == null)
            {
                Debug.LogWarning("CameraRigController: Target is not assigned.");
                enabled = false;
                return;
            }

            currentZoomDistance = followOffset.magnitude;
            followRotation = Quaternion.Euler(followRotationEuler);

            // Initialize position and rotation to avoid snapping
            Vector3 desiredPosition = target.position + target.rotation * followOffset.normalized * currentZoomDistance;
            transform.position = desiredPosition;
            transform.rotation = target.rotation * followRotation;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            FollowTargetSmooth();
            ZoomCamera();
        }

        private void FollowTargetSmooth()
        {
            // Calculate desired position based on target rotation and zoomed offset
            Vector3 desiredPosition = target.position + target.rotation * followOffset.normalized * currentZoomDistance;

            // Smoothly interpolate position
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, positionSmoothTime);

            // Calculate desired rotation based on target rotation and rotation offset
            Quaternion desiredRotation = target.rotation * followRotation;

            // Smoothly interpolate rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothTime * Time.deltaTime);
        }

        private void ZoomCamera()
        {
            float scroll = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentZoomDistance = Mathf.Clamp(currentZoomDistance - scroll * zoomSpeed * Time.deltaTime, minZoomDistance, maxZoomDistance);
            }
        }
    }
}