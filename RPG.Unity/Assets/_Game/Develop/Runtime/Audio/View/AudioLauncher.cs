using PleasantlyGames.RPG.Runtime.Audio.Contract;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Audio.View
{
    [HideMonoScript]
    public abstract class AudioLauncher : MonoBehaviour
    {
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] protected Transform _target;

        [Inject] protected IAudioService AudioService;

        private void OnEnable()
        {
            if(_playOnEnable)
                Play();
        }

        [Button]
        public abstract void Play();
    }
}