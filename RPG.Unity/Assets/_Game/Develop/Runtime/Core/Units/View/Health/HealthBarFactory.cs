using Cysharp.Threading.Tasks;
using PleasantlyGames.RPG.Runtime.Core.UI;
using PleasantlyGames.RPG.Runtime.Pool;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Health
{
    public class HealthBarFactory
    {
        private HealthBar _prefab;
        private ObjectPoolWithParent<HealthBar> _pool;
        
        private UnityEngine.Camera _mainCamera;
        private RectTransform _parent;

        [Inject] private UIFactory _uiFactory;

        [Preserve]
        public HealthBarFactory(){}

        public async UniTask WarmUpAsync()
        {
            var healthBarPrefab = await _uiFactory.LoadAsync(Asset.UI.HealthBar, false);
            _prefab = healthBarPrefab.GetComponent<HealthBar>();
        }

        public void Setup(RectTransform parent, UnityEngine.Camera camera)
        {
            _mainCamera = camera;
            _parent = parent;
            _pool = new ObjectPoolWithParent<HealthBar>(_parent, () => Object.Instantiate(_prefab, _parent),
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy);

            void ActionOnGet(HealthBar healthBar) => 
                healthBar.gameObject.SetActive(true);

            void ActionOnRelease(HealthBar healthBar) => 
                healthBar.gameObject.SetActive(false);

            void ActionOnDestroy(HealthBar healthBar) => 
                Object.Destroy(healthBar.gameObject);
        }

        public HealthBar Create(Transform target, Vector3 offset, UnitHealth health)
        {
            var healthBar = _pool.Get();
            healthBar.SetTarget(target, offset);
            healthBar.SetHealth(health);
            healthBar.Initialize(_mainCamera);

            return healthBar;
        }
        
        public HealthBar Create(Vector3 position, UnitHealth health)
        {
            var healthBar = _pool.Get();
            healthBar.SetHealth(health);
            healthBar.Initialize(_mainCamera);
            healthBar.SetPosition(position);

            return healthBar;
        }

        public void Dispose(HealthBar healthBar)
        {
            healthBar.Dispose();
            _pool.Release(healthBar);
        }
    }
}