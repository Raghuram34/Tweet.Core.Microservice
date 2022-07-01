using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweet.Core.Models
{
    public class TweetModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        public string TweetContent { get; set; }

        public DateTime Date { get; set; } 

        public Profile User { get; set; }

        public List<TweetReply> Replies { get; set; } 

        public List<string> Likes { get; set; }
    }
}
