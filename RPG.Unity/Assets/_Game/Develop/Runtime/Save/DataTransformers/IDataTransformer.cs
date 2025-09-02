namespace PleasantlyGames.RPG.Runtime.Save.DataTransformers
{
    public interface IDataTransformer
    {
        string Transform(string data);
        string Reverse(string data);
    }
}