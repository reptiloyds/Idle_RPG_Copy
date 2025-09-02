using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.UIImpact.Variants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.UIImpact 
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UIImpactListener : MonoBehaviour
    {
        [SerializeField] private List<BaseUIImpact> _impacts = new ();

        [Button]
        private void SearchImpacts()
        {
            _impacts.Clear();
            var impacts =  GetComponentsInChildren<BaseUIImpact>();
            foreach (var impact in impacts)
            {
                if (impact.transform.parent.TryGetComponent(out UIImpactListener impactListener) && impactListener != this) continue;
                _impacts.Add(impact);
            }
        }
        
        public void Play()
        {
            foreach (var impact in _impacts) 
                impact.InvokePlay();
        }
    }
}
