using MongoDB.Bson.Serialization.Conventions;

namespace FargateDemo
{
    public static class BsonConfig
    {
        public static void RegisterConventionPacks() => ConventionRegistry.Register("conventions",
            new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            },
            _ => true);
    }
}
