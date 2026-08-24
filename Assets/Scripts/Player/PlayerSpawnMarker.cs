using UnityEngine;

namespace Causebound.Player
{
    public sealed class PlayerSpawnMarker : MonoBehaviour
    {
        [SerializeField] private string spawnId = "player-spawn";

        public string SpawnId => spawnId;
    }
}
