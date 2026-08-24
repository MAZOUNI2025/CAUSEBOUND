using UnityEngine;

namespace Causebound.UI
{
    public sealed class ScreenRouter : MonoBehaviour
    {
        [SerializeField] private GameObject initialScreen;

        private void Awake()
        {
            if (initialScreen != null)
            {
                initialScreen.SetActive(true);
            }
        }
    }
}
