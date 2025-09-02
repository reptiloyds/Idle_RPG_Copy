using System.Linq;
using Animancer;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Logger = PleasantlyGames.RPG.Runtime.DebugUtilities.Logger;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Building
{
    [HideMonoScript]
    public class UnitViewBuilder : MonoBehaviour
    {
        private enum RotatorType
        {
            Self = 0,
            WeaponSocket = 1,
        }

        [SerializeField, ReadOnly] private UnitView _unit;
        [SerializeField] private UnitData _data;
        [SerializeField] private Transform _weaponSocket;
        [SerializeField] private Transform _particlePoint;
        [SerializeField] private RotatorType _rotatorType;
        [SerializeField] private bool _normalizeWeaponScale = true;

        private void Reset() => GetComponents();
        private void OnValidate() => GetComponents();

        private void GetComponents() =>
            _unit ??= GetComponent<UnitView>();

        [Button]
        public void Build()
        {
            var errorCount = 0;
            TrySetAnimator(ref errorCount);
            TrySetParticlePoint(ref errorCount);
            TrySetWeapon(ref errorCount);
            TrySetRotator(ref errorCount);
            CheckEnemyTargets(ref errorCount);
            CheckBuildElements(ref errorCount);
            TrySetData(ref errorCount);
            if (errorCount == 0)
                Logger.Log("Unit view built successfully.");
        }

        private void TrySetAnimator(ref int errorCount)
        {
            var animancer = _unit.GetComponentInChildren<AnimancerComponent>();
            if (animancer == null) return;
            animancer.Animator = _unit.Visual.GetComponentInChildren<Animator>();
            if (animancer.Animator != null) return;
            errorCount++;
            Logger.LogError($"No {nameof(Animator)} found on the visual and unit has an {nameof(AnimancerComponent)}");
        }

        private void TrySetParticlePoint(ref int errorCount)
        {
            if (_particlePoint == null)
                _particlePoint = FindByTagRecursive(_unit.Visual.transform, "ParticlePoint");
            if (_particlePoint != null)
                _unit.SetParticlePoint(_particlePoint);
            else
            {
                errorCount++;
                Logger.LogError($"No {nameof(Transform)} with tag 'ParticlePoint' found on the visual");
            }
        }

        private void TrySetWeapon(ref int errorCount)
        {
            if (_weaponSocket == null)
                _weaponSocket = FindByTagRecursive(_unit.Visual.transform, "WeaponSocket");
            if (_weaponSocket != null)
            {
                var weapons = _unit.Visual.GetComponentsInChildren<Weapon>();
                if (weapons.Length > 1)
                {
                    errorCount++;
                    foreach (var weapon in weapons)
                        Logger.LogError($"More than one {nameof(Weapon)} found on the visual", weapon.gameObject);
                    return;
                }

                var singleWeapon = weapons.FirstOrDefault();
                if (singleWeapon != null)
                    _unit.Equipment.Setup(_weaponSocket, singleWeapon, _normalizeWeaponScale);
                else
                {
                    errorCount++;
                    Logger.LogError($"No {nameof(Weapon)} found on the visual");
                }
            }
            else
            {
                errorCount++;
                Logger.LogError($"No {nameof(Transform)} with tag 'WeaponSocket' found on the visual");
            }
        }

        private void TrySetRotator(ref int errorCount)
        {
            switch (_rotatorType)
            {
                case RotatorType.Self:
                    _unit.Rotator.SetLookObject(_unit.transform);
                    break;
                case RotatorType.WeaponSocket:
                    if (_weaponSocket != null)
                        _unit.Rotator.SetLookObject(_weaponSocket);
                    else
                        errorCount++;
                    break;
                default:
                    errorCount++;
                    Logger.LogError($"Unknown rotator type {_rotatorType}");
                    break;
            }
        }

        private void CheckEnemyTargets(ref int errorCount)
        {
            var enemyTargets = _unit.transform.GetComponentInChildren<EnemyTargets>();
            if (enemyTargets == null)
            {
                errorCount++;
                Logger.LogError($"No {nameof(EnemyTargets)} found on the visual");
            }
            else if (_unit.EnemyTargets == null)
                _unit.SetEnemyTargets(enemyTargets);
        }

        private void CheckBuildElements(ref int errorCount)
        {
            var updateableBuildElements = _unit.GetComponentsInChildren<IUpdateableBuildElement>();
            foreach (var updateableBuildElement in updateableBuildElements) 
                updateableBuildElement.UpdateState(_unit);
            var buildElements = _unit.GetComponentsInChildren<IBuildElement>();
            foreach (var buildElement in buildElements)
                buildElement.LogIfWrong(ref errorCount);
        }

        private void TrySetData(ref int errorCount)
        {
            if (_data != null)
                _unit.SetData(_data);
            else
            {
                errorCount++;
                Logger.LogError($"No {nameof(UnitData)} provided");
            }
        }

        private Transform FindByTagRecursive(Transform parent, string tagName)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tagName))
                    return child;

                var found = FindByTagRecursive(child, tagName);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}