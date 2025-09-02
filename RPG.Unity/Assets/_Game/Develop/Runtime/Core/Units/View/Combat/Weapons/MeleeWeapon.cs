namespace PleasantlyGames.RPG.Runtime.Core.Units.View.Combat.Weapons
{
    public class MeleeWeapon : Weapon
    {
        public override WeaponType Type => WeaponType.Melee;

        public override void PerformAttack(UnitView target) => 
            AttackPerformedTo(target);

        public override void CancelAttack()
        {
        }
    }
}