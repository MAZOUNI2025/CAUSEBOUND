using System;
using UnityEngine;
using Causebound.Objects;

namespace Causebound.Core
{
    public sealed class LevelResetController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] explicitTargets = Array.Empty<MonoBehaviour>();

        public void ResetLevel()
        {
            if (explicitTargets.Length > 0)
            {
                ResetTargets(explicitTargets);
                return;
            }

            ResetTargets(GetComponentsInChildren<MonoBehaviour>(true));
        }

        private static void ResetTargets(MonoBehaviour[] targets)
        {
            foreach (var target in targets)
            {
                if (target is IResettable resettable)
                {
                    resettable.ResetState();
                }
            }
        }
    }
}
