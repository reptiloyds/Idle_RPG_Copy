namespace PleasantlyGames.RPG.NutakuRuntime.Nutaku.Server.Exceptions
{
    public sealed class ExceptionData
    {
        public readonly string DataJson;
        public readonly string Text;
        public readonly string Type;

        public ExceptionData(string dataJson, string text, string type)
        {
            DataJson = dataJson;
            Text = text;
            Type = type;
        }
    }
}