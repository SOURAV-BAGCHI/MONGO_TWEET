using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class TweetModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public String Tweet { get; set; }
        public String Tag { get; set; }
        public DateTime CreateDateTime { get; set; }
        public String Username { get; set; } //User firstname + lastname
        public String LoginId { get; set; }
    }
}
