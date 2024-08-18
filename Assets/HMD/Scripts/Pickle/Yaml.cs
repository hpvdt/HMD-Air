namespace HMD.Scripts.Pickle
{
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class Yaml
    {
        public static readonly INamingConvention convention = CamelCaseNamingConvention.Instance;

        public readonly ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(convention)
            .Build();

        public readonly IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(convention)
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
