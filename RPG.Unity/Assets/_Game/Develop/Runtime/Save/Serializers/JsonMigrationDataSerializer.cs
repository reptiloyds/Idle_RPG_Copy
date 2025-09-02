using FastMigrations.Runtime;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Save.Serializers
{
    public class JsonMigrationDataSerializer : IDataSerializer
    {
        [Preserve]
        public JsonMigrationDataSerializer()
        {
            
        }
        
        public string Serialize<T>(T data)
        {
            var migrator = new FastMigrationsConverter(MigratorMissingMethodHandling.ThrowException);
            return JsonConvert.SerializeObject(data, migrator);
        }

        public T Deserialize<T>(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;
            var migrator = new FastMigrationsConverter(MigratorMissingMethodHandling.ThrowException);
            return JsonConvert.DeserializeObject<T>(data, migrator);
        }

        public string Transform(string data) => data;

        public string Reverse(string data) => data;
    }
}