namespace Cuku.OurCity
{
    using UnityEngine.InputSystem;
    using UnityEngine;

    public class TopDownCameraController : MonoBehaviour
    {
        public InputAction MoveAction;

        public float MoveSpeed = 1.0f;
        public float MoveLerpSpeed = 0.5f;

        private Transform target;

        private void Awake()
        {
            target = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            MoveAction.Enable();
        }

        private void OnDisable()
        {
            MoveAction.Disable();
        }

        private void Update()
        {
            var moveValue = MoveAction.ReadValue<Vector2>() * (MoveSpeed * Time.deltaTime);
            var targetPosition = target.position;

            targetPosition.x += moveValue.x;
            targetPosition.z += moveValue.y;

            target.position = Vector3.Lerp(target.position, targetPosition, MoveLerpSpeed);
        }
    }
}