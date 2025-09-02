using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.Definition;
using PleasantlyGames.RPG.Runtime.PrivacyPolicy.View;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.PrivacyPolicy.Model
{
    public sealed class PrivacyPolicyService : IInitializable
    {
        private const string PrivacyPolicyKey = "privacyPolicy";
        private const int NotViewed = -1;
        private const int Accepted = 1;
        private const int Rejected = 0;

        private readonly PrivacyPolicyConfiguration _config;
        private readonly UIFactory _uiFactory;
        private PrivacyPolicyView _prefab;
        private PrivacyPolicyView _view;

        private int _value;
        private UniTaskCompletionSource<bool> _completionSource;

        public bool IsViewed => _value != NotViewed;
        public bool IsAccepted => _value == Accepted;

        [Preserve]
        public PrivacyPolicyService(PrivacyPolicyConfiguration config, UIFactory uiFactory)
        {
            _config = config;
            _uiFactory = uiFactory;
        }

        public void Initialize() => 
            _value = PlayerPrefs.GetInt(PrivacyPolicyKey, NotViewed);

        public async UniTask<bool> ShowPrivacyPolicyAsync()
        {
            _completionSource = new UniTaskCompletionSource<bool>();

            _prefab ??= (await _uiFactory.LoadAsync(Asset.UI.PrivacyPolicy, false)).GetComponent<PrivacyPolicyView>();
            _view = Object.Instantiate(_prefab);
            _view.Setup(_config.Uris);
            _view.OnUriClicked += OnUriClicked;
            _view.OnAccepted += OnAccepted;
            _view.OnRejected += OnRejected;
            
            return await _completionSource.Task;
        }

        private void OnUriClicked(PrivacyPolicyUri uri) => 
            Application.OpenURL(uri.Uri);

        private void OnAccepted()
        {
            _value = Accepted;
            PlayerPrefs.SetInt(PrivacyPolicyKey, _value);
            DestroyView();
            _completionSource.TrySetResult(true);
        }

        private void OnRejected()
        {
            _value = Rejected;
            PlayerPrefs.SetInt(PrivacyPolicyKey, _value);
            DestroyView();
            _completionSource.TrySetResult(false);
        }

        private void DestroyView()
        {
            _view.OnUriClicked -= OnUriClicked;
            _view.OnAccepted -= OnAccepted;
            _view.OnRejected -= OnRejected;
            
            Object.Destroy(_view.gameObject);
            _view = null;
        }
    }
}