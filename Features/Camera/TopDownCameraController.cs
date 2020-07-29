using Unity.Mathematics;

namespace Cuku.OurCity
{
    using UnityEngine.InputSystem;
    using UnityEngine;

    public class TopDownCameraController : MonoBehaviour
    {
        public InputAction MoveAction;
        public InputAction ZoomAction;

        public float MoveSpeed = 1.0f;
        public float ZoomSpeed = 1.0f;
        public float LerpSpeed = 0.5f;

        private Transform target;

        private void Awake()
        {
            target = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            MoveAction.Enable();
            ZoomAction.Enable();
        }

        private void OnDisable()
        {
            MoveAction.Disable();
            ZoomAction.Disable();
        }

        private void Update()
        {
            var moveValue = MoveAction.ReadValue<Vector2>() * MoveSpeed;
            var targetPosition = target.position;

            var zoomValue = ZoomAction.ReadValue<float>() * ZoomSpeed;

            targetPosition.x += moveValue.x;
            targetPosition.z += moveValue.y;

            targetPosition.y += zoomValue;
            targetPosition.y = math.max(targetPosition.y, 0);

            target.position = Vector3.Lerp(target.position, targetPosition, LerpSpeed);
        }
    }
}