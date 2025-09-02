using System.Collections.Generic;
using PleasantlyGames.RPG.Runtime.Core.Characters.Model.Bonus;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using VContainer;
using VContainer.Unity;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace PleasantlyGames.RPG.Runtime.Core.Characters.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class CharacterBonusPresenter : MonoBehaviour
    {
        [SerializeField, Required] private CharacterBonusView _bonusPrefab;
        [SerializeField, Required] private Transform _bonusContainer;

        private readonly List<CharacterBonusView> _views = new();
        private CharacterBonusView _selectedView;
        private Tween _hideHintTween;

        [Inject] private IObjectResolver _objectResolver;

        private void Awake()
        {
            if(_objectResolver != null)
                Touch.onFingerUp += OnFingerUp;
        }

        public void Setup(IEnumerable<ICharacterBonus> bonuses)
        {
            int viewId = 0;
            foreach (var bonus in bonuses)
            {
                var view = GetView(viewId);
                view.gameObject.SetActive(true);
                view.Setup(bonus);
                viewId++;
            }

            for (; viewId < _views.Count; viewId++) 
                _views[viewId].gameObject.SetActive(false);
        }
        
        private CharacterBonusView GetView(int id)
        {
            if (id >= _views.Count)
            {
                var view = _objectResolver.Instantiate(_bonusPrefab, _bonusContainer);
                _views.Add(view);
                view.OnClick += OnViewClick;
                return view;
            }
            return _views[id];
        }

        private void OnViewClick(CharacterBonusView view)
        {
            if (_selectedView != null && _selectedView != view) 
                _selectedView.HideHint();
            
            _hideHintTween.Stop();
            _selectedView = view;
            _selectedView.ShowHint();
        }

        private void OnFingerUp(Finger finger)
        {
            if(_selectedView == null) return;
            _hideHintTween.Stop();
            _hideHintTween = Tween.Delay(0.05f, HideSelectedHint);
        }
        
        private void HideSelectedHint()
        {
            if(_selectedView == null) return;
            _selectedView.HideHint();
            _selectedView = null;
        }
    }
}