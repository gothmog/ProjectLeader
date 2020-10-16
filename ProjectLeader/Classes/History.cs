using MongoBaseRepository.Classes;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLeader.Classes.Enums;
using System;

namespace ProjectLeader.Classes
{
    public class History : BaseEntity
    {
        public PriorityEnum PriorityValuePast { get; set; }
        public StateEnum StateValuePast { get; set; }
        public TypeEnum TypeValuePast { get; set; }
        public DateTime? StartPast { get; set; }
        public DateTime? EndPast { get; set; }
        public string UserIdPast { get; set; }
        [BsonIgnore]
        public bool IsChanged { get; set; }
    }
}