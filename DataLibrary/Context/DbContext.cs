using DataLibrary.Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class DbContext : IDbContext
    {
        private IMongoDatabase db;
        public DbContext(String database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public async Task InsertRecordAsync<T>(String table, T record)
        {
            var collection = db.GetCollection<T>(table);
            await collection.InsertOneAsync(record);
        }

        public async Task<List<T>> LoadRecordsAsync<T>(String table)
        {
            var collection = db.GetCollection<T>(table);

            var res = await collection.FindAsync(new BsonDocument());
            return res.ToList();
        }

        public async Task<T> LoadRecordByIdAsync<T>(String table,Guid Id)
        {
            var collection = db.GetCollection<T>(table);
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", Id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> LoadRecordByFilterAsync<T>(String table,FilterDefinition<T> filter)
        {
            var collection = db.GetCollection<T>(table);
            var res = await collection.FindAsync(filter);
            return await res.ToListAsync();
        }

        public async Task UpsertRecordAsync<T>(String table,Guid id,T record)
        {
            var collection = db.GetCollection<T>(table);
            var result = await collection.ReplaceOneAsync(
                new BsonDocument("_id", id),
                record,
                new ReplaceOptions { IsUpsert = true });
        }

        public async Task DeleteRecordAsync<T>(String table,Guid id)
        {
            var collection = db.GetCollection<T>(table);
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
            await collection.DeleteOneAsync(filter);
        }

        public async Task DeleteRecordByFilterAsync<T>(String table, FilterDefinition<T> filter)
        {
            var collection = db.GetCollection<T>(table);
            
            await collection.DeleteOneAsync(filter);
        }

        public IEntity LoadContext(String table)
        {
            return table switch
            {
                "Users" => new UserEntity(),
                "Tokens"=> new TokenEntity(),
                "Tweets"=> new TweetEntity(),
                _ => new UserEntity()
            };

        }
    }
}
