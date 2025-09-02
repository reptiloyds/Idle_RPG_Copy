using System;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Audio.View
{
    public class GenericAudioLauncher<T> : AudioLauncher where T : Enum
    {
        [SerializeField] private T _audioType;
        
        public override void Play()
        {
            AudioService.CreateLocalSound(_audioType)
                .WithPosition(_target.position)
                .Play();
        }
    }
}