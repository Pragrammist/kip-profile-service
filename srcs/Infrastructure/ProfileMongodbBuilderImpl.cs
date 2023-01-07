using ProfileService.Core;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace Infrastructure;



public interface ProfileMongodbBuilder
{
    void Build();
}

public class ProfileMongodbBuilderImpl : ProfileMongodbBuilder
{
    public void Build()
    {
        try
        {
            BsonMemberMap SetStringId<T>(BsonClassMap<T> map) => map.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId)).SetIdGenerator(StringObjectIdGenerator.Instance);

            BsonClassMap.RegisterClassMap<Profile>(map =>
            {
                map.AutoMap();
                SetStringId(map);

            });
        }
        catch (ArgumentException ex)
        {
            if (!ex.Message.ToLower().Contains("an item with the same key has already been added"))
                throw ex;
        }
    }
}
