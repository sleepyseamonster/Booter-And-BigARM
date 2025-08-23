using UnityEngine;

namespace BooterAndBigARM.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class BigARMController : MonoBehaviour
    {
        [Header("Follow Target")]
        public Transform target; // set to Booter at runtime

        [Header("Movement")]
        [Min(0f)] public float maxSpeed = 2f;
        [Min(0f)] public float acceleration = 12f;
        [Min(0f)] public float deceleration = 18f;
        [Min(0f)] public float stopDistance = 8f;
        [Tooltip("When within this many meters, start proportional braking to avoid overshoot.")]
        public float slowDownRadius = 12f;

        [Header("Body")]
        public float rotateSpeed = 2f;

        [Header("Grounding")]
        [Tooltip("Lock Y to this value since terrain is flat for now.")]
        public float groundY = 0f;

        private CharacterController _cc;
        private Vector3 _vel;

        void Awake() => _cc = GetComponent<CharacterController>();

        void Update()
        {
            // keep mech on the flat plane
            var p = transform.position;
            if (Mathf.Abs(p.y - groundY) > 0.0001f)
            {
                p.y = groundY;
                transform.position = p;
            }

            if (!target)
            {
                // no target? passive brake
                _vel = Vector3.MoveTowards(_vel, Vector3.zero, deceleration * Time.deltaTime);
                if (_vel.sqrMagnitude > 1e-5f) _cc.Move(_vel * Time.deltaTime);
                return;
            }

            Vector3 to = target.position - transform.position;
            to.y = 0f;
            float dist = to.magnitude;

            // desired speed with proportional falloff near target
            float desiredSpeed = 0f;
            if (dist > stopDistance)
            {
                if (dist < slowDownRadius)
                {
                    // ramp down as we approach (prevents orbiting/jitter)
                    float t = Mathf.InverseLerp(stopDistance, slowDownRadius, dist);
                    desiredSpeed = Mathf.Lerp(0f, maxSpeed, t);
                }
                else desiredSpeed = maxSpeed;
            }

            Vector3 desiredVel = (dist > 0.001f) ? to.normalized * desiredSpeed : Vector3.zero;

            // accel/brake toward desired
            float rate = (desiredVel.sqrMagnitude > _vel.sqrMagnitude) ? acceleration : deceleration;
            _vel = Vector3.MoveTowards(_vel, desiredVel, rate * Time.deltaTime);

            // move without rotation (locked rotation)
            if (_vel.sqrMagnitude > 1e-6f)
            {
                _cc.Move(_vel * Time.deltaTime);

                // Rotation disabled to keep original orientation
                // if (dist >= stopDistance)
                // {
                //     Quaternion want = Quaternion.LookRotation(_vel.normalized, Vector3.up);
                //     transform.rotation = Quaternion.Slerp(transform.rotation, want, rotateSpeed * Time.deltaTime);
                // }
            }
        }

        public void SetTarget(Transform t) => target = t;
    }
}