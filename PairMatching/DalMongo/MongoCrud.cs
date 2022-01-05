using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;

namespace DalMongo
{
    /// <summary>
    /// Crud operation to the database in MongoDB cloud
    /// </summary>
    public class MongoCrud
    {
        // The connections strings of the database
        readonly string _connctionsStrings;

        readonly string _databaseName;

        /// <summary>
        /// Constructor to MongoCrud.<br/>
        /// Create connection to the database as client.  
        /// </summary>
        /// <param name="databaseName">The name of the database</param>
        public MongoCrud(string databaseName)
        {
            _connctionsStrings = ConfigurationManager.ConnectionStrings["mongoDB"].ConnectionString;
            _databaseName = databaseName;
        }

        private IMongoCollection<T> ConnectToMongo<T>(string collectionName)
        {
            var client = new MongoClient(_connctionsStrings);

            var db = client.GetDatabase(_databaseName);

            return db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// Insert one record to the cloud
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Table name (collection)</param>
        /// <param name="record">The record to insert</param>
        public void InsertRecord<T>(string table, T record)
        {
            var collection = ConnectToMongo<T>(table);
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
            var collection = ConnectToMongo<T>(table);
            
            var list = collection.Find(_ => true);
            
            return list.ToList();
        }

        public T LoadeRecordById<T>(string table, int id)
        {
            var collection = ConnectToMongo<T>(table);

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
            var collection = ConnectToMongo<T>(table);
            collection.ReplaceOne(
                new BsonDocument("_id", id),
                record, 
                new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteRecord<T>(string table, int id)
        {
            var collection = ConnectToMongo<T>(table);

            var filter = Builders<T>.Filter.Eq("Id", id);

            collection.DeleteOne(filter);
        }

        public async Task InsertManyAsync<T>(string table, IEnumerable<T> records)
        {
            var collection = ConnectToMongo<T>(table);
            await collection.InsertManyAsync(records);
        }
    }
}
