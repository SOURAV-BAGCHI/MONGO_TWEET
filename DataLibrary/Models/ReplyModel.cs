using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class ReplyModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid TweetId { get; set; }
        public String LoginId { get; set; }
        public String Username { get; set; }
        public String Reply { get; set; }
        public String Tag { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
