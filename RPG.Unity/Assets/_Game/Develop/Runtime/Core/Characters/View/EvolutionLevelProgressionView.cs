using System;
using PleasantlyGames.RPG.Runtime.Core.UI.CommonComponents;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    public class EvolutionLevelProgressionView : LevelProgressionView
    {
        [Serializable]
        private class EvolutionImageConfig
        {
            [SerializeField, Required] private GameObject _zeroEvolutionObject;
            [SerializeField, Required] private GameObject _defaultEvolutionObject;

            public void ZeroEvolution()
            {
                _zeroEvolutionObject.gameObject.SetActive(true);
                _defaultEvolutionObject.gameObject.SetActive(false);
            }

            public void DefaultEvolution()
            {
                _zeroEvolutionObject.gameObject.SetActive(false);
                _defaultEvolutionObject.gameObject.SetActive(true);
            }
        }
        
        [SerializeField, Required] private TextMeshProUGUI _evolutionText;
        [SerializeField, Required] private EvolutionImageConfig[] _evolutionImages;

        public void RedrawEvolution(int evolution)
        {
            if (evolution <= 0)
                foreach (var config in _evolutionImages)
                    config.ZeroEvolution();
            else
                foreach (var config in _evolutionImages) 
                    config.DefaultEvolution();
            
            _evolutionText.SetText(evolution.ToString());
        }
    }
}