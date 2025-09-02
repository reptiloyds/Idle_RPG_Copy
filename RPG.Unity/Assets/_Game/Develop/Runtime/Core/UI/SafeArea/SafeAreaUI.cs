using System;
using PleasantlyGames.RPG.Runtime.Device;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using DeviceType = PleasantlyGames.RPG.Runtime.Device.DeviceType;

namespace PleasantlyGames.RPG.Runtime.Core.UI.SafeArea
{
    [DisallowMultipleComponent, HideMonoScript]
    public class SafeAreaUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;

        [Inject] private IDeviceService _deviceService;
        
        private Rect _lastSafeArea = Rect.zero; 
        private Vector2Int _lastScreenSize = Vector2Int.zero;
        private DeviceType _deviceType;
        private OrientationType _orientationType;
        private bool _isActive;

        public RectTransform RectTransform => _rectTransform;
        public event Action OnApplied;
        
        private void Reset() => 
            _rectTransform = GetComponent<RectTransform>();

        private void Awake()
        {
            _deviceType = _deviceService.GetDevice();
            _orientationType = _deviceService.GetOrientation();
            if (_deviceType == DeviceType.Mobile)
            {
                ApplySafeArea();
                _isActive = true;
            }
        }

        private void Update()
        {
            if(!_isActive) return;
            if (Screen.safeArea != _lastSafeArea) // || Screen.width != _lastScreenSize.x || Screen.height != _lastScreenSize.y
                ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
        
            if (safeArea != _lastSafeArea)
            {
                _lastSafeArea = safeArea;
                return;
                
                Vector2 anchorMin = safeArea.position;
                Vector2 anchorMax = safeArea.position + safeArea.size; 
                anchorMin.x /= Screen.width;
                //anchorMin.y /= Screen.height;
                anchorMin.y = 0;
                anchorMax.x /= Screen.width;
                //anchorMax.y /= Screen.height;
                anchorMax.y = 1;
            
                _rectTransform.anchorMin = anchorMin;
                _rectTransform.anchorMax = anchorMax;
            }
        
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            
            OnApplied?.Invoke();
        }
    }
}
