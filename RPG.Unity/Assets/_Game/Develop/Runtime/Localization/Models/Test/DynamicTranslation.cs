using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using TMPro;

namespace PleasantlyGames.RPG.Runtime.Localization.Models.Test
{
    public class DynamicTranslation
    {
        private readonly ITranslator _localization;
        private readonly TextMeshProUGUI _text;
        
        private string _textToken;
        private string _fontToken;
        private string _materialToken;
        
        public DynamicTranslation(ITranslator localization, TextMeshProUGUI text)
        {
            _localization = localization;
            _text = text;
        }

        public void SetTextToken(string textToken)
        {
            _textToken = textToken;
        }

        public void SetFontToken(string fontToken)
        {
            _fontToken = fontToken;
        }

        public void SetMaterialToken(string materialToken)
        {
            _materialToken = materialToken;
        }
    }
}