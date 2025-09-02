using Sirenix.OdinInspector;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.UI.ScreenClamper
{
    [DisallowMultipleComponent, HideMonoScript]
    public class ScreenFitter : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField, MinValue(0)] private float _offset = 15;

        public void Fit(RectTransform target = null, float offset = -1)
        {
            if (target == null) target = _target;
            if (offset < 0) offset = _offset;
            
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            Vector3 lowerLeft = corners[0];
            Vector3 upperRight = corners[2];

            Vector3 correction = Vector3.zero;

            if (lowerLeft.x < offset)
                correction.x += offset - lowerLeft.x;

            if (lowerLeft.y < offset)
                correction.y += offset - lowerLeft.y;

            if (upperRight.x > screenWidth - offset)
                correction.x += (screenWidth - offset) - upperRight.x;

            if (upperRight.y > screenHeight - offset)
                correction.y += (screenHeight - offset) - upperRight.y;

            target.position += correction;
        }
    }
}