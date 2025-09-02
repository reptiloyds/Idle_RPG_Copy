using PleasantlyGames.RPG.Runtime.UnityExtension.Extensions;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    public class DisableOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.Off();
        }
    }
}
