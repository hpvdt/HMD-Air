namespace HMD.Scripts.Pickle
{
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class Yaml
    {
        public readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public string Fwd<T>(T value)
        {
            return Serializer.Serialize(value);
        }

        public T Rev<T>(string yaml)
        {
            return Deserializer.Deserialize<T>(yaml);
        }
    }

}
