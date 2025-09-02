using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Camera
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CameraSafeAreaRender : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Camera _cam;
        private float _screenWidth;
        private float _screenHeight;
        private Rect _lastSafeArea;

        private void Start()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            ApplySafeArea();
        }

        [Button]
        private void ApplySafeArea()
        {
            _lastSafeArea = Screen.safeArea;

            return;
             var x = _lastSafeArea.x / _screenWidth;
             //var y = _lastSafeArea.y / _screenHeight;
             var width = _lastSafeArea.width / _screenWidth;
             //var height = _lastSafeArea.height / _screenHeight;

             var origin = _cam.rect;
            _cam.rect = new Rect(x, origin.y, width, origin.height);
        }

        private void Update()
        {
            if (Screen.safeArea != _lastSafeArea) 
                ApplySafeArea();
        }
    }
}