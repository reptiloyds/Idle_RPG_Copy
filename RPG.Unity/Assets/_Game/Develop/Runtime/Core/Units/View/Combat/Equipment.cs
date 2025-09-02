using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons;
using PleasantlyGames.RPG.Runtime.Core.Units.View.VisualStates.SubStates.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat
{
    public class Equipment : UnitComponent
    {
        [SerializeField] private Transform _weaponSocket;
        [SerializeField] private Weapon _weapon;
        [SerializeField] private AttackIdleSubState _attackIdleSubState;
        [SerializeField] private AttackSubState _attackSubState;

        public Weapon Weapon => _weapon;

        protected override void GetComponents()
        {
            base.GetComponents();

            _attackIdleSubState = Unit.transform.GetComponentInChildren<AttackIdleSubState>();
            _attackSubState = Unit.transform.GetComponentInChildren<AttackSubState>();
        }

        public void Setup(Transform weaponSocket, Weapon weapon, bool normalizeScale = true)
        {
            _weaponSocket = weaponSocket;
            _weapon = weapon;
            
            _weapon.transform.SetParent(_weaponSocket);
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localRotation = Quaternion.identity;
            if (normalizeScale) 
                _weapon.transform.localScale = Vector3.one;
            
            _attackIdleSubState.SetupEffects(_weapon.IdleEffects);
            _attackSubState.SetupEffects(_weapon.AttackEffects);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            
            _weapon.SetTeamType(Unit.TeamType);
        }

        //TODO PLACE WEAPON
    }
}