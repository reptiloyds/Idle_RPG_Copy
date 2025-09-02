using System;
using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PleasantlyGames.RPG.Runtime.Core.PopupNumbers.Model
{
    public class PopupTextFactory
    {
        private UnityEngine.Camera _camera;
        private RectTransform _parent;
        private PopupText _prefab;
        private ObjectPoolWithParent<PopupText> _pool;

        [Inject] private UIFactory _uiFactory;
        
        [Preserve]
        public PopupTextFactory()
        {
            
        }

        public async UniTask WarmUpAsync()
        {
            var popupPrefab = await _uiFactory.LoadAsync(Asset.UI.PopupText, false);
            _prefab = popupPrefab.GetComponent<PopupText>();
        }

        public void Setup(RectTransform parent, UnityEngine.Camera camera)
        {
            _parent = parent;
            _camera = camera;
            
            _pool = new ObjectPoolWithParent<PopupText>(_parent, () => Object.Instantiate(_prefab),
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy);

            void ActionOnGet(PopupText popupText) => 
                popupText.gameObject.SetActive(true);

            void ActionOnRelease(PopupText popupText) => 
                popupText.gameObject.SetActive(false);

            void ActionOnDestroy(PopupText popupText) => 
                Object.Destroy(popupText.gameObject);
        }
        
        public void SpawnFromWorldPosition(Vector3 worldPosition, string content, Color color, Action onComplete = null)
        {
            var uiPosition = _camera.WorldToScreenPoint(worldPosition);

            SpawnFromUI(uiPosition, content, color, onComplete);
        }
    
        public void SpawnFromUI(Vector3 popupPosition, string content, Color color, Action onComplete = null)
        {
            var popupText = _pool.Get();
            popupText.transform.localScale = Vector3.one;
            popupText.transform.position = popupPosition + popupText.GetOffset();
            popupText.Show(content, color);
            popupText.PlayAnimation();
            
            popupText.OnDisappear += OnPopupDisappear;
        }

        private void OnPopupDisappear(PopupText popup)
        {
            popup.OnDisappear -= OnPopupDisappear;
            _pool.Release(popup);
        }
    }
}