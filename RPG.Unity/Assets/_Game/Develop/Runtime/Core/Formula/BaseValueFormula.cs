namespace PleasantlyGames.RPG.Runtime.Core.Formula
{
    public class BaseValueFormula
    {
        protected BigDouble.Runtime.BigDouble StartValue;

        public BaseValueFormula() { }

        public BaseValueFormula(BigDouble.Runtime.BigDouble startValue) => 
            StartValue = startValue;

        public void SetStartValue(BigDouble.Runtime.BigDouble bigDouble) => 
            StartValue = bigDouble;

        public virtual BigDouble.Runtime.BigDouble CalculateBigDouble(int level)
        {
            if (level == 0)
                return 0;
            return StartValue;
        }
    }
}