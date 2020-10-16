using MongoBaseRepository.Classes;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace ProjectLeader.Classes
{
    public class Resource : BaseEntity
    {
        [DisplayName("Název")]
        public string ResourceName { get; set; }
        public string ResourceImageUrl { get; set; }
        [DisplayName("Cena")]
        public decimal ResourcePrice { get; set; }
        [DisplayName("Počet")]
        public decimal ResourceCount { get; set; }

        [BsonIgnore]
        public string ParentTaskIdString { get; set; }

        [DisplayName("Souhrn")]
        [BsonIgnore]
        public decimal Sum
        {
            get
            {
                return ResourcePrice * ResourceCount;
            }
        }

        [BsonIgnore]
        public string ResourceIdString
        {
            get
            {
                return _id.ToString();
            }
        }
    }
}