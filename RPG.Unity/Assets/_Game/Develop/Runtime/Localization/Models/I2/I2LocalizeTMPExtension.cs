using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Localization.Models.I2
{
    public enum FontType
    {
        Default,
    }

    public enum MaterialType
    {
        Default,
        Outline,
    }
    
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent, HideMonoScript]
    public class I2LocalizeTMPExtension : MonoBehaviour
    {
        [SerializeField] private FontType _fontType;
        [SerializeField] private MaterialType _materialType;
        
        [BoxGroup("Components")]
        [SerializeField, Required, ReadOnly] private TMP_Text _text;
        
        private void Reset()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            LocalizationManager.OnLocalizeEvent += LocalizeTMPVisual;
            LocalizeTMPVisual();
        }

        private void OnDestroy()
        {
            LocalizationManager.OnLocalizeEvent -= LocalizeTMPVisual;
        }

        public void LocalizeTMPVisual()
        {
            LocalizeFont();
            LocalizeMaterial();
        }

        private void LocalizeFont()
        {
            switch (_fontType)
            {
                case FontType.Default:
                    _text.font =
                        LocalizationManager.GetTranslatedObjectByTermName<TMP_FontAsset>(LocalizationKeys.FONT);
                    break;
                default:
                    _text.font =
                        LocalizationManager.GetTranslatedObjectByTermName<TMP_FontAsset>(LocalizationKeys.FONT);
                    break;
            }
        }

        private void LocalizeMaterial()
        {
            switch (_materialType)
            {
                case MaterialType.Default:
                    _text.fontMaterial =
                        LocalizationManager.GetTranslatedObjectByTermName<Material>(LocalizationKeys.FONT_MATERIAL_ZERO_OUTLINE);
                    break;
                case MaterialType.Outline:
                    _text.fontMaterial =
                        LocalizationManager.GetTranslatedObjectByTermName<Material>(LocalizationKeys.FONT_MATERIAL_OUTLINE);
                    break;
                default:
                    _text.fontMaterial =
                        LocalizationManager.GetTranslatedObjectByTermName<Material>(LocalizationKeys.FONT_MATERIAL_ZERO_OUTLINE);
                    break;
            }
        }
    }
}