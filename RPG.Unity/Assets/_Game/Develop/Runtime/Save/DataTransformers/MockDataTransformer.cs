namespace PleasantlyGames.RPG.Runtime.Save.DataTransformers
{
    public class MockDataTransformer : IDataTransformer
    {
        public string Transform(string data) => data;

        public string Reverse(string data) => data;
    }
}