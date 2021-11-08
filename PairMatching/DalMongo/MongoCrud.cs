using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;

namespace DalMongo
{
    /// <summary>
    /// Crud operation to the database in MongoDB cloud
    /// </summary>
    public class MongoCrud
    {
        private IMongoDatabase db;

        /// <summary>
        /// Constroctor to MongoCrud.<br/>
        /// Create connection to the database as client.  
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        public MongoCrud(string databaseName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["mongoDB"].ConnectionString;
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            var client = new MongoClient(settings);
            db = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Insert one record to the cloud
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Table name (collction)</param>
        /// <param name="record">The recorod to insert</param>
        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        /// <summary>
        /// Loade many record from the cloud
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">The table name (collction)</param>
        /// <returns></returns>
        public List<T> LoadeRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            
            var list = collection.Find(new BsonDocument());
            
            return list.ToList();
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
