using MongoDB.Bson.Serialization.Attributes;

namespace Tweet.Core.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Image { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        [BsonIgnore]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
    }
}
