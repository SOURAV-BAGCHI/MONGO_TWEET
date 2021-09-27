using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary
{
    public class UserEntity:IEntity
    {
       public FilterDefinition<T> CheckUserValidity<T>(String email,String loginid)
       {
            FilterDefinition<T> filter = Builders<T>.Filter.Or(Builders<T>.Filter.Eq("Email", email), Builders<T>.Filter.Eq("LoginId", loginid));

            return filter;
       }

       public FilterDefinition<T> GetUserDetails<T>(String loginid)
       {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("LoginId", loginid);
            return filter;
       }
    }
}
