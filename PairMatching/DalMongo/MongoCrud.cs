using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace DalMongo
{
    public class MongoCrud
    {
        private IMongoDatabase db;

        public MongoCrud(string databaseName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["mongoDB"].ConnectionString;
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            var client = new MongoClient(settings);
            //var client = new MongoClient();
            db = client.GetDatabase(databaseName);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadeRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            
            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadeRecordById<T>(string table, int id)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("Id", id);

            var obj = collection.Find(filter);
            if (obj.Any())
            {
                return obj.First();
            }
            return default;
        }

        public void UpsertRecord<T>(string table, int id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record, 
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, int id)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("Id", id);

            collection.DeleteOne(filter);
        }

        public async Task InsertManyAsync<T>(string table, IEnumerable<T> records)
        {
            var collection = db.GetCollection<T>(table);
            await collection.InsertManyAsync(records);
        }
    }
}
