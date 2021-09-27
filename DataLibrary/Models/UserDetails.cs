using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class UserDetails
    {
        [BsonId]
        public Guid Id { get; set; }
        public String Firstname { get; set; }
        public String Lastname { get; set; }
        public String Email { get; set; }
        public String LoginId { get; set; }
        public String Password { get; set; }
        public String Contactnumber { get; set; }
    }
}
