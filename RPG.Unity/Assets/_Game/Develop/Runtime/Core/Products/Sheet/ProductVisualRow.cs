using System.Collections.Generic;
using Cathei.BakingSheet;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Utilities.Const;
using UnityEngine;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    public class ProductVisualRow : SheetRow<string>
    {
        [Preserve] public string BackColor { get; private set; }
        [Preserve] public ProductContentImageType ContentImageType { get; private set; }
        [Preserve] public string ManualImages { get; private set; }
        [Preserve] public bool HideWhenOver { get; private set; }
        [Preserve] public bool BadgeEnabled { get; private set; }
        [Preserve] public string BadgeText { get; private set; }
        [Preserve] public string BadgeValue { get; private set; }
        [Preserve] public string BadgeStyle { get; private set; }
        
        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                // if (_backgroundColor == Color.white) DeserializeColor(BackColor, out _backgroundColor);
                return _backgroundColor;
            }
        }
        
        private List<string> _contentManualImages;
        public List<string> ContentManualImages => _contentManualImages ??= DeserializeManualImages();
        
        [Preserve]
        protected ProductVisualRow() { }

        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);
            
            DeserializeColor(BackColor, out _backgroundColor);
            if (SheetExt.IsValidationNeeded)
                Validate();
        }

        protected virtual void Validate()
        {
            // DeserializeColor(BackColor, out _backgroundColor);
            if (ContentImageType == ProductContentImageType.Manual)
            {
                foreach (var image in ContentManualImages)
                    SheetExt.CheckSprite(Asset.MainAtlas, image);
            }
        }

        private List<string> DeserializeManualImages()
        {
            if (string.IsNullOrEmpty(ManualImages))
                return null;
            return JsonConvertLog.DeserializeObject<List<string>>(ManualImages);
        }

        protected void DeserializeColor(string hexColor, out Color color) =>
            ColorUtility.TryParseHtmlString(hexColor, out color);
    }
}