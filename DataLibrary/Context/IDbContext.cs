using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public interface IDbContext
    {
        public Task InsertRecordAsync<T>(String Table, T Record);
        public Task<List<T>> LoadRecordsAsync<T>(String table);
        public Task<T> LoadRecordByIdAsync<T>(String table, Guid Id);
        public Task<List<T>> LoadRecordByFilterAsync<T>(String table, FilterDefinition<T> filter);
        public Task UpsertRecordAsync<T>(String table, Guid id, T record);
        public Task DeleteRecordAsync<T>(String table, Guid id);
        public Task DeleteRecordByFilterAsync<T>(String table, FilterDefinition<T> filter);
        public IEntity LoadContext(String table);
    }
}
