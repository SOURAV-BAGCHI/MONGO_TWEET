using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class TweetLikeModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid TweetId { get; set; }
        public String LoginId { get; set; }
        public String Username { get; set; }
        public Boolean Like { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
