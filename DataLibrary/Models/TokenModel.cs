using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class TokenModel
    {
        [BsonId]
        public Guid Id { get; set; }
        // The ClientId, where it comes from
        public string ClientId { get; set; }
        // Value of the Token
        public string Value { get; set; }
        // Get the Token Creation Date
        public DateTime CreatedDate { get; set; }
        // The UserId it was issued to
        public Guid UserId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
