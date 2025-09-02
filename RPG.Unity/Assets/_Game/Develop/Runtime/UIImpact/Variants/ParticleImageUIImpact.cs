using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UIImpact.Variants 
{
    public class ParticleImageUIImpact : BaseUIImpact
    {
        [SerializeField, Required] private ParticleImage _particleImage;
        [SerializeField] private bool _clearPrevent;
        
        protected override void Play()
        {
            if (_clearPrevent) _particleImage.Clear();
            _particleImage.Play();
        }
    }
}