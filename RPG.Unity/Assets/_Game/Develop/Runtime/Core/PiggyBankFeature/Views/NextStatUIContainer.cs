using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace PleasantlyGames.RPG.Runtime.Core.PiggyBankFeature.Views
{
    public class NextStatUIContainer : MonoBehaviour
    {
        [SerializeField, Required] private Image _image;
        [SerializeField, Required] private Outline _outline;
        [SerializeField] private Color _imageValidColor;
        [SerializeField] private Color _imageInvalidColor;
        [SerializeField] private Color _outlineValidColor;
        [SerializeField] private Color _outlineInvalidColor;

        public void ValidColor()
        {
            _image.color = _imageValidColor;
            _outline.effectColor = _outlineValidColor;
        }

        public void InvalidColor()
        {
            _image.color = _imageInvalidColor;
            _outline.effectColor = _outlineInvalidColor;
        }
    }
}