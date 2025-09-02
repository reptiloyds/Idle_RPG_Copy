using Microsoft.Extensions.Logging;
using PleasantlyGames.RPG.Runtime.Core.Balance.Attributes;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container;
using PleasantlyGames.RPG.Runtime.Core.Products.Offers.Sheet;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Sheet
{
    public class ProductsContainer : CustomSheetContainer
    {
        [Preserve] [SheetList("Products")] public ProductsSheet products { get; private set; }
        [Preserve] [SheetList("Products")] public HorizontalProductVisualSheet horizontalVisual { get; private set; }
        [Preserve] [SheetList("Products")] public VerticalProductVisualSheet verticalVisual { get; private set; }
        [Preserve] [SheetList("Products")] public ProductOffersSheet productOffers { get; private set; }

        public ProductsContainer(ILogger logger) : base(logger)
        {
        }

        protected override void PostLoadRuntime()
        {
            base.PostLoadRuntime();
            Balance.Set(products);
            Balance.Set(horizontalVisual);
            Balance.Set(verticalVisual);
            Balance.Set(productOffers);
        }
    }
}