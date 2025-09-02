using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Branches.Model;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.Factory;
using PleasantlyGames.RPG.Runtime.Core.Units.Stat.Model;
using PleasantlyGames.RPG.Runtime.Core.Units.View;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.UnitRendering
{
    [DisallowMultipleComponent, HideMonoScript]
    public class UnitRenderer : MonoBehaviour
    {
        [SerializeField, Required] private GameObject _testVisual;
        [SerializeField, Required] private Transform _root;
        [SerializeField, Required] private Transform _cameraSocket;
        [SerializeField, Required] private Light _light;
        [SerializeField, Required] private UnityEngine.Camera _camera;

        private UnitView _unitView;
        private readonly HashSet<UnitRenderRequest> _requests = new();
        private bool _enabled;

        [Inject] private UnitFactory _unitFactory;
        [Inject] private BranchService _branchService;
        [Inject] private CharacterService _characterService;
        [Inject] private UnitStatService _statService;

        public void Initialize()
        {
            _testVisual.gameObject.SetActive(false);

            _characterService.OnEvolved += OnEvolved;
            _characterService.OnAnySwitched += CharacterAnySwitched;
            _branchService.SwitchBranch += OnSwitchBranch;
            UpdateMainCharacterAsync();
            Disable();
        }

        public void AddRequest(UnitRenderRequest request)
        {
            if (_requests.Add(request) && !_enabled) 
                Enable();
        }

        public void RemoveRequest(UnitRenderRequest request)
        {
            if (_requests.Remove(request) && _enabled && _requests.Count == 0)
                Disable();
        }

        private void OnEvolved(Character character) => 
            UpdateMainCharacterAsync();

        private void CharacterAnySwitched() => 
            UpdateMainCharacterAsync();

        private void OnSwitchBranch() => 
            UpdateMainCharacterAsync();

        private async void UpdateMainCharacterAsync()
        {
            if (_unitView != null)
            {
                _unitView.NormalizeModelHeight();
                _unitFactory.Release(_unitView);  
            } 

            var characterId = _branchService.GetSelectedBranch().CharacterId;
            var character = _characterService.GetCharacter(characterId);
            var stats = _statService.GetPlayerStats();
            _unitView = await _unitFactory.CreateAsync(character.UnitId, character.Evolution, isDummy: true);
            _unitView.SetStats(stats);
            _unitView.transform.SetParent(_root);
            _unitView.transform.localPosition = Vector3.zero;
            _unitView.transform.localRotation = Quaternion.identity;
            _unitView.BaseMovement.SetPosition(_root.transform.position);
            _unitView.Initialize();
            _unitView.SetModelHeight(0);
            ApplySettings(_unitView.Render);
        }

        private void ApplySettings(UnitData.Render old)
        {
            _cameraSocket.localPosition = old.Offset;
            _cameraSocket.localRotation = Quaternion.Euler(old.Rotation);
            _camera.transform.localPosition = Vector3.back * old.CameraDistance;
            _light.type = old.LightType;
            _light.range = old.LightRange;
            _light.transform.localPosition = old.LightPosition;
            _light.transform.localRotation = Quaternion.Euler(old.LightRotation);
        }

        private void Disable()
        {
            _enabled = false;
            _light.gameObject.SetActive(false);
            _cameraSocket.gameObject.SetActive(_enabled);
        }

        private void Enable()
        {
            _enabled = true;
            _light.gameObject.SetActive(true);
            _cameraSocket.gameObject.SetActive(_enabled);
        }
    }
}