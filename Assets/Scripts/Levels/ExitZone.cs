using UnityEngine;
using Causebound.Objects;
using Causebound.Player;

namespace Causebound.Levels
{
    public sealed class ExitZone : MonoBehaviour
    {
        [SerializeField] private StatefulObject exitState;
        [SerializeField] private string availableState = "Available";
        [SerializeField] private LevelCompletionState completion;

        private void Awake()
        {
            exitState ??= GetComponent<StatefulObject>();
            completion ??= GetComponent<LevelCompletionState>();
        }

        private void OnTriggerEnter(Collider other)
        {
            TryComplete(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryComplete(other);
        }

        private void TryComplete(Collider other)
        {
            if (other.GetComponentInParent<PlayerTapMovement>() == null || exitState == null || completion == null)
            {
                return;
            }

            if (exitState.CurrentState == availableState)
            {
                completion.Complete();
            }
        }
    }
}
