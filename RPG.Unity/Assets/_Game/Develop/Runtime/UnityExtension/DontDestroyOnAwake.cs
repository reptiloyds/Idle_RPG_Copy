using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UnityExtension
{
    [HideMonoScript, DisallowMultipleComponent]
    public class DontDestroyOnAwake : MonoBehaviour
    {
        private bool _initialized;
        
        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if(_initialized) return;
            _initialized = true;
            DontDestroyOnLoad(this);
        }
    }
}
