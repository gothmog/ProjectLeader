using MongoBaseRepository.Classes;
using MongoDB.Bson.Serialization.Attributes;
using ProjectLeader.Classes.Enums;

namespace ProjectLeader.Classes
{
  public class History: BaseEntity
  {
    public PriorityEnum PriorityValuePast { get; set; }
    public StateEnum StateValuePast { get; set; }
    public TypeEnum TypeValuePast { get; set; }
    public string UserIdPast { get; set; }
    [BsonIgnore]
    public bool IsChanged { get; set; }
  }
}