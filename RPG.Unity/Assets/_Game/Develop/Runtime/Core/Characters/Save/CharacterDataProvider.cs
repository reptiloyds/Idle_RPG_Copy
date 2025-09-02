using System;
using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Characters.Type;
using PleasantlyGames.RPG.Runtime.Save.Models;
using UnityEngine.Scripting;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.Save
{
    [Serializable]
    public class CharacterDataContainer
    {
        public List<CharacterData> List = new();
        
        [UnityEngine.Scripting.Preserve]
        public CharacterDataContainer()
        {
        }
    }

    [Serializable]
    public class CharacterData
    {
        public string Id;
        public bool IsOwned = false;
        public int Evolution = 0;
        public int Level = 1;
        public int Experience = 0;
        
        [UnityEngine.Scripting.Preserve]
        public CharacterData()
        {
            
        }
    }
    
    public class CharacterDataProvider : BaseDataProvider<CharacterDataContainer>
    {
        [Inject] private CharacterService _characterService;
        [Inject] private BalanceContainer _balanceContainer;

        [UnityEngine.Scripting.Preserve]
        public CharacterDataProvider() { }
        
        public override void LoadData()
        {
            base.LoadData();

            if (Data == null) 
                CreateData();
            else
                ValidateData();

            _characterService.Setup(Data);
        }

        private void CreateData()
        {
            Data = new CharacterDataContainer();
            var characterSheet = _balanceContainer.Get<CharacterSheet>();
            foreach (var characterRow in characterSheet) 
                AddDataElement(characterRow);
        }

        private void ValidateData()
        {
            var characterSheet = _balanceContainer.Get<CharacterSheet>();
            foreach (var config in characterSheet)
            {
                if(HasDataWithId(config.Id)) continue;
                AddDataElement(config);
            }
        }

        private void AddDataElement(CharacterRow config)
        {
            var characterData = new CharacterData()
            {
                Id = config.Id,
                IsOwned = false,
            };
            Data.List.Add(characterData);
        }

        private bool HasDataWithId(string characterId)
        {
            foreach (var characterData in Data.List)
                if (characterData.Id == characterId) return true;

            return false;
        }
    }
}