using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Tutorials.Feature.Pointer.View
{
    public enum PointerParent
    {
        None = 0,
        Target = 1,
        Parent = 2,
        ParentParent = 3,
    }
    
    [Serializable]
    public class PointerSettings
    {
        public bool Flip;
        public bool ChangeRotation;
        [HideIf("@this.ChangeRotation == false")]
        public Vector3 Rotation;

        public bool Follow;
        [HideIf("@this.Follow == false")]
        [Required] public RectTransform FollowTarget;
        
        public PointerParent ParentType;
        [Required] public RectTransform Target;
        
        public bool OffsetPosition;
        [HideIf("@this.OffsetPosition == false")]
        public Vector2 Offset;
    }
}