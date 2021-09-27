using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Entity
{
    public class TweetEntity:IEntity
    {
        public FilterDefinition<T> GetUserTweets<T>(String loginid)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("LoginId", loginid);
            return filter;
        }
    }
}
