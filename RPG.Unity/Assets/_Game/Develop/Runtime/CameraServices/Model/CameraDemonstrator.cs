using System;
using Cinemachine;
using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using PleasantlyGames.RPG.Runtime.CameraServices.Defenition;
using PleasantlyGames.RPG.Runtime.CameraServices.Type;
using PleasantlyGames.RPG.Runtime.Pause.Contract;
using PleasantlyGames.RPG.Runtime.Pause.Definition;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using CameraType = PleasantlyGames.RPG.Runtime.CameraServices.Type.CameraType;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Model
{
    [HideMonoScript, DisallowMultipleComponent]
    public class CameraDemonstrator : MonoBehaviour, ICameraDemonstrator
    {
        [SerializeField, Required] private Transform _presenterTarget;

        [HideInEditorMode, ReadOnly, ShowInInspector]
        private CameraDemonstratorState _state;
        
        private CinemachineBrain _brain;
        private float _moveBackTime;
        private bool _skipFrame;
        private bool _awaitStartMovement;

        [Inject] private IPauseService _pauseService;
        [Inject] private PauseConfiguration _pauseConfiguration;
        [Inject] private IVirtualCameraService _virtualCameraPriorityService;
        [Inject] private CameraConfiguration _configuration;
        [Inject] private ICameraProvider _cameraProvider;
        
        public event Action OnComplete;

        void ICameraDemonstrator.Demonstrate(Transform point)
        {
            if (_virtualCameraPriorityService.GetCurrentCameraType() != CameraType.Main)
            {
                OnComplete?.Invoke();
                return;
            }
            
            _pauseService.Pause(_pauseConfiguration.CameraMovement);
            
            _presenterTarget.position = new Vector3(point.position.x, 0, point.position.z);
            SwitchState(CameraDemonstratorState.MoveToTarget);
        }

        private void Awake()
        {
            _brain = _cameraProvider.GetBrain();
            _brain.m_CameraActivatedEvent.AddListener(StartCameraMovement);
        }

        private void OnDestroy() => 
            _brain.m_CameraActivatedEvent.RemoveListener(StartCameraMovement);

        private void StartCameraMovement(ICinemachineCamera arg0, ICinemachineCamera arg1) => 
            _awaitStartMovement = false;

        private void Update()
        {
            if (_awaitStartMovement) return;

            switch (_state)
            {
                case CameraDemonstratorState.None:
                    return;
                case CameraDemonstratorState.MoveToTarget:
                    if(_brain.IsBlending) return;
                    SwitchState(CameraDemonstratorState.LookToTarget);
                    break;
                case CameraDemonstratorState.LookToTarget:
                    if(Time.time < _moveBackTime) return;
                    SwitchState(CameraDemonstratorState.MoveBack);
                    break;
                case CameraDemonstratorState.MoveBack:
                    if(_brain.IsBlending) return;
                    SwitchState(CameraDemonstratorState.None);
                    break;
            }
        }

        private void SwitchState(CameraDemonstratorState state)
        {
            if(_state == state) return;
            _state = state;
            switch (_state)
            {
                case CameraDemonstratorState.None:
                    _pauseService.Continue(_pauseConfiguration.CameraMovement);
                    OnComplete?.Invoke();
                    break;
                case CameraDemonstratorState.MoveToTarget:
                    _virtualCameraPriorityService.SwitchTo(CameraType.Demonstrator);
                    _awaitStartMovement = true;
                    break;
                case CameraDemonstratorState.LookToTarget:
                    _moveBackTime = Time.time + _configuration.DemonstrationTime;
                    break;
                case CameraDemonstratorState.MoveBack:
                    _virtualCameraPriorityService.SwitchTo(CameraType.Main);
                    _awaitStartMovement = true;
                    break;
            }
        }
    }
}
