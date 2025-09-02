using Cathei.BakingSheet;
using Cathei.BakingSheet.Internal;
using PleasantlyGames.RPG.Runtime.Core.Balance.Container.Extension;
using PleasantlyGames.RPG.Runtime.Core.Formula;
using PleasantlyGames.RPG.Runtime.Core.Formula.Extension;
using PleasantlyGames.RPG.Runtime.Utilities.Const;

namespace PleasantlyGames.RPG.Runtime.Core.BlessingFeature.Sheet
{
    public class BlessingRow : SheetRowArray<string, BlessingElem>
    {
        [Preserve] public string LocalizationToken { get; private set; }
        [Preserve] public string ImageName { get; private set; }
        [Preserve] public int MaxLevel { get; private set; }
        [Preserve] public FormulaType LevelFormula { get; private set; }
        [Preserve] public string LevelFormulaJSON { get; private set; }

        [Preserve]
        public BlessingRow()
        {
            
        }
        
        public override void PostLoad(SheetConvertingContext context)
        {
            base.PostLoad(context);

            if (SheetExt.IsValidationNeeded) 
                Validate();
        }

        private void Validate()
        {
            LevelFormula.DeserializeFormula(LevelFormulaJSON);
            SheetExt.CheckSprite(Asset.MainAtlas, ImageName);
        }
    }
}