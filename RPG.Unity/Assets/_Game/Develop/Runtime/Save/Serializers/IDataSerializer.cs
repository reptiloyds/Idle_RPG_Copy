using PleasantlyGames.RPG.Runtime.Save.DataTransformers;

namespace PleasantlyGames.RPG.Runtime.Save.Serializers
{
    public interface IDataSerializer : IDataTransformer
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string data);
    }
}