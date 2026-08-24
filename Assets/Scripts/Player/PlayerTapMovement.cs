using UnityEngine;
using Causebound.Interaction;

namespace Causebound.Player
{
    public sealed class PlayerTapMovement : MonoBehaviour
    {
        [SerializeField] private PointerInteractionInput input;
        [SerializeField] private Camera movementCamera;
        [SerializeField] private LayerMask walkableLayers = ~0;
        [SerializeField, Min(0.1f)] private float movementSpeed = 4f;

        private Vector3 destination;
        private bool moving;

        private void Awake()
        {
            input ??= FindFirstObjectByType<PointerInteractionInput>();
            movementCamera ??= Camera.main;
            destination = transform.position;
        }

        private void OnEnable()
        {
            if (input != null)
            {
                input.Pressed += HandlePressed;
            }
        }

        private void OnDisable()
        {
            if (input != null)
            {
                input.Pressed -= HandlePressed;
            }
        }

        private void Update()
        {
            if (!moving)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
            moving = Vector3.SqrMagnitude(transform.position - destination) > 0.001f;
        }

        private void HandlePressed(Vector2 screenPosition)
        {
            if (movementCamera == null)
            {
                return;
            }

            var ray = movementCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 100f, walkableLayers))
            {
                return;
            }

            destination = hit.point;
            destination.y = transform.position.y;
            moving = true;
        }
    }
}
