using System;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Combat;
using PleasantlyGames.RPG.Runtime.Core.Units.View.Movement;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Units.View
{
    [CreateAssetMenu(fileName = nameof(UnitData), menuName = "SO/Unit/" + nameof(UnitData))]
    public class UnitData : ScriptableObject
    {
        public Vector3 VisualOffset;
        [Min(0)] public float ProjectileOffset;

        public Render RenderData;
        public Health HealthData;
        public Movement MovementData;
        public Physics PhysicsData;
        public Combat CombatData;
        
        [Serializable]
        public class Combat
        {
            public DamageProviderType ProviderType = DamageProviderType.Self;
            public DamageDecoratorType Decorator = DamageDecoratorType.DefaultAlly;
        }
        
        [Serializable]
        public class Physics
        {
            [Min(0)] public float Height;
            [Min(0)] public float Radius;
        }
        
        [Serializable]
        public class Health
        {
            public bool HideWhenFull;
            [Min(0)] public float ViewOffset;
        }
        
        [Serializable]
        public class Movement
        {
            public MovementType Type;
            public bool AnimateFakeMovement;
            [HideIf("@this.AnimateFakeMovement == false")]
            public TweenSettings<float> StartFakeMovement;
            [HideIf("@this.AnimateFakeMovement == false")]
            public TweenSettings<float> StopFakeMovement;
        }
        
        [Serializable]
        public class Render
        {
            public float CameraDistance;
            public Vector3 Rotation;
            public Vector3 Offset = Vector3.up;
            public LightType LightType = LightType.Spot;
            public int LightRange = 1;
            public Vector3 LightPosition;
            public Vector3 LightRotation;
        }
    }
}