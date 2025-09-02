using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.Branches.View
{
    [DisallowMultipleComponent, HideMonoScript]
    public class BranchMarkView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;

        public void Setup(Sprite sprite, Transform parent)
        {
            _rectTransform.SetParent(parent);
            _rectTransform.localPosition = Vector3.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            _image.sprite = sprite;
        }
    }
}