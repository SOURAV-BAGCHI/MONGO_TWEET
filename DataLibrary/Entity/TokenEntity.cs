using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Entity
{
    public class TokenEntity: IEntity
    {
        public FilterDefinition<T> GetTokenDetails<T>(Guid userid)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("UserId", userid);
            return filter;
        }

        public FilterDefinition<T> GetTokenByClientAndValue<T>(String clientid,String value)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.And(Builders<T>.Filter.Eq("ClientId", clientid), Builders<T>.Filter.Eq("Value", value));

            return filter;
        }
    }
}
